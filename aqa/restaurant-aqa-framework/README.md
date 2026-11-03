# Restaurant AQA framework

End-to-end tests for the Green & Tasty restaurant application: Reqnroll (Gherkin) + TUnit + Playwright for .NET, with the Screenplay pattern.

## Homework: direction and approach

- **Direction 1 — Group pre-order**, tested against my own deployed backend and frontend (homeworks 4–6).
- **Approach: BDD + Screenplay**, on this framework as it is. The business rules read as Gherkin, the steps stay thin, and every action is a task or question shared between the UI and the API stage.
- **Test pyramid.** The main scenarios run on screen (`@ui`): what the owner sees. Authorisation, validation and test-data set-up go through the API (`@api`).
- **Levels are tags:**
  - `@L1` covers viewing the participants (`Features/GroupPreOrderParticipants.feature`).
  - `@L2` covers submitting the consolidated pre-order (`Features/GroupPreOrderSubmission.feature`).
  - `@L3` (removing a participant) comes next.

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

Guidance for extending the suite (writing Gherkin, steps, Screenplay, test data, working with an AI assistant) is in [CLAUDE.md](CLAUDE.md).

## Run the tests

```bash
cp .env.example .env        # then fill in your own values (see "Configuration")
dotnet build
dotnet run --no-build -- --report-trx                          # everything
dotnet run --no-build -- --treenode-filter "/*/*/*/*[Category=smoke]"   # by tag
```

Everything a run produces lands in `TestResults/`: the TUnit report and TRX, the Reqnroll report in `reqnroll/`, and Playwright traces in `traces/` (open one with `npx playwright show-trace <file>.zip`).

Some scenarios are expected to fail (`@known-defect`, bugs in the application) or are skipped (`@wip`, no admin account configured). That is intentional.

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
| `WAITER_USERNAME`, `WAITER_PASSWORD` | a waiter account that already exists in the environment under test |
| `TESTTABLE_ID`, `TESTTABLE_WAITER_*` | a table and the waiter assigned to it (only for scenarios that need them) |
| `ADMIN_USERNAME`, `ADMIN_PASSWORD` | an admin account; without it the admin scenarios are skipped |

The full list, with defaults, is in `.env.example`.

## Testing your own deployment

### If you deployed the backend / frontend / DevOps homework

1. `cp .env.example .env` and set `UI_URL` and `API_URL` to your deployment (API URL includes the stage, for example `…/develop`).
2. Waiters cannot be created by signing up, so create a waiter (and, for the admin scenarios, an admin) in *your* environment and put its credentials in `WAITER_*` / `ADMIN_*`. Optionally add a table and its assigned waiter (`TESTTABLE_*`).
3. Run `dotnet build` and a small slice first (`--treenode-filter "/*/*/*/*[Category=smoke]"`).

### If you have not deployed your own homework

If you deployed the original application, test that: set `UI_URL` / `API_URL` as above. Otherwise point the framework at a comparable public site (Booking, Airbnb, OpenTable, …) and write all the layers for it yourself: Gherkin features, step definitions, Screenplay tasks and questions, page objects with locators, and API clients where the site has a usable API. Use only flows you are allowed to automate (no purchases, no real bookings, respect the site's terms), and set `UI_URL` to that site.

1. **Generate a skeleton with Playwright codegen** (see "Presetup instructions"): record one flow, for example search → open a result → check details. Save the generated code to a scratch file, it is only raw material.
2. **Split it across the layers**: locators and small actions go to `Ui/` page objects, the flow becomes a task in `Screenplay/Tasks/`, the read-backs become questions in `Screenplay/Questions/`, the business wording goes to `Features/` and thin glue to `StepDefinitions/`.
3. **Replace weak locators.** Codegen often emits positional or text-fragile selectors. Prefer role + accessible name, label, test id, then CSS (see [CLAUDE.md](CLAUDE.md)); check them in the Playwright inspector.
4. Tag scenarios `@ui` / `@api`, keep them independent, and run `npx gherkin-lint -c .gherkin-lintrc "Features/*.feature"`.

### CI on your own public repository

Publish your work in your own public Git repository (the link is the deliverable). Workflows are disabled in a fresh fork until you enable them. Add the same values under Settings → Secrets and variables → Actions:

- **Variables:** `AQA_UI_URL`, `AQA_API_URL`, `AQA_TESTTABLE_ID`
- **Secrets:** `AQA_WAITER_USERNAME`, `AQA_WAITER_PASSWORD`, `AQA_ADMIN_USERNAME`, `AQA_ADMIN_PASSWORD`, `AQA_TESTTABLE_WAITER_USERNAME`, `AQA_TESTTABLE_WAITER_PASSWORD`

`aqa-quality.yml` runs on every push and pull request; `aqa-tests.yml` (nightly and on demand) tests whatever those variables point at. Never put credentials in the repository, in `.env.example` or in test code.

## Presetup instructions

This framework contains PlaywriteInstaller script so you don't need to setup it manually.
For full BDD highlighting in your IDE you may need to install Reqnroll extension https://docs.reqnroll.net/latest/ide-integrations/index.html
To start the Playwright code generator open Powershell in the framework directory and enter the line:
```bash
bin/Debug/net10.0/playwright.ps1 codegen "<your UI_URL or the public site you test>"
```
Build first so `playwright.ps1` exists. The URL is optional.
