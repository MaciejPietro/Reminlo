# Obligations

Obligations represent life responsibilities — either one-time deadlines or recurring tasks. They do not appear on the calendar themselves. A user converts an obligation into a concrete event when the time comes. Obligations can exist without reminders, or have multiple reminders with different timing (e.g., 1 week before, 1 day before).

**Examples:**
- One-time: car service on Nov 12, 2026 (remind 1 week before)
- Recurring: annual medical check-up (remind 14 days before)
- Recurring with end date: car insurance renewal every Dec 11 (remind 1 week before) until Dec 18, 2026

---

## Database

```sql
CREATE TABLE obligation_categories (
  id                UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name              VARCHAR(255) NOT NULL UNIQUE,
  created_at        TIMESTAMP DEFAULT NOW(),
  updated_at        TIMESTAMP DEFAULT NOW()
);

CREATE TABLE obligations (
  id                UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  workspace_id      UUID NOT NULL REFERENCES workspaces(id) ON DELETE CASCADE,
  created_by        UUID NOT NULL REFERENCES workspace_members(id) ON DELETE RESTRICT,
  category_id       UUID NOT NULL REFERENCES obligation_categories(id) ON DELETE RESTRICT,
  title             VARCHAR(255) NOT NULL,
  description       TEXT,
  frequency         VARCHAR(50),  -- NULL (=once), weekly, monthly, yearly
  next_due_date     TIMESTAMP NOT NULL,  -- deadline for one-time, next occurrence for recurring
  end_date          TIMESTAMP,  -- optional: when recurring obligation expires
  priority          VARCHAR(50) NOT NULL DEFAULT 'medium',  -- low, medium, high, crucial
  visible_to        UUID[] DEFAULT NULL,  -- NULL=all, []=only creator+owner, [ids]=array of workspace_member IDs
  is_active         BOOLEAN DEFAULT true,
  created_at        TIMESTAMP DEFAULT NOW(),
  updated_at        TIMESTAMP DEFAULT NOW(),
  CHECK (priority IN ('low', 'medium', 'high', 'crucial'))
);

CREATE TABLE obligation_reminders (
  id                UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  obligation_id     UUID NOT NULL REFERENCES obligations(id) ON DELETE CASCADE,
  reminder_days     INT NOT NULL,  -- days before next_due_date
  reminder_type     VARCHAR(50) NOT NULL DEFAULT 'email',
  is_active         BOOLEAN DEFAULT true,
  created_at        TIMESTAMP DEFAULT NOW(),
  updated_at        TIMESTAMP DEFAULT NOW(),
  CHECK (reminder_type IN ('email', 'in_app', 'sms', 'push'))
);
```

### Frequency

**Frequency** determines if an obligation is one-time or recurring:

| Frequency | Meaning             | Example |
|-----------|---------------------|---------|
| NULL      | One-time deadline   | Car service on Nov 12, 2026 |
| `weekly`  | Repeats every week  | Recurring weekly task |
| `monthly` | Repeats every month | Monthly check-in |
| `yearly`  | Repeats every year  | Annual medical check-up |
| `custom`  | Custom pattern      | Reserved for future `rrule` support |

**For one-time obligations (`frequency=NULL`):**
- `next_due_date` is the deadline (user-specified date in the future)
- `end_date` is not used
- Reminders are sent based on days before `next_due_date`

**For recurring obligations:**
- `next_due_date` is calculated: `now + frequency interval` (e.g., now + 1 week, now + 1 month, now + 1 year)
- `end_date` (optional) marks when the obligation should stop recurring
- Reminders recur on the same schedule as the obligation

### Priority

Priority is used for organization and filtering. No functional impact on reminders or notifications.

| Priority | Usage |
|----------|-------|
| `low` | Nice-to-do tasks |
| `medium` | Regular tasks (default) |
| `high` | Important tasks |
| `crucial` | Critical deadlines |

### Visibility (`visible_to`)

Controls who can see an obligation:

| visible_to | Who can see |
|-----------|------------|
| NULL | All workspace members |
| [] (empty array) | Only creator + workspace owner |
| [member_id1, ...] | Creator + workspace owner + listed members |

Creator always sees their own obligations. Workspace owner always sees all obligations.

---

## API Endpoints

### Obligation Categories (Admin only)

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/api/obligation-categories` | List all categories |
| `POST` | `/api/obligation-categories` | Create category (super admin only) |
| `PUT` | `/api/obligation-categories/:id` | Update category (super admin only) |
| `DELETE` | `/api/obligation-categories/:id` | Delete category (super admin only) |

### Obligations

| Method | Path | Description |
|--------|------|-------------|
| `POST` | `/api/workspaces/:workspace_id/obligations` | Create obligation |
| `GET` | `/api/workspaces/:workspace_id/obligations` | List obligations with filters |
| `GET` | `/api/workspaces/:workspace_id/obligations/:id` | Single obligation |
| `PUT` | `/api/workspaces/:workspace_id/obligations/:id` | Update obligation (creator or owner only) |
| `DELETE` | `/api/workspaces/:workspace_id/obligations/:id` | Soft delete (creator or owner only) |
| `POST` | `/api/workspaces/:workspace_id/obligations/:id/create-event` | Convert to event |

**Query Parameters for List:**
```
GET /api/workspaces/:workspace_id/obligations?
  category_id=<uuid>&
  priority=<low|medium|high|crucial>&
  start_date=<ISO-8601>&
  end_date=<ISO-8601>&
  status=<active|due_soon>
```

### Obligation Reminders

| Method | Path | Description |
|--------|------|-------------|
| `POST` | `/api/workspaces/:workspace_id/obligations/:obligation_id/reminders` | Create reminder (creator or owner only) |
| `GET` | `/api/workspaces/:workspace_id/obligations/:obligation_id/reminders` | List reminders for obligation |
| `GET` | `/api/workspaces/:workspace_id/obligations/:obligation_id/reminders/:id` | Single reminder |
| `PUT` | `/api/workspaces/:workspace_id/obligations/:obligation_id/reminders/:id` | Update reminder (creator or owner only) |
| `DELETE` | `/api/workspaces/:workspace_id/obligations/:obligation_id/reminders/:id` | Delete reminder (creator or owner only) |

### Create obligation request body

**One-time obligation:**
```json
{
  "title": "Car service",
  "description": "Oil change and filter replacement",
  "category_id": "550e8400-e29b-41d4-a716-446655440000",
  "priority": "high",
  "next_due_date": "2026-11-12T10:00:00Z",
  "visible_to": null
}
```

**Recurring obligation:**
```json
{
  "title": "Annual car insurance renewal",
  "description": "Renew annual car insurance",
  "category_id": "550e8400-e29b-41d4-a716-446655440000",
  "frequency": "yearly",
  "priority": "crucial",
  "next_due_date": "2025-12-11T00:00:00Z",
  "end_date": "2026-12-18T23:59:59Z",
  "visible_to": null
}
```

**With restricted visibility:**
```json
{
  "title": "Mother's birthday",
  "category_id": "550e8400-e29b-41d4-a716-446655440001",
  "frequency": "yearly",
  "priority": "medium",
  "next_due_date": "2025-06-15T00:00:00Z",
  "visible_to": ["member_id_1", "member_id_2"]
}
```

Notes:
- `frequency` is optional (NULL = one-time obligation)
- `priority` defaults to "medium" if omitted
- `visible_to` is NULL by default (visible to all workspace members)
- Creator is always the authenticated user making the request
- Only creator or workspace owner can update obligations

### Create reminder request body

```json
{
  "reminder_days": 7,
  "reminder_type": "email"
}
```

Multiple reminders can be added to a single obligation. For the examples above:
- Car service (Nov 12): add reminder with `reminder_days=7` (remind Nov 5)
- Car insurance (Dec 11): add reminders `reminder_days=7` (Dec 4) and `reminder_days=1` (Dec 10)

### Convert obligation to event (`/create-event`)

User confirms or adjusts the suggested date and optionally sets event-specific notifications. Backend:
1. Creates an event row (pre-filled from obligation fields)
2. Optionally creates `event_notification` rows if the event has `notifications_enabled = true`
3. Recalculates `obligation.next_due_date = event.start_time + (frequency × interval)`

```json
{
  "event_start_time": "2026-06-15T09:00:00Z",
  "event_end_time": "2026-06-15T10:00:00Z",
  "notifications_enabled": true,
  "is_private": false
}
```

Note: Event notifications are separate from obligation reminders. Obligation reminders are processed by cron before the event is created. Event notifications are attached to specific events.

---

## Reminder Logic

Two types of reminders:

### Obligation Reminders
A cron job (every 30 minutes) checks `obligation_reminders` for rows where `is_active = true AND reminder_type = 'email'`. For each reminder, it calculates:
```
send_at = obligation.next_due_date - reminder_days
```
If `send_at <= NOW()`, it sends the email to the obligation's owner and logs the result.

**For one-time obligations:**
- Reminder is sent once, on the calculated `send_at` date
- After the deadline passes or an event is created, the reminder is no longer triggered

**For recurring obligations:**
- Reminder is sent on each recurrence (e.g., every Dec 11 for yearly obligations)
- If `end_date` is set and has passed, reminders stop being processed
- After converting to an event, `next_due_date` advances, and reminders follow the new schedule

Obligations can have zero or multiple reminders, each independent.

### Event Notifications
A separate cron job (every 30 minutes) checks `event_notifications` for rows where `sent_at IS NULL AND scheduled_for_datetime <= NOW()`. It sends the email, marks `sent_at`, and logs the result. Event notifications are tied to specific events and are independent of obligation reminders.

### "Due soon" banner
The `GET /obligations/due-soon` endpoint is called on dashboard load to surface obligations whose `next_due_date` is within 7 days but no event has been created yet. This is a UI-level prompt, not an automated email, and appears regardless of whether the obligation has reminders configured.

---

## UI Flow

### Creating a One-Time Obligation
1. User opens **Obligations** section
2. Clicks **Create obligation**
3. Fills in:
   - Title (required)
   - Description (optional)
   - **Category** (required, dropdown from global list)
   - **Priority** (optional, defaults to "medium")
   - **Due Date** (required)
   - **Visibility** (optional: visible to all, only me+owner, or specific members)
4. Optionally adds one or more reminders immediately
5. Backend creates obligation and sends initial emails if reminders are configured

Example: "Car service" category=Car, priority=high, due Nov 12, 2026, remind 7 days before

### Creating a Recurring Obligation
1. User opens **Obligations** section
2. Clicks **Create obligation**
3. Fills in same as above plus:
   - **Frequency** (weekly, monthly, yearly)
   - **End Date** (optional, for when recurrence should stop)
4. Backend calculates `next_due_date` from frequency and creates obligation

Example: "Insurance renewal" category=Car, priority=crucial, frequency=yearly, end date Dec 18 2026, remind 7 days before

### Using an Obligation
1. User sees list of all obligations they can see, sorted by `next_due_date`
2. Can filter by:
   - **Category** (e.g., show only "Car" obligations)
   - **Priority** (e.g., show only "crucial")
   - **Date range** (e.g., "next month", "next 3 months")
   - **Status** (e.g., "due soon", "active")
3. Obligations due within 7 days appear in a "Due soon" section with badge
4. Workspace owner sees ALL obligations; other members see only those visible to them
5. User clicks **Create event** on an obligation → dialog opens pre-filled
6. User confirms date, time, and optional event notifications → event created
7. For one-time obligations: obligation stays for reference
8. For recurring obligations: `next_due_date` advances to next cycle

### Managing Obligations
- **Only creator or workspace owner can update/delete** an obligation
- Workspace owner can view and change visibility/priority of any obligation
- When updating: can change title, description, priority, visible_to, frequency, end_date
- Cannot change category after creation (prevents data inconsistency)

### Managing Reminders
1. User can add, update, or delete reminders for their own obligations at any time
2. Can add multiple reminders (e.g., "remind 1 week before AND 1 day before")
3. Deleting a reminder doesn't affect the obligation
4. An obligation without reminders will not send emails but still appears in list and "Due soon" UI
