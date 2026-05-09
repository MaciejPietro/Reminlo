# Hints — Things Easy to Forget

Architecture reminders and gotchas to keep in mind across both iterations to avoid painful refactors later.

---

## Access Control

**Always scope every query by `workspace_id`.**
Every endpoint receives a `workspace_id` from the URL. Always include `WHERE workspace_id = :workspace_id` in every query — never trust the request body for this. Middleware should verify the current user is a member of that workspace before any handler runs.

**Check workspace freeze status before any operation.**
If a workspace's `OwnerId IS NULL`, the workspace is frozen. Return 403 Forbidden for any read/create/update/delete operations on that workspace's obligations. Display a message: "This workspace is inactive. Contact support."

**Obligation visibility filtering.**
When listing obligations, filter like this:
```sql
WHERE workspace_id = :workspace_id
  AND (
    visible_to IS NULL  -- visible to all
    OR created_by = :current_workspace_member_id  -- creator always sees their own
    OR :current_user_id = (SELECT owner_id FROM workspaces WHERE id = workspace_id)  -- workspace owner sees all
    OR :current_workspace_member_id = ANY(visible_to)  -- listed in visible_to
  )
```
All four conditions matter. Compare `created_by` and `visible_to` against the CURRENT USER'S `workspace_member(id)`, not their user ID. Workspace owner always sees everything regardless of visibility. Do not leak obligations to workspace members who shouldn't see them.

**Obligation `created_by` and `visible_to` reference WorkspaceMember.**
Both fields use `workspace_member(id)`, not `user(id)`. This enforces that:
- Creator must be an active workspace member at creation time
- Only active workspace members can be in `visible_to` arrays

When updating `visible_to` array, validate that all member IDs are:
1. Active members of the same workspace
2. In Active status (not Removed/Inactive)
Reject the request if any ID is invalid.

On workspace member removal, run cleanup to remove that member from all `visible_to` arrays in that workspace. Use `ON DELETE RESTRICT` for `created_by` to prevent deleting a member if they created obligations (or soft-delete the obligation instead).

**Obligations require workspace owner for updates.**
Only `created_by` user OR workspace owner can update/delete obligations. Other workspace members get 403 Forbidden. Categories are global and require super admin.

**Notifications only go to workspace members.**
Before creating `event_notifications` rows, validate that every user in `assigned_to` is an active member of the workspace. If someone is removed from a workspace, their pending notifications should be cancelled or ignored.

---

## Data Integrity

**Obligation reminders are separate and optional.**
An obligation can exist without any reminders. Reminders are stored in a separate `obligation_reminders` table with a foreign key to `obligations`. Allow users to add, update, or delete reminders independently of the obligation. Validate `reminder_days` is not negative and `is_active` determines whether reminders are processed by the cron job.

**Obligation categories are global and immutable after creation.**
Categories are not scoped to workspaces — they're application-wide. Only super admins can create/edit/delete. Once a category is created, users reference it by `category_id`. Do NOT allow renaming or deletion if obligations exist with that category (soft-delete/archive the category instead). This prevents data inconsistency across workspaces.

**`visible_to` is a UUID array of WorkspaceMember IDs.**
Like `assigned_to` in events, this is stored as a UUID[] array. While `created_by` has a foreign key constraint, `visible_to` does NOT (it's an array). Validate at the application layer:
1. On obligation creation: check all member IDs exist, are active, and belong to the same workspace
2. On obligation update: re-validate the new array
3. On member removal: run cleanup to remove them from all `visible_to` arrays in their workspace
4. On workspace deletion: cascade delete all obligations

Future: consider migrating to a join table if obligations need member roles (organizer, viewer, etc.)

**`assigned_to` is a JSONB array, not a foreign key.**
This means the database will not enforce membership. Validate at the application layer on create and on update. When a user leaves a workspace, run a cleanup to remove their ID from any `assigned_to` arrays in that workspace's events.

**`created_from_obligation_id` uses `ON DELETE SET NULL`.**
If an obligation is deleted, events created from it keep their data — only the link is broken. This is intentional: events are independent records after creation.

**Soft delete obligations, hard delete events.**
Obligations use `is_active = false` (soft delete) because `next_due_date` history is useful for the future. Events can be hard deleted since they cascade to `event_notifications`.

**`next_due_date` must be updated atomically with event creation.**
When a user converts an obligation to an event, update `obligation.next_due_date` in the same database transaction as the event insert. If the transaction fails, roll everything back.

---

## Notifications

**The cron job must be idempotent.**
If it runs twice in quick succession (restart, duplicate job), it should not send duplicate emails. The `sent_at IS NULL` check in the query handles this naturally. Add a database-level unique constraint or optimistic locking if needed.

**Add `retry_count` to `event_notifications` now, even if you don't use it yet.**
When SendGrid fails, you'll want to retry without accidentally re-querying all unsent rows. Add `retry_count INT DEFAULT 0` and `last_error TEXT` columns from the start. Free to add now, expensive to migrate later.

**Email delivery is async and fallible.**
Never mark a notification as sent before the email provider confirms delivery. Always mark `sent_at` in a callback or after a successful API response — not optimistically before the call.

---

## Schema Future-Proofing

**`frequency` determines obligation type.**
`frequency` can be NULL (one-time), `weekly`, `monthly`, `yearly`, or `custom`.
- NULL: user-specified deadline, no recurrence
- `weekly`: recurs every 7 days
- `monthly`: recurs every 30 days (or use day-of-month logic if needed later)
- `yearly`: recurs every 365 days
- `custom`: reserved for future `rrule` support

When you add recurring *events* (weekly tennis), you will need `rrule` support. Design to be migratable — consider adding an `rrule_string` column (nullable) from day one. No logic needed now, just reserve it.

**`next_due_date` has different meanings:**
For one-time obligations (frequency=NULL), it's the deadline (user-specified). For recurring obligations, it's the next occurrence (calculated from now + frequency interval). Always store in UTC; display in user's timezone on frontend.

**`notification_type` should be an enum or constrained.**
Currently only `email` is supported for both obligation reminders and event notifications. Add check constraints now on both tables:
```sql
-- On obligation_reminders table
CHECK (reminder_type IN ('email', 'in_app', 'sms', 'push'))

-- On event_notifications table
CHECK (notification_type IN ('email', 'in_app', 'sms', 'push'))
```
This prevents invalid data and documents the intended future values.

**`assigned_to` might need to become a join table.**
`JSONB []` is fine for iteration 2. By iteration 3 (proposals, conflict detection), you will likely need `event_members` with roles (`organizer`, `attendee`, `optional`). Design the JSON structure now to match: `[{"user_id": "...", "role": "attendee"}]` instead of a plain array of IDs. Migrating to a join table later is straightforward if the shape is already structured.

---

## Calendar Logic

**Always store times in UTC.**
Display in the user's local timezone on the frontend. Never store local time in the database. Add `timezone VARCHAR(64)` to the `users` table now (e.g., `"Europe/Warsaw"`) even if you don't use it until iteration 3.

**`start_time` must be before `end_time`.**
Validate this on the backend, not just the frontend. Add a database check constraint:
```sql
CHECK (end_time > start_time)
```

**All-day events are a future edge case.**
For now, require both `start_time` and `end_time`. Do not add `is_all_day` until you actually need it — it complicates timezone handling significantly.

---

## Security

**Never expose `workspace_id` logic to the client.**
The client passes `workspace_id` in the URL, but the backend must re-verify membership on every request. Middleware should do this — not individual handlers.

**`assigned_to` user IDs must be validated.**
A user could pass arbitrary UUIDs in `assigned_to`. Always cross-reference against actual workspace members before persisting.
