# Hints — Things Easy to Forget

Architecture reminders and gotchas to keep in mind across both iterations to avoid painful refactors later.

---

## Access Control

**Always scope every query by `workspace_id`.**
Every endpoint receives a `workspace_id` from the URL. Always include `WHERE workspace_id = :workspace_id` in every query — never trust the request body for this. Middleware should verify the current user is a member of that workspace before any handler runs.

**Private events need two checks.**
When listing events, filter like this:
```sql
WHERE workspace_id = :workspace_id
  AND (is_private = false OR user_id = :current_user_id)
```
Both conditions matter. Do not leak private events to other workspace members under any circumstance.

**Notifications only go to workspace members.**
Before creating `event_notifications` rows, validate that every user in `assigned_to` is an active member of the workspace. If someone is removed from a workspace, their pending notifications should be cancelled or ignored.

---

## Data Integrity

**`assigned_to` is a JSONB array, not a foreign key.**
This means the database will not enforce membership. Validate at the application layer on create and on update. When a user leaves a workspace, run a cleanup to remove their ID from any `assigned_to` arrays in that workspace's events.

**`created_from_pattern_id` uses `ON DELETE SET NULL`.**
If a pattern is deleted, events created from it keep their data — only the link is broken. This is intentional: events are independent records after creation.

**Soft delete patterns, hard delete events.**
Patterns use `is_active = false` (soft delete) because `next_due_date` history is useful for the future. Events can be hard deleted since they cascade to `event_notifications`.

**`next_due_date` must be updated atomically with event creation.**
When a user converts a pattern to an event, update `pattern.next_due_date` and `pattern.last_triggered_at` in the same database transaction as the event insert. If the transaction fails, roll everything back.

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

**`frequency` will need to expand.**
`every_2_years` as a string enum works now. When you add recurring *events* (weekly tennis), you will need `rrule` support. Design the `recurring_patterns.frequency` field to be easily migratable — consider storing an `rrule_string` column alongside the current enum from day one (nullable). No logic needed now, just reserve the column.

**`notification_type` should be an enum or constrained.**
Currently only `email` is supported. Add a check constraint now:
```sql
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
