# Restaurant AQA framework

End-to-end tests for the Green & Tasty restaurant application: Reqnroll (Gherkin) + TUnit + Playwright for .NET, with the Screenplay pattern.

## Homework: direction and approach

- **Direction 1 — Group pre-order**, tested against my own deployed backend and frontend (homeworks 4–6).
- **Approach: BDD + Screenplay**, on this framework as it is. The business rules read as Gherkin, the steps stay thin, and every action is a task or question shared between the UI and the API stage.
- **Test pyramid.** The main scenarios run on screen (`@ui`): what the owner sees. Authorisation, validation and test-data set-up go through the API (`@api`).
- **Levels are tags:**
  - `@L1` covers viewing the participants (`Features/GroupPreOrderParticipants.feature`).
  - `@L2` covers submitting the consolidated pre-order (`Features/GroupPreOrderSubmission.feature`).
  - `@L3` covers removing a guest, plus the end-to-end scenario (`Features/GroupPreOrderGuestRemoval.feature`). My frontend has the Remove Guest button (the bonus of the frontend homework), so L3 runs on screen as well as through the API.
- **Other tags:** `@smoke` marks the main scenario of each level, `@e2e` the end-to-end one, `@negative` the refusals.

```bash
dotnet run --no-build -- --treenode-filter "/*/*/*/*[Category=L1]"   # one level
dotnet run --no-build -- --treenode-filter "/*/*/*/*[Category=api]"  # one stage
```

### Test data

Every scenario sets up its own restaurant: a location, a table, a menu with fixed prices and **a waiter of its own**. It also books its own reservation. So `WAITER_*` does not have to be configured for these scenarios:

- The waiter is a freshly registered customer, made a waiter of that location (`POST /waiters` needs no authorisation).
- That waiter is the location's only waiter, so the booking is always assigned to them.
- When the scenario ends, the reservation is cancelled, the dishes, the table and the location are deleted, and the waiter is removed from the waiters list.
- Accounts cannot be deleted through the API. Each one therefore has a unique `aqa.sample.<guid>@example.com` email and an `AQA-Sample-` first name.

### Behaviour not defined by the homework (as my backend implements it)

| Case | Outcome |
|---|---|
| Not signed in, or a guest (a guest has a session token, not an account) | 401 |
| Signed in, but not the owner of the group (another customer, the waiter) | 403 "Only the reservation owner can perform this action." |
| Group that does not exist | 404 |
| Group nobody has joined yet | `participants: []`, `total: 0`. Once the owner opens their screen, the owner has a session too and is listed with no dishes. |
| Grand total | Through the service, `total` is the sum of every participant. On the owner's screen, Grand Total is the consolidated order, so only submitted carts count (as the frontend homework specifies). |
| Submitting: no guest has submitted a cart | 400 "There are no submitted guest sessions." |
| Submitting: the owner has no pre-order of their own | 400 "Order for reservation &lt;id&gt; was not found.". The owner's pre-order is the order the restaurant receives, so the owner has to pre-order something first. On screen the button is still enabled, and the same message comes back as a toast. |
| Submitting: group or booking does not exist | 404 "Group reservation was not found" / "Reservation &lt;id&gt; was not found." |
| Submitting a second time | 400 "Group reservation was submitted." On screen the button disappears once the pre-order is sent. |
| Submitting: not the owner / not signed in | 403 "Only the reservation owner can perform this action." / 401. The group stays open. |
| Submitting: the submitted carts hold no dishes | Not testable from outside. An empty cart cannot be submitted, and a submitted cart cannot be changed, so this state cannot be set up through the API or the UI. |
| Removing a guest | 200 with the updated `participants` and `total`, so the removed guest's cart no longer counts. A guest who has already submitted can be removed too. |
| Removing the last guest | 200, `participants: []` and `total: 0` (the owner is still listed if their session exists). On screen: "Participants haven't added any dishes yet". |
| Removing a guest of another group | 400 "Guest session does not belong to this group reservation." |
| Removing a guest who does not exist | 404 "Guest session with id &lt;id&gt; not found." |
| Removing: not the owner / not signed in | 403 "Only the reservation owner can perform this action." / 401. The guest stays. |

Guidance for extending the suite (writing Gherkin, steps, Screenplay, test data, working with an AI assistant) is in [CLAUDE.md](CLAUDE.md).

## Run the tests

```bash
cp .env.example .env        # then fill in your own values (see "Configuration")
dotnet build
dotnet run --no-build -- --report-trx                          # everything
dotnet run --no-build -- --treenode-filter "/*/*/*/*[Category=smoke]"   # by tag
```

Everything a run produces lands in `TestResults/`: the TUnit report and TRX, the Reqnroll report in `reqnroll/`, and Playwright traces in `traces/` (open one with `npx playwright show-trace <file>.zip`).

## Quality gates

| Where | What runs |
|---|---|
| Git hooks (local) | `pre-commit`: `dotnet format` check and Gherkin lint on staged files · `commit-msg`: Conventional Commits · `pre-push`: build with analyzers |
| `aqa-quality.yml` | on every push and pull request: no generated files tracked, format, build (warnings are errors), Gherkin lint, test discovery |
| `aqa-tests.yml` | the scenarios themselves, nightly and on demand (Actions → AQA Tests → Run workflow) |

Enable the hooks once per clone, from the repository root:

```bash
dotnet tool restore --tool-manifest aqa/restaurant-aqa-framework/.config/dotnet-tools.json
git config core.hooksPath aqa/restaurant-aqa-framework/.husky
```

The hooks only act on commits and pushes that touch `aqa/restaurant-aqa-framework`. Gherkin lint needs Node.js.

## Configuration

There is no shared environment and no committed `.env`: copy `.env.example` to `.env` and fill it in with the data of **your own** deployment (see "Testing your own deployment"). `.env` is git-ignored; never commit it.

Settings come from `.env` or real environment variables (variables win):

| Setting | Meaning |
|---|---|
| `UI_URL` | where the frontend is served |
| `API_URL` | base URL of the backend API, including the stage (for example `…/develop`) |
| `DEFAULT_TIMEOUT_MS`, `NAVIGATION_TIMEOUT_MS` | how long to wait for an action / a page; raise the first to `30000` against a backend that starts cold |
| `WAITER_USERNAME`, `WAITER_PASSWORD` | optional: a standing waiter account, used only by the sweep that removes leftover `AQA-Sample-` data before and after a run. The scenarios hire a waiter of their own and do not need it |

The full list, with defaults, is in `.env.example`.

## Testing your own deployment

1. `cp .env.example .env` and set `UI_URL` and `API_URL` to your deployment (the API URL includes the stage, for example `…/develop`).
2. Run `dotnet build` and a small slice first (`--treenode-filter "/*/*/*/*[Category=smoke]"`).

No accounts have to be prepared: every scenario registers the customers it needs and hires its own waiter (see "Test data" above).

### CI on your own public repository

Publish your work in your own public Git repository (the link is the deliverable). Workflows are disabled in a fresh fork until you enable them. Add the values under Settings → Secrets and variables → Actions:

- **Variables:** `AQA_UI_URL`, `AQA_API_URL`
- **Secrets (optional, for the leftover sweep only):** `AQA_WAITER_USERNAME`, `AQA_WAITER_PASSWORD`

`aqa-quality.yml` runs on every push and pull request; `aqa-tests.yml` (nightly and on demand) tests whatever those variables point at. Never put credentials in the repository, in `.env.example` or in test code.

## Presetup instructions

This framework contains PlaywriteInstaller script so you don't need to setup it manually.
For full BDD highlighting in your IDE you may need to install Reqnroll extension https://docs.reqnroll.net/latest/ide-integrations/index.html
To start the Playwright code generator open Powershell in the framework directory and enter the line:
```bash
bin/Debug/net10.0/playwright.ps1 codegen "<your UI_URL or the public site you test>"
```
Build first so `playwright.ps1` exists. The URL is optional.
