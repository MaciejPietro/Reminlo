# Plan

## Overview

Two iterations, focused on the most impactful problems first: remembering recurring obligations, then scheduling concrete events on a calendar. Gamification, conflict detection, SMS/push notifications, and AI features are explicitly out of scope.

---

## Iteration 1 — Recurring Pattern Templates

Goal: users define recurring life obligations (insurance renewal, car service, medical tests) and get email reminders before the due date. No calendar entry is created automatically — the user converts a pattern to an event manually when ready.

### Tasks

1. Create `recurring_patterns` table with fields: `id`, `workspace_id`, `user_id`, `title`, `description`, `location`, `frequency`, `reminder_days`, `reminder_type`, `next_due_date`, `last_triggered_at`, `is_active`
2. Define frequency enum: `weekly`, `monthly`, `yearly`, `every_2_years`, `every_3_years`
3. Build CRUD endpoints: `POST`, `GET`, `PUT`, `DELETE` on `/api/workspaces/:workspace_id/patterns`
4. Build `GET /patterns/due-soon` endpoint (next 7 days)
5. Implement `next_due_date` calculation on create based on frequency
6. Implement `POST /patterns/:pattern_id/create-event` — converts pattern to an event, updates `next_due_date` and `last_triggered_at`
7. Frontend: patterns list page, create/edit dialog, detail page with "Create event" CTA
8. Frontend: "Due soon" tab on patterns list (badge on navigation)

### Definition of done

A user can create "Car service every 2 years, remind 14 days before", come back 2 years later, see a reminder in their inbox and a "Due soon" badge, click "Create event", pick a date/time, confirm — and the event appears on the calendar.

---

## Iteration 2 — Events & Calendar

Goal: users create concrete time-bound events that appear on a shared workspace calendar. Events can be private or workspace-visible, with optional email reminders.

### Tasks

1. Create `events` table: `id`, `workspace_id`, `user_id`, `title`, `description`, `location`, `start_time`, `end_time`, `is_private`, `assigned_to` (jsonb), `notifications_enabled`, `created_from_pattern_id`
2. Create `event_notifications` table: `id`, `event_id`, `user_id`, `notification_type`, `scheduled_for_datetime`, `sent_at`, `created_from_pattern`
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

- Recurring events directly on the calendar (weekly tennis) — requires `rrule.js` integration
- Multi-person event proposals and acceptance workflow
- Conflict detection across family members
- Push notifications via Firebase
- SMS reminders via Twilio
- Gamification (points, leaderboards)
