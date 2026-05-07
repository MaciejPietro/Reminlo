# Events

Events are concrete, time-bound items that appear on the workspace calendar. They can be created from scratch or converted from a recurring pattern template.

---

## Database

```sql
CREATE TABLE events (
  id                      UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  workspace_id            UUID NOT NULL REFERENCES workspaces(id) ON DELETE CASCADE,
  user_id                 UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
  title                   VARCHAR(255) NOT NULL,
  description             TEXT,
  location                VARCHAR(255),
  start_time              TIMESTAMP NOT NULL,
  end_time                TIMESTAMP NOT NULL,
  is_private              BOOLEAN DEFAULT false,
  assigned_to             JSONB DEFAULT '[]',  -- array of user_id strings
  notifications_enabled   BOOLEAN DEFAULT true,
  created_from_pattern_id UUID REFERENCES recurring_patterns(id) ON DELETE SET NULL,
  created_at              TIMESTAMP DEFAULT NOW(),
  updated_at              TIMESTAMP DEFAULT NOW()
);

CREATE TABLE event_notifications (
  id                      UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  event_id                UUID NOT NULL REFERENCES events(id) ON DELETE CASCADE,
  user_id                 UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
  notification_type       VARCHAR(50) DEFAULT 'email',
  scheduled_for_datetime  TIMESTAMP NOT NULL,
  sent_at                 TIMESTAMP,
  created_from_pattern    BOOLEAN DEFAULT false,
  created_at              TIMESTAMP DEFAULT NOW()
);
```

### Key indexes

```sql
CREATE INDEX ON events(workspace_id);
CREATE INDEX ON events(start_time);
CREATE INDEX ON event_notifications(scheduled_for_datetime) WHERE sent_at IS NULL;
```

---

## API Endpoints

| Method | Path | Description |
|--------|------|-------------|
| `POST` | `/api/workspaces/:workspace_id/events` | Create event |
| `GET` | `/api/workspaces/:workspace_id/events` | List events |
| `GET` | `/api/workspaces/:workspace_id/events/:id` | Single event |
| `PUT` | `/api/workspaces/:workspace_id/events/:id` | Update event |
| `DELETE` | `/api/workspaces/:workspace_id/events/:id` | Delete event |

Query params for list: `?start_date=YYYY-MM-DD&end_date=YYYY-MM-DD`

### Create event request body

```json
{
  "title": "Mother's birthday",
  "description": "Birthday dinner",
  "location": "Home",
  "start_time": "2025-12-25T18:00:00Z",
  "end_time": "2025-12-25T22:00:00Z",
  "assigned_to": ["user_id_1", "user_id_2"],
  "is_private": false,
  "notifications_enabled": true,
  "reminder_days_before": 3
}
```

`assigned_to` is optional. Empty array means the event is visible to all workspace members (subject to `is_private`). When populated, only those members receive notifications.

---

## Privacy Model

| `is_private` | `assigned_to` | Visibility |
|---|---|---|
| `false` | `[]` | All workspace members see it on the calendar |
| `false` | `["id1", "id2"]` | All workspace members see it; only id1 and id2 get notifications |
| `true` | anything | Only the creator sees it |

The backend enforces this at the query level — private events are filtered from workspace list responses unless `event.user_id = current_user.id`.

---

## Notification Scheduling

When an event is created or updated:

1. If `notifications_enabled = false` → skip, no rows created
2. If `notifications_enabled = true` and `reminder_days_before` is set:
   - `scheduled_for_datetime = start_time - (reminder_days_before * 24 hours)`
   - Create one `event_notifications` row per recipient
   - Recipients = `assigned_to` members, or all workspace members if empty (excluding private events)
3. If `start_time` changes on update → recalculate and update pending (unsent) `event_notifications` rows

### Cron job

Runs every 30 minutes:

```sql
SELECT en.*, e.title, u.email
FROM event_notifications en
JOIN events e ON en.event_id = e.id
JOIN users u ON en.user_id = u.id
WHERE en.sent_at IS NULL
  AND en.scheduled_for_datetime <= NOW();
```

For each row: send email via SendGrid → set `sent_at = NOW()`.

### Reminder options exposed to users

`0` (day of), `1`, `3`, `7`, `14`, `30` days before. Stored as integer in request, converted to datetime on the backend.

---

## Calendar View

- Default: month view
- Also: week view, list view (sorted by `start_time ASC`)
- Events shown with creator name and color-coded by visibility (workspace vs private)
- Clicking an empty date on the calendar opens the create event dialog pre-filled with that date
- Events created from a pattern show a small badge linking back to the pattern
