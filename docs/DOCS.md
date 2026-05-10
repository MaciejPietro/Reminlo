# System Documentation

## Architecture Overview

The system is a workspace-based family coordination tool. Users register, create a workspace, and invite family members. Within a workspace, users can define recurring obligation templates and schedule concrete events on a shared calendar.

---

## Core Concepts

### Workspace
A shared space for a family or group. All recurring patterns and events belong to a workspace. Only workspace members can access its data. A user can belong to multiple workspaces.

### Obligation
A life obligation that can be one-time (e.g., "car service on Nov 12") or recurring (e.g., "annual medical check-up"). Belongs to a category (e.g., "Car", "Health", "Family"). Can have visibility restrictions. Obligations do not appear on the calendar. They exist as templates for creating real events when the time comes. An obligation can exist without reminders.

**Fields:** id, workspace_id, created_by (WorkspaceMember), category_id, name, description, frequencyInterval (null/Daily/Weekly/Monthly/Yearly), frequencyValue (nullable int, used with frequencyInterval), priority (Low/Medium/High/Crucial), nextDate, expirationDate, visibleTo (collection of WorkspaceMember), status (Active/Inactive)

### Obligation Category
A global taxonomy for organizing obligations. Examples: Home, Health, Car, Family, Birthday, Anniversary. Only super admins can create/edit/delete categories.

**Fields:** id, name (unique), created_at, updated_at

### Obligation Reminder
A scheduled reminder tied to a specific obligation. Multiple reminders can be attached to one obligation, each with different timing. Reminders are optional — an obligation can exist without any reminders.

**Fields:** id, obligation_id, reminderDays, notificationType, status (Active/Inactive)

### Event
A concrete, time-bound item on the calendar. Created either from scratch or by converting a recurring pattern. Events have a start time, end time, privacy setting, assignees, and optional email reminders.

**Fields:** title, location, start/end time, is_private, assigned_to, notifications_enabled, created_from_pattern_id

### Event Notification
A scheduled email reminder tied to a specific event. Created automatically when an event is saved with `notifications_enabled = true`. Processed by a background cron job.

---

## Data Flow

### Iteration 1 — Recurring Obligations

```
User creates obligation
  → Backend calculates nextDate from frequencyInterval + frequencyValue
  → Obligation stored with status=Active
  → User can optionally add one or more reminders

Cron / dashboard load
  → App shows "Due soon" badge for obligations where nextDate ≤ now + 7 days
  → Reminders are processed: send emails to users with reminders configured

User clicks "Create event"
  → Dialog opens pre-filled from obligation
  → User confirms date/time
  → Backend creates Event + EventNotification rows
  → Obligation.nextDate advances by frequencyInterval + frequencyValue
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
/workspaces/:workspace_id/obligations              CRUD for obligations
/workspaces/:workspace_id/obligations/due-soon     Due within 7 days
/workspaces/:workspace_id/obligations/:id/reminders  CRUD for reminders on an obligation
/workspaces/:workspace_id/obligations/:id/create-event  Convert to event

/workspaces/:workspace_id/events         CRUD for events
/workspaces/:workspace_id/notifications  List pending notifications for current user
```

---

## Notification System

| Layer | Implementation |
|-------|----------------|
| Email delivery | SendGrid (transactional) |
| Obligation reminders | `obligation_reminders` table with `reminder_days` before next_due_date |
| Event notifications | `event_notifications` table with `scheduled_for_datetime` |
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
