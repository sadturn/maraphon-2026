# Green Tasty

Restaurant chain with 3 locations, 10–20 tables per location.

**Current main capabilities:**
- Restaurant discovery — location details, ratings, and review browsing
- Table reservations
- Menu browsing
- Pre-ordering and order management
- Guest feedback collection

## Homework 1 - Create User Story

<table>
  <tr><td><b>Epic</b></td><td>Order Management</td></tr>
  <tr><td><b>US name</b></td><td>Consolidated Group Pre-order View</td></tr>
  <tr><td><b>Role</b></td><td>Customer (Reservation Owner)</td></tr>
  <tr><td><b>Description</b></td><td>As a Reservation Owner, I want to view all guests' pre-orders in my group in one consolidated view, so I can review them in one place and ensure they are correct.</td></tr>
</table>

Acceptance Criteria

1. Given a group has pre-orders from multiple guests, when the group owner opens the consolidated view, then the pre-orders are shown together in that view.
2. Given a guest in the group has a pre-order, when the group owner opens the consolidated view, then that guest’s pre-order is included.
3. Given the group has multiple guest pre-orders, when the group owner views them, then they can be viewed without opening a separate view for each guest. 

## Homework 2 - Design Improve in Figma

Improved [the cart screen](https://www.figma.com/design/VAfhBVYcF7xawSNyG0MeCq/EPAM-IT-Marathon-6.0--Copy-?node-id=115001-49572) for the order owner.

## Homework 3 - Creating and Verifying an AWS Account

Completed.

## Homework 4 - Deployment

[Green Tasty](https://d11rv63sjw7xpo.cloudfront.net/)

## Homework 4 - Backend

**Group Pre-order: Functionality for the Owner**

- [x] Level 1
GET list of group participants' sessions: items, individual totals, and the grand total.

- [x] Level 2
	GET `guestSessionId` for the owner's session.
POST confirmation of the consolidated pre-order (includes only submitted sessions; aggregates items by `dishId`; updates statuses).

- [x] Level 3
	DELETE a guest session from the group; the response returns the updated list and total.