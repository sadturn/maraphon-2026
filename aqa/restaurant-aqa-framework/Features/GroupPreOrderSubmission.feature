@L2
Feature: Submitting the group pre-order

  Once the owner has looked at what everyone chose, the owner sends one consolidated
  pre-order to the restaurant. It holds the owner's own dishes and the carts guests have
  submitted; the same dish from different people becomes one line with the total quantity.
  Carts still being chosen are left out. The owner's own pre-order is the order the
  restaurant receives, so the guests' dishes are added to it.

  Background:
    Given a restaurant whose menu has
      | dish     | price |
      | Borscht  | 8.50  |
      | Varenyky | 6.00  |
      | Uzvar    | 3.00  |
    And a customer has booked a table for 6 guests

  Rule: The restaurant receives one order with the submitted carts, the same dishes added up

    Background:
      Given the owner has pre-ordered 1 Uzvar
      And these guests have joined the group
        | guest  | cart                  | cart submitted |
        | Olena  | 2 Borscht, 1 Varenyky | yes            |
        | Taras  | 1 Borscht, 1 Uzvar    | yes            |
        | Mariia | 3 Varenyky            | no             |

    @api @smoke
    Scenario: The owner submits the group pre-order
      When the owner submits the group pre-order
      Then the pre-order is sent to the restaurant
      And the group pre-order is marked as submitted
      And the restaurant's order is marked as submitted by the customer
      And the restaurant receives these dishes
        | dish     | quantity |
        | Borscht  | 3        |
        | Varenyky | 1        |
        | Uzvar    | 2        |

    @ui @smoke
    Scenario: The owner submits the group pre-order on screen
      When the owner submits the group pre-order
      Then the pre-order is sent to the restaurant
      And the restaurant receives these dishes
        | dish     | quantity |
        | Borscht  | 3        |
        | Varenyky | 1        |
        | Uzvar    | 2        |

    @api
    Scenario: The waiter sees the order the owner submitted
      Given the owner has submitted the group pre-order
      When the waiter opens the booking's pre-order
      Then the waiter sees these dishes
        | dish     | quantity |
        | Borscht  | 3        |
        | Varenyky | 1        |
        | Uzvar    | 2        |

    @ui
    Scenario: The waiter sees the order the owner submitted on screen
      Given the owner has submitted the group pre-order
      When the waiter opens the booking's pre-order
      Then the waiter sees these dishes
        | dish     | quantity |
        | Borscht  | 3        |
        | Varenyky | 1        |
        | Uzvar    | 2        |

  # A submitted cart can be neither emptied nor changed, and an empty cart cannot be
  # submitted, so "the submitted carts hold no dishes" cannot be set up from the outside
  # (see README).
  Rule: A group pre-order needs a submitted cart and the owner's own pre-order to be sent

    @api @negative
    Scenario: The owner cannot submit before any guest has submitted a cart
      Given the owner has pre-ordered 1 Uzvar
      And these guests have joined the group
        | guest | cart      | cart submitted |
        | Olena | 2 Borscht | no             |
      When the owner submits the group pre-order
      Then the owner is told "There are no submitted guest sessions."

    @ui @negative
    Scenario: The owner cannot submit on screen before any guest has submitted a cart
      Given the owner has pre-ordered 1 Uzvar
      And these guests have joined the group
        | guest | cart      | cart submitted |
        | Olena | 2 Borscht | no             |
      When the owner submits the group pre-order
      Then the owner is told "There are no submitted guest sessions."

    @api @negative
    Scenario: The owner cannot submit without a pre-order of their own
      Given these guests have joined the group
        | guest | cart      | cart submitted |
        | Olena | 2 Borscht | yes            |
      When the owner submits the group pre-order
      Then the owner is told their own pre-order could not be found

    @ui @negative
    Scenario: The owner cannot submit on screen without a pre-order of their own
      Given these guests have joined the group
        | guest | cart      | cart submitted |
        | Olena | 2 Borscht | yes            |
      When the owner submits the group pre-order
      Then the owner is told their own pre-order could not be found

  Rule: Only an existing group and booking can be submitted

    @api @negative
    Scenario: The owner submits a group that does not exist
      When the owner submits a group that does not exist
      Then the owner is told the group could not be found

    @api @negative
    Scenario: The owner submits the group for a booking that does not exist
      When the owner submits the group for a booking that does not exist
      Then the owner is told the booking could not be found

  Rule: A group pre-order is submitted only once

    Background:
      Given the owner has pre-ordered 1 Uzvar
      And these guests have joined the group
        | guest | cart      | cart submitted |
        | Olena | 2 Borscht | yes            |

    @api @negative
    Scenario: The owner submits a group pre-order that was already sent
      Given the owner has submitted the group pre-order
      When the owner submits the group pre-order
      Then the owner is told "Group reservation was submitted."

    @ui @negative
    Scenario: The owner cannot submit on screen once the pre-order was sent
      When the owner submits the group pre-order
      Then the pre-order is sent to the restaurant
      And the owner can no longer submit the group pre-order

  Rule: Only the owner of the group can submit it

    Background:
      Given the owner has pre-ordered 1 Uzvar
      And these guests have joined the group
        | guest | cart      | cart submitted |
        | Olena | 2 Borscht | yes            |

    @api @negative
    Scenario Outline: Someone signed in who is not the owner cannot submit
      When <viewer> submits the group pre-order
      Then they are refused because only the owner may submit it
      And the group pre-order is still open

      Examples: Signed in, but not the owner
        | viewer           |
        | another customer |
        | the waiter       |

    @api @negative
    Scenario Outline: Someone who is not signed in cannot submit
      When <viewer> submits the group pre-order
      Then they are asked to sign in before submitting
      And the group pre-order is still open

      Examples: Not signed in
        | viewer                         |
        | a guest of the group           |
        | a visitor who is not signed in |
