# Test Run — US11 Guest Viewing Reservation Menu

**Build under test:** screenshot of the implemented Menu page ([attachments/00-app-menu-screenshot.png](attachments/00-app-menu-screenshot.png)), `https://d28jftogiwtein.cloudfront.net/join/<token>`, desktop browser, ~1900 px wide.
**Compared with:** the User Story and the Figma frame "Placing pre-order by guest | Menu" ([attachments/00-figma-guest-menu.jpg](attachments/00-figma-guest-menu.jpg)).

The app was checked from one static screenshot, so only what is visible on it could be verified. Steps that need clicks, back-office changes or other users are marked **Not run**.

| TC | Title | Status | Notes / Defects |
|---|---|---|---|
| 1 | Guest sees the Menu page with the reservation details | ❌ Fail | Menu page and breadcrumb are shown, but the header is the customer header ([BR-02](04-defect-reports.md#id-2)) and the banner has no date ([BR-03](04-defect-reports.md#id-3)). |
| 2 | Menu is grouped by categories | ❌ Fail | Tabs are "All dishes / Appetizers / Main Courses": "Desserts" is missing and "All dishes" is selected by default ([BR-05](04-defect-reports.md#id-5)). Tab switching not run. |
| 3 | Full menu of the reservation's location only | ⏸ Not run | Needs back-office data. Only 3 dishes are visible on "All dishes". |
| 4 | Every card shows image, name, price, weight | ❌ Fail | All three dishes show the same salad photo ([BR-01](04-defect-reports.md#id-1)). Weight format is inconsistent ([BR-07](04-defect-reports.md#id-7)). Name and price are shown. |
| 5 | Available dish has an enabled [Pre-Order] | ✅ Pass | "Margherita Pizza": card isn't greyed out, no badge, and [Pre-Order] is dark green. The button is always visible (no hover needed). |
| 6 | Add an available dish to the cart | ⏸ Not run | Needs a click. |
| 7 | Second click increases the quantity | ⏸ Not run | Needs a click. The expected behaviour isn't defined yet (C11). |
| 8 | Dish goes only to the Guest's own cart | ⏸ Not run | Needs a second Guest. |
| 9 | "On Stop" dish is shown as unavailable | ❌ Fail | Card is greyed out and [Pre-Order] is disabled (OK), but the badge says "Out Of Stock" instead of "Sold Out" ([BR-04](04-defect-reports.md#id-4)). |
| 10 | "On Stop" dish cannot be added | ⏸ Not run | Visually the button is disabled. The click wasn't tested. |
| 11 | Dish put "On Stop" shows as unavailable after refresh | ⏸ Not run | Needs a back-office change. |
| 12 | Dish taken off "On Stop" becomes available | ⏸ Not run | Needs a back-office change. |
| 13 | Price/weight change shown after refresh | ⏸ Not run | Needs a back-office change. |
| 14 | Dish put "On Stop" after load cannot be added | ⏸ Not run | Needs a back-office change. |
| 15 | Dish sorting options | ❌ Fail | Default is "Recommended" instead of "Popular First" ([BR-06](04-defect-reports.md#id-6)). Options and sorting weren't run. |
| 16 | No adding after the pre-order was submitted | ⏸ Not run | Needs the Owner's action. |
| 17 | Removed Guest cannot add dishes | ⏸ Not run | Needs the Owner's action. |
| 18 | Placeholder for a dish without an image | ⏸ Not run | Needs test data. |
| 19 | Menu page on 375 px | ⏸ Not run | Only a desktop screenshot is available. |

**Totals:** 19 test cases, ✅ 1 passed, ❌ 5 failed, ⏸ 13 not run. **7 defects** reported.

## Observations (not reported as defects)

- Prices are shown with two decimals ("$12.00"), while Figma shows whole numbers ("$17"). Two decimals are needed for "$22.50", so this looks like a design simplification. Worth confirming with the designer.
- The page subtitle is "Green & Tasty Restaurants", without the location name. This matches Figma, but it is a requirements gap (A5, E8).
- The URL stays `/join/<token>` on the Menu page. If the Guest refreshes the page, it's worth checking whether they get the "You're invited" pop-up again (covered by TC 11, step 2).
