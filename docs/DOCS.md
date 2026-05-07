# System Documentation

## Architecture Overview

The system is a workspace-based family coordination tool. Users register, create a workspace, and invite family members. Within a workspace, users can define recurring obligation templates and schedule concrete events on a shared calendar.

---

## Core Concepts

### Workspace
A shared space for a family or group. All recurring patterns and events belong to a workspace. Only workspace members can access its data. A user can belong to multiple workspaces.

### Recurring Pattern
A template representing an obligation that repeats on a schedule — insurance renewal, car service, medical tests. Patterns do not appear on the calendar. They exist to remind users when something is due and provide a one-click way to create a real event when the time comes.

**Fields:** title, description, location, frequency, reminder_days, next_due_date, last_triggered_at

### Event
A concrete, time-bound item on the calendar. Created either from scratch or by converting a recurring pattern. Events have a start time, end time, privacy setting, assignees, and optional email reminders.

**Fields:** title, location, start/end time, is_private, assigned_to, notifications_enabled, created_from_pattern_id

### Event Notification
A scheduled email reminder tied to a specific event. Created automatically when an event is saved with `notifications_enabled = true`. Processed by a background cron job.

---

## Data Flow

### Iteration 1 — Recurring Patterns

```
User creates pattern
  → Backend calculates next_due_date from frequency
  → Pattern stored, dormant

Cron / dashboard load
  → App shows "Due soon" badge for patterns where next_due_date ≤ now + 7 days

User clicks "Create event"
  → Dialog opens pre-filled from pattern
  → User confirms date/time
  → Backend creates Event + EventNotification rows
  → Pattern.next_due_date advances by frequency
  → Pattern.last_triggered_at = NOW()
```

### Iteration 2 — Events

```
User creates event
  → If notifications_enabled: create EventNotification per recipient
  → scheduled_for_datetime = start_time - reminder_days * 24h

Cron job (every 30 min)
  → SELECT event_notifications WHERE sent_at IS NULL AND scheduled_for <= NOW()
  → Send email via SendGrid
  → Mark sent_at = NOW()

User opens calendar
  → GET /events?start_date=&end_date=
  → Backend filters private events (only creator sees own private events)
```

---

## API Structure

All routes are prefixed with `/api` and require authentication.

```
/workspaces/:workspace_id/patterns       CRUD for recurring patterns
/workspaces/:workspace_id/patterns/due-soon     Due within 7 days
/workspaces/:workspace_id/patterns/:id/create-event  Convert to event

/workspaces/:workspace_id/events         CRUD for events
/workspaces/:workspace_id/notifications  List pending notifications for current user
```

---

## Notification System

| Layer | Implementation |
|-------|----------------|
| Email delivery | SendGrid (transactional) |
| Scheduling | `event_notifications` table with `scheduled_for_datetime` |
| Processing | Cron job every 30 minutes |
| Status tracking | `sent_at` timestamp (null = pending) |

Future channels (stubbed): in-app, SMS (Twilio), push (Firebase).

---

## Privacy Rules

| Scenario | Who sees the event |
|---|---|
| `is_private = false`, `assigned_to = []` | All workspace members |
| `is_private = false`, `assigned_to = [ids]` | All workspace members see event; only `assigned_to` receive notifications |
| `is_private = true` | Only the creator |

---

## Tech Stack Decisions

- **Database:** PostgreSQL — JSONB for `assigned_to`, UUID primary keys throughout
- **Email:** SendGrid — free tier sufficient for personal/family use
- **Cron:** Any scheduler (node-cron, pg_cron, system cron) — job is idempotent
- **Timezones:** All timestamps stored in UTC; display converted on client
- **Recurrence:** Simple enum for now; `rrule.js` added when recurring events land
