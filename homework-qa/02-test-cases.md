# Test Cases — US11 Guest Viewing Reservation Menu

Format follows the course template (ID, Title, Pre-conditions, Steps, Expected result). Each step's number matches the number of its expected result.

**Common pre-conditions** (referred to as **CP** below):
- The Owner has generated and shared a valid invite link.
- Reservation status is "Reserved".
- The Guest has joined the group session (US_G10): opened the link, entered a name and clicked [Browse menu].

**Test data:** reservation at *48 Rustaveli Avenue, Table 1*, with dishes in every category, at least one dish marked "On Stop" and at least one available.

Where the story doesn't define the expected behaviour, the expected result is an **assumption** and links to the open question in [01-requirements-analysis.md](01-requirements-analysis.md).

---

### ID: 1
**Title:** Verify that the Guest sees the Menu page with the reservation details after joining the group session

| | |
|---|---|
| **Pre-conditions** | The Owner has generated and shared a valid invite link. Reservation status is "Reserved". |
| **Steps** | 1. Open the invite link.<br>2. Enter a name in the "Enter your name" field and click [Browse menu].<br>3. Look at the page header.<br>4. Look at the green reservation banner. |
| **Expected result** | 1. The "You're invited" pop-up is opened.<br>2. The Guest is redirected to the Menu page (breadcrumb "Main page › Menu").<br>3. The Guest variant header is shown: logo and cart icon only, with no "Main page", "Book a Table", "Reservations" or profile icon (Figma).<br>4. The banner says "You are making a pre-order for the reservation &lt;address&gt;, Table &lt;N&gt;, &lt;date&gt;, &lt;time from&gt; - &lt;time to&gt;", and the values match the reservation. |

### ID: 2
**Title:** Verify that the menu is grouped by categories (AC1)

| | |
|---|---|
| **Pre-conditions** | CP. |
| **Steps** | 1. Look at the category tabs on the Menu page.<br>2. Click the [Main Courses] tab.<br>3. Click the [Desserts] tab.<br>4. Click the [Appetizers] tab. |
| **Expected result** | 1. Tabs "Appetizers", "Main Courses", "Desserts" are shown, and "Appetizers" is selected by default (Figma, see E4).<br>2. Only Main Course dishes are shown, and the tab is highlighted as active.<br>3. Only Desserts are shown.<br>4. Only Appetizers are shown. |

### ID: 3
**Title:** Verify that the Guest sees the full menu of the reservation's location only

| | |
|---|---|
| **Pre-conditions** | CP. The list of dishes for 48 Rustaveli Avenue is taken from the back-office/API. |
| **Steps** | 1. Open each category tab and count the dishes.<br>2. Compare the dishes with the location's menu from the back-office. |
| **Expected result** | 1. Every dish of the location is shown in its own category, including On Stop dishes.<br>2. No dish from another location is shown, and no dish of this location is missing (assumption, see A5). |

### ID: 4
**Title:** Verify that every dish card shows image, name, price and weight (AC2)

| | |
|---|---|
| **Pre-conditions** | CP. |
| **Steps** | 1. Look at the cards in the "Appetizers" tab.<br>2. Compare each card with the dish data in the back-office. |
| **Expected result** | 1. Every card shows the dish's own image, name, price in "$" and weight in the format "&lt;number&gt; g", laid out as in Figma.<br>2. Image, name, price and weight match the back-office data for that dish. |

### ID: 5
**Title:** Verify that an available dish shows an enabled [Pre-Order] button (AC2, AC3)

| | |
|---|---|
| **Pre-conditions** | CP. The dish "Margherita Pizza" is available. |
| **Steps** | 1. Find the "Margherita Pizza" card.<br>2. Hover over the card. |
| **Expected result** | 1. The card isn't greyed out, and it has no "Sold Out" badge.<br>2. The [Pre-Order] button is visible and enabled (dark green). |

### ID: 6
**Title:** Verify that the Guest can add an available dish to their personal cart (AC3)

| | |
|---|---|
| **Pre-conditions** | CP. The Guest's cart is empty. The dish "Margherita Pizza" is available. |
| **Steps** | 1. Click [Pre-Order] on the "Margherita Pizza" card.<br>2. Click the cart icon in the header. |
| **Expected result** | 1. The dish is added. The Guest gets visual feedback, e.g. a cart counter or a toast (assumption, see C12).<br>2. The cart contains "Margherita Pizza", quantity 1, $14.99. |

### ID: 7
**Title:** Verify that clicking [Pre-Order] on the same dish twice increases the quantity

| | |
|---|---|
| **Pre-conditions** | CP. "Margherita Pizza" is already in the Guest's cart with quantity 1. |
| **Steps** | 1. Click [Pre-Order] on the "Margherita Pizza" card.<br>2. Open the cart. |
| **Expected result** | 1. The dish is added again.<br>2. The cart contains one "Margherita Pizza" line with quantity 2 and subtotal $29.98, not two separate lines (assumption, see C11). |

### ID: 8
**Title:** Verify that a dish added by the Guest goes only to the Guest's own cart

| | |
|---|---|
| **Pre-conditions** | CP. A second Guest has joined the same group. The Owner has the group pre-order screen open. |
| **Steps** | 1. Guest 1 clicks [Pre-Order] on "Margherita Pizza".<br>2. Guest 2 opens their cart.<br>3. The Owner refreshes the group pre-order screen. |
| **Expected result** | 1. The dish is added to Guest 1's cart.<br>2. Guest 2's cart doesn't contain the dish.<br>3. The dish is listed under Guest 1 only. |

### ID: 9
**Title:** Verify that a dish marked "On Stop" is shown as unavailable (AC4)

| | |
|---|---|
| **Pre-conditions** | CP. The dish "Fresh Avocado Toast" is marked "On Stop" in the back-office. |
| **Steps** | 1. Find the "Fresh Avocado Toast" card.<br>2. Hover over the card. |
| **Expected result** | 1. The card is greyed out, and the badge "Sold Out" is shown in its top-right corner (Figma; label to be agreed, see E1).<br>2. No enabled [Pre-Order] button is shown. |

### ID: 10
**Title:** Verify that a dish marked "On Stop" cannot be added to the cart (AC4)

| | |
|---|---|
| **Pre-conditions** | CP. The Guest's cart is empty. "Fresh Avocado Toast" is "On Stop". |
| **Steps** | 1. Click the [Pre-Order] button (if shown) or the card of "Fresh Avocado Toast".<br>2. Open the cart. |
| **Expected result** | 1. Nothing happens, and no request to add the dish is sent.<br>2. The cart is empty. |

### ID: 11
**Title:** Verify that a dish put "On Stop" while the page is open shows as unavailable after refresh (AC5)

| | |
|---|---|
| **Pre-conditions** | CP. "Margherita Pizza" is available, and the Guest has the Menu page open. |
| **Steps** | 1. In the back-office, mark "Margherita Pizza" as "On Stop".<br>2. Refresh the Menu page (F5).<br>3. Look at the "Margherita Pizza" card. |
| **Expected result** | 1. The status is saved in the back-office.<br>2. The Menu page is reloaded, and the Guest stays in the same group session (no new "You're invited" pop-up).<br>3. The card is greyed out with the "Sold Out" badge, and [Pre-Order] is not available. |

### ID: 12
**Title:** Verify that a dish taken off "On Stop" becomes available after refresh (AC5)

| | |
|---|---|
| **Pre-conditions** | CP. "Fresh Avocado Toast" is "On Stop", and the Guest has the Menu page open. |
| **Steps** | 1. In the back-office, make "Fresh Avocado Toast" available.<br>2. Refresh the Menu page.<br>3. Click [Pre-Order] on "Fresh Avocado Toast". |
| **Expected result** | 1. The status is saved.<br>2. The card is no longer greyed out, and the "Sold Out" badge is gone.<br>3. The dish is added to the Guest's cart. |

### ID: 13
**Title:** Verify that a price and weight change is shown after refresh (AC5)

| | |
|---|---|
| **Pre-conditions** | CP. "Margherita Pizza" costs $14.99, 450 g. |
| **Steps** | 1. In the back-office, change the price to $15.50 and the weight to 500 g.<br>2. Refresh the Menu page. |
| **Expected result** | 1. The changes are saved.<br>2. The card shows "$15.50" and "500 g". |

### ID: 14
**Title:** Verify that a dish put "On Stop" after the page was loaded cannot be added without refresh (negative)

| | |
|---|---|
| **Pre-conditions** | CP. "Margherita Pizza" is available, and the Guest has the Menu page open. |
| **Steps** | 1. In the back-office, mark "Margherita Pizza" as "On Stop".<br>2. Without refreshing, click [Pre-Order] on "Margherita Pizza".<br>3. Open the cart. |
| **Expected result** | 1. The status is saved.<br>2. An error message says the dish is no longer available, and the card switches to "Sold Out" (assumption, see C13, C19).<br>3. The dish isn't in the cart. |

### ID: 15
**Title:** Verify the dish sorting options

| | |
|---|---|
| **Pre-conditions** | CP. The "Appetizers" tab has at least 3 dishes with different prices. |
| **Steps** | 1. Look at the [Sort by] dropdown.<br>2. Open the dropdown.<br>3. Select "Price: Low to High".<br>4. Select "Price: High to Low". |
| **Expected result** | 1. "Popular First" is selected by default (Figma).<br>2. The options are: Popular First, Newest First, Price: Low to High, Price: High to Low.<br>3. Dishes are sorted by price, lowest first.<br>4. Dishes are sorted by price, highest first. |

### ID: 16
**Title:** Verify that the Guest cannot add dishes after the group pre-order was submitted (negative)

| | |
|---|---|
| **Pre-conditions** | CP. The Guest has the Menu page open. |
| **Steps** | 1. The Owner submits the group pre-order to the restaurant.<br>2. The Guest refreshes the Menu page.<br>3. The Guest tries to click [Pre-Order] on an available dish. |
| **Expected result** | 1. The group pre-order status is "Submitted".<br>2. The Guest sees a message that the pre-order is closed (assumption, see B3).<br>3. The dish isn't added. |

### ID: 17
**Title:** Verify that a Guest removed by the Owner cannot add dishes (negative)

| | |
|---|---|
| **Pre-conditions** | CP. The Guest has the Menu page open. |
| **Steps** | 1. The Owner removes the Guest from the group.<br>2. The Guest clicks [Pre-Order] on an available dish. |
| **Expected result** | 1. The Guest is removed from the Owner's list.<br>2. The dish isn't added, and the Guest sees a message that they're no longer part of the group (assumption, see B3). |

### ID: 18
**Title:** Verify that a dish without an image shows a placeholder

| | |
|---|---|
| **Pre-conditions** | CP. One available dish has no image in the back-office. |
| **Steps** | 1. Open the category of that dish. |
| **Expected result** | 1. The card shows a placeholder image, not a broken image icon or another dish's photo, and the rest of the card is laid out normally (assumption, see C7). |

### ID: 19
**Title:** Verify the Menu page on a mobile screen (375 px)

| | |
|---|---|
| **Pre-conditions** | CP. The Menu page is opened on a 375 px wide screen (or in DevTools device mode). |
| **Steps** | 1. Scroll through the "Appetizers" tab.<br>2. Tap [Pre-Order] on an available dish. |
| **Expected result** | 1. There is no horizontal scroll. Cards, tabs and Sort by fit the screen, and the [Pre-Order] button is visible without hover (see E3).<br>2. The dish is added to the cart. |
