# Plan

## Overview

Two iterations, focused on the most impactful problems first: remembering recurring obligations, then scheduling concrete events on a calendar. 
Gamification, conflict detection, SMS/push notifications, and AI features are explicitly out of scope.

---

## Iteration 1 — Recurring Obligations

Goal: users define recurring life obligations (insurance renewal, car service, medical tests) and optionally set up email reminders before the due date.
No calendar entry is created automatically — the user converts an obligation to an event manually when ready.

### Tasks

1. Create `obligation_categories` table with fields: `id`, `name` (unique), `created_at`, `updated_at` — super admin only CRUD
2. Create `obligations` table with fields: `id`, `workspace_id`, `created_by` (FK to workspace_members), `category_id`, `name`, `description`, `frequencyInterval` (nullable), `frequencyValue` (nullable), `priority`, `nextDate`, `expirationDate`, `visibleTo` (collection of WorkspaceMember), `status`
3. Create `obligation_reminders` table with fields: `id`, `obligation_id`, `reminderDays`, `notificationType`, `status`
4. Add access control:
   - Only creator or workspace owner can update/delete obligations
   - Super admin can manage categories
   - Respect visibility rules: visible_to (NULL=all, []=creator+owner, [ids]=creator+owner+members)
5. Build CRUD endpoints:
   - `/api/obligation-categories` (super admin only)
   - `/api/workspaces/:workspace_id/obligations` with query filters (category_id, priority, start_date, end_date, status)
6. Build reminder CRUD: `/api/workspaces/:workspace_id/obligations/:obligation_id/reminders`
7. Implement `nextDate` calculation:
   - For `frequencyInterval=null`: use user-provided date
   - For recurring: calculate `now + (frequencyInterval * frequencyValue)` (weekly=+7 days, monthly=+30 days, yearly=+365 days)
8. Implement `POST /obligations/:obligation_id/create-event` — converts obligation to event, updates `next_due_date` 
9. Build cron job: every 30 minutes, check obligation_reminders where `nextDate - reminderDays <= NOW()` and `status=Active` and workspace is not frozen, send email
10. Workspace freeze logic: if workspace owner leaves (OwnerId becomes null), block all read/create/update/delete operations on that workspace's obligations
11. Frontend: obligations list with filters (category, priority, date range, status), create/edit dialog with category + visibility + priority, detail page with "Create event" CTA
12. Frontend: "Due soon" section showing obligations due within 7 days, respecting visibility rules

### Definition of done

A user can:
1. Create "Car service on Nov 12, 2026" in the Car category with priority High, reminderDays 7, visibleTo family members only
2. Receive a reminder email on Nov 5
3. See it in "Due soon" with a badge
4. Filter obligations by category (Car), priority (High), and date range
5. Click "Create event", pick a date/time, confirm — event appears on calendar
6. Another family member with visibility access can see the obligation
7. Workspace owner can view/edit/delete any obligation, change visibility/priority
8. Super admin can manage obligation categories globally

---

## Iteration 2 — Events & Calendar

Goal: users create concrete time-bound events that appear on a shared workspace calendar. Events can be private or workspace-visible, with optional email reminders.

### Tasks

1. Create `events` table: `id`, `workspace_id`, `user_id`, `title`, `description`, `location`, `start_time`, `end_time`, `is_private`, `assigned_to` (jsonb), `notifications_enabled`, `created_from_obligation_id`
2. Create `event_notifications` table: `id`, `event_id`, `user_id`, `notification_type`, `scheduled_for_datetime`, `sent_at`, `created_from_obligation`
3. Build CRUD endpoints: `POST`, `GET`, `PUT`, `DELETE` on `/api/workspaces/:workspace_id/events`
4. On event create/update: auto-create `event_notifications` rows if `notifications_enabled`
5. Build cron job: every 30 minutes, select pending notifications where `scheduled_for_datetime <= NOW()`, send email via SendGrid, mark `sent_at`
6. Frontend: calendar view (month/week/list), create event dialog, event detail page
7. Frontend: notification bell in app header with dropdown list of upcoming reminders
8. Enforce workspace membership on all reads (events marked `is_private` are filtered to creator only)

### Definition of done

A user can create "Mother's birthday Dec 25, remind 3 days before", see it on the calendar, receive an email on Dec 22, and other workspace members can see the event (unless it's marked private).

---

## After Both Iterations — What Unlocks Next

- Recurring events directly on the calendar (weekly tennis) — requires `rrule.js` integration in obligations
- Multi-person event proposals and acceptance workflow
- Conflict detection across family members
- Push notifications via Firebase (in addition to email)
- SMS reminders via Twilio (in addition to email)
- Gamification (points, leaderboards)
