# Requirements Analysis — US11 Guest Viewing Reservation Menu

## User Story under review

| Field | Value |
|---|---|
| **Title** | [Draft] US11_Guest Viewing Reservation Menu |
| **Epic** | Order Management |
| **US name** | Guest Reservation and Menu View |
| **Role** | Guest |
| **Description** | As a Guest, I want to see the full restaurant menu, so that I know what options are available and can decide what to choose. |

**Pre-conditions**
- Guest has successfully joined the session (US_G10).
- The parent reservation or group session status is in Created/"Reserved".

**Acceptance Criteria**
1. The Guest can view the full restaurant menu structured logically by category (e.g., Appetizers, Main Course, Desserts).
2. Each menu item must clearly display name, price, serving size in grams, image and current availability status.
3. The Guest can add an available dish directly to their personal subcart by interacting with a clear action button (e.g., "Pre-order").
4. Items currently marked "On Stop" are clearly indicated as unavailable and cannot be selected or added to the cart.
5. The menu reflects the current data at the time the Guest opens or refreshes the screen.

---

## Found issues

Severity: **High** means the story can't be built or tested without the answer, **Medium** means testers and developers will read it differently, and **Low** is a wording or consistency problem.

### A. Story header and description

| # | Where | Issue | Severity | Question / suggestion |
|---|---|---|---|---|
| A1 | Title | The story is still marked **[Draft]**, so it isn't ready for development or testing (it fails Definition of Ready). | Medium | Finish the review and remove the Draft marker before the story goes into a sprint. |
| A2 | Title vs US name | The title says "Guest **Viewing Reservation Menu**" but the US name is "Guest **Reservation and Menu View**". The story doesn't cover viewing the reservation at all. | Low | Use one name, e.g. "Guest Views Menu for Group Pre-order". |
| A3 | ID | The story is "US11", but the precondition refers to "US_G10". The two IDs follow different naming schemes. | Low | Use one ID format and link the dependency explicitly. |
| A4 | Role | "Guest" isn't defined. It could mean a signed-in customer who joined through an invite, an anonymous visitor with an invite link, or any unregistered visitor of the site. | High | Define the role, e.g. "Group guest: a person who joined a group reservation through the invite link (no account required)". |
| A5 | Description | "Full restaurant menu" doesn't say **which** restaurant. The chain has 3 locations, and the menu should be the one for the reservation's location. | High | "...the menu of the location where the group reservation is made". |
| A6 | Description vs AC3 | The description only covers **viewing** the menu, but AC3 adds dishes to a cart. Ordering is a separate capability, so the story is larger than its description (scope creep, the story is not INVEST-"Small"). | Medium | Either extend the description ("...and pre-order dishes") or move AC3 into its own "Guest adds dish to subcart" story. |
| A7 | Epic | Epic is "Order Management", while viewing the menu belongs to "Menu browsing". This only holds together if AC3 stays. | Low | Agree on the epic after A6 is resolved. |

### B. Pre-conditions

| # | Issue | Severity | Question / suggestion |
|---|---|---|---|
| B1 | "Session" is ambiguous: guest session, group session or browser session? | Medium | Name it: "Guest has an active guest session in the group (US_G10)". |
| B2 | "Created/"Reserved"" — two status names for one state. It's unclear whether these are two allowed statuses or two names for the same one. | Medium | List the exact status values used by the system. |
| B3 | Behaviour for **other** statuses isn't described: the group pre-order was already submitted, the reservation was cancelled, it has expired or finished, or the owner removed the guest. | High | Add negative ACs: what does the guest see, can they still browse, is the "Pre-order" button hidden or disabled, and which message is shown? |
| B4 | It doesn't say whether the guest's own cart must still be open. If the guest has already submitted their cart, can they add more dishes? | High | Clarify the cart states (draft, submitted) and how the menu behaves in each. |

### C. Acceptance criteria

| # | AC | Issue | Severity | Question / suggestion |
|---|---|---|---|---|
| C1 | AC1 | "Structured **logically**" is subjective and can't be tested. | Medium | Name the order of categories and the order of dishes within a category (alphabetical, by popularity, set by admin). |
| C2 | AC1 | Categories are given only as "e.g.", so the real list is unknown. | Medium | Give the full list, or say the list comes from the back-office. |
| C3 | AC1 | Not specified: how the guest moves between categories (tabs, anchors, filter, scroll), whether empty categories are shown, and whether one dish can be in several categories. | Medium | Add a navigation rule and say whether empty categories are hidden. |
| C4 | AC2 | "Clearly display" is subjective. | Low | Refer to the Figma layout as the source of truth. |
| C5 | AC2 | Serving size "in grams" doesn't fit drinks and soups (ml), or portions counted in pieces. | Medium | Allow g / ml / pcs, or confirm that only grams are used. |
| C6 | AC2 | Price format isn't defined: currency, decimals, discounts. | Medium | E.g. "$12.50", 2 decimals, currency from location settings. |
| C7 | AC2 | Missing image: there is no fallback (placeholder) defined. | Low | Add "if a dish has no image, a placeholder is shown". |
| C8 | AC2 | "Current availability status" — the list of statuses isn't given. AC4 only mentions "On Stop". | Medium | List all statuses (Available, On Stop, ...?) and how each is shown. |
| C9 | AC2 | Long dish names have no truncation rule. Description, allergens, calories and a dish details view aren't mentioned, so it's unclear whether they're out of scope. | Low | Say what's out of scope explicitly. |
| C10 | AC3 | Terms are inconsistent: "personal **subcart**" in AC3 and "**cart**" in AC4. | Low | Use one term across the story. |
| C11 | AC3 | Quantity isn't covered: does a second click add +1 or do nothing? Is there a "−" control or a maximum quantity? | High | Define the quantity behaviour and limits. |
| C12 | AC3 | There's no feedback after adding a dish (toast, cart counter, button state), so "added" can't be verified from the UI. | Medium | E.g. "the cart counter increases and the button changes to a quantity stepper". |
| C13 | AC3 | Errors when adding aren't covered: network failure, a dish that went On Stop in the meantime, or a group that was closed. | High | Add negative ACs with the error messages. |
| C14 | AC3 | The button label is only an example ("e.g., 'Pre-order'"). | Low | Fix the label, or refer to Figma. |
| C15 | AC4 | "Clearly indicated" — the visual treatment isn't specified (greyed out, label, disabled button). | Medium | Refer to Figma, e.g. "badge 'On Stop', button disabled". |
| C16 | AC4 | It's unclear whether On Stop dishes are **shown** (disabled) or **hidden**. "Cannot be selected" could also mean the dish card can't be opened at all. | Medium | Clarify: shown but not orderable. |
| C17 | AC4 | A dish already in the cart can go On Stop later. What happens then: removed, marked, or does the submit fail? | High | Add an AC for this case, or link the story that covers it. |
| C18 | AC5 | "Refreshes the screen" is unclear: a browser reload, pull-to-refresh, or a refresh button? | Medium | Name the mechanism. |
| C19 | AC5 | It suggests there are **no** live updates. If a dish goes On Stop while the guest is on the page, they can still try to add it, and that must be handled (see C13). | Medium | Confirm there's no real-time update and that the server re-checks availability when adding. |
| C20 | AC5 | Prices can change between viewing and submitting, and it's not said which price wins. | Low | Clarify, or move it to the submit story. |

### D. Missing non-functional and UI-state requirements

| # | Issue | Severity |
|---|---|---|
| D1 | No loading, empty (location has no dishes) or error (menu failed to load) states. | Medium |
| D2 | No responsive or mobile requirement. Guests usually open an invite link on a phone. | Medium |
| D3 | No performance requirement (menu load time, image lazy-loading, pagination for a large menu). | Low |
| D4 | No accessibility requirement (alt text for images, a disabled state that doesn't rely on colour only, keyboard access). | Low |
| D5 | No search or filter (e.g. vegetarian), and it doesn't say they're out of scope. | Low |
| D6 | No Figma link in the story itself. The design isn't attached to the requirement. | Medium |
| D7 | The blue text in the document looks like unreviewed edits, so it's unclear which wording is final. | Low |

### E. User Story vs Figma design

Design: [EPAM IT Marathon 6.0, "Placing pre-order by guest | Menu"](https://www.figma.com/design/P9rkBNLJZ3WabXSQX9b5DO/EPAM-IT-Marathon-6.0?node-id=111683-86716) ([local copy](attachments/00-figma-guest-menu.jpg)).

| # | Issue | Severity | Question / suggestion |
|---|---|---|---|
| E1 | The story says **"On Stop"**, but the design badge says **"Sold Out"**. A third term for the same state will show up in the app (see BR-04). | Medium | Agree on one user-facing label and use it in the US, Figma and the app. |
| E2 | AC2 says **each** item shows its availability status, but in the design only unavailable dishes have a badge. Available dishes show nothing. | Medium | Either change AC2 to "unavailable items are marked", or add an "Available" state to the design. |
| E3 | In the design, only the first card shows the **Pre-Order** button. It looks like a hover state, but no note says so. On touch screens there is no hover, so AC3 ("clear action button") might not hold on mobile. | High | Say when the button is visible (always or on hover) and add the mobile behaviour. |
| E4 | The design has no **"All dishes"** tab and opens on **Appetizers**. The story doesn't say which category is open by default. | Medium | Add the default category to the AC. |
| E5 | The design adds **Sort by** (Popular First, Newest First, Price: Low to High, Price: High to Low), which the story doesn't mention, and doesn't explain what "Popular" is based on. | Medium | Add a sorting AC with the default option, or mark sorting as out of scope. |
| E6 | The design's reservation banner shows address, table, **date** and time slot, and the header is a "Guest variant" (logo and cart only). Neither is in the story. | Medium | Add an AC: "the guest sees which reservation they're ordering for (location, table, date, time)". |
| E7 | Typo in the design: the tab is spelled "Main **Cources**". | Low | Fix it to "Main Courses". |
| E8 | The page title says "Green & Tasty Restaurants" and doesn't name the location, so the guest can't see whose menu it is (see A5). | Low | Show the location name, or rely on the banner (E6). |
| E9 | The design doesn't show loading, empty, error, cart counter, or the state after a dish is added (see C12, D1). | Medium | Add the missing frames. |

---

## Summary

The story's main value (the guest sees the menu of the right restaurant and orders only what's available) is clear, but the story **isn't ready for development**:

- the role and the restaurant/location aren't defined (A4, A5);
- the scope mixes viewing and ordering (A6);
- the negative and edge cases (other statuses, a dish going On Stop, quantity, errors) are missing (B3, B4, C11, C13, C17);
- several ACs use subjective words ("logically", "clearly") that can't be tested (C1, C4, C15);
- the story and the design disagree on terms and scope ("On Stop" vs "Sold Out", sorting, the default tab, the button visibility) (E1–E5).
