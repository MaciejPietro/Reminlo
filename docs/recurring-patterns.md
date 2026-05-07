# Recurring Patterns

Recurring patterns are templates representing life obligations that repeat on a schedule. They do not appear on the calendar themselves. A user converts a pattern into a concrete event when the time comes.

**Examples:** car service every 2 years, annual medical check-up, home boiler service, insurance renewal, pet vaccination.

---

## Database

```sql
CREATE TABLE recurring_patterns (
  id                UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  workspace_id      UUID NOT NULL REFERENCES workspaces(id) ON DELETE CASCADE,
  user_id           UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
  title             VARCHAR(255) NOT NULL,
  description       TEXT,
  location          VARCHAR(255),
  frequency         VARCHAR(50) NOT NULL,  -- see Frequency below
  reminder_days     INT NOT NULL DEFAULT 7,
  reminder_type     VARCHAR(50) DEFAULT 'email',
  next_due_date     TIMESTAMP NOT NULL,
  last_triggered_at TIMESTAMP,
  is_active         BOOLEAN DEFAULT true,
  created_at        TIMESTAMP DEFAULT NOW(),
  updated_at        TIMESTAMP DEFAULT NOW()
);
```

### Frequency values

| Value | Meaning |
|-------|---------|
| `weekly` | Every 7 days |
| `monthly` | Every 1 month |
| `yearly` | Every 12 months |
| `every_2_years` | Every 24 months |
| `every_3_years` | Every 36 months |

`next_due_date` is computed on create: `now + frequency interval`. It is updated every time the user converts this pattern into an event.

---

## API Endpoints

| Method | Path | Description |
|--------|------|-------------|
| `POST` | `/api/workspaces/:workspace_id/patterns` | Create pattern |
| `GET` | `/api/workspaces/:workspace_id/patterns` | List all patterns |
| `GET` | `/api/workspaces/:workspace_id/patterns/due-soon` | Due in next 7 days |
| `GET` | `/api/workspaces/:workspace_id/patterns/:id` | Single pattern |
| `PUT` | `/api/workspaces/:workspace_id/patterns/:id` | Update pattern |
| `DELETE` | `/api/workspaces/:workspace_id/patterns/:id` | Soft delete |
| `POST` | `/api/workspaces/:workspace_id/patterns/:id/create-event` | Convert to event |

### Create pattern request body

```json
{
  "title": "Car service",
  "description": "Oil change and filter replacement",
  "location": "Mechanic XYZ",
  "frequency": "every_2_years",
  "reminder_days": 14,
  "reminder_type": "email"
}
```

`next_due_date` is calculated server-side from `frequency`.

### Convert pattern to event (`/create-event`)

User confirms or adjusts the suggested date. Backend:
1. Creates an event row (pre-filled from pattern fields)
2. Creates `event_notification` rows based on `pattern.reminder_days`
3. Updates `pattern.last_triggered_at = NOW()`
4. Recalculates `pattern.next_due_date = event.start_time + frequency interval`

```json
{
  "event_start_time": "2026-06-15T09:00:00Z",
  "event_end_time": "2026-06-15T10:00:00Z",
  "notifications_enabled": true,
  "is_private": false
}
```

---

## Reminder Logic

A cron job (every 30 minutes) checks `event_notifications` for rows where `sent_at IS NULL AND scheduled_for_datetime <= NOW()`. It sends the email, marks `sent_at`, and logs the result. Patterns themselves do not trigger emails directly — only the events converted from them do.

**Exception — "due soon" banner:** The `GET /patterns/due-soon` endpoint is called on dashboard load to surface patterns whose `next_due_date` is within 7 days but no event has been created yet. This is a UI-level prompt, not an automated email.

---

## UI Flow

1. User opens **Reminders** section
2. Sees list of all active patterns sorted by `next_due_date`
3. Patterns due within 7 days appear in a "Due soon" tab with a badge
4. User clicks **Create event** on a pattern → dialog opens pre-filled
5. User confirms date, time, and notification preference → event created
6. Pattern `next_due_date` advances to next cycle
