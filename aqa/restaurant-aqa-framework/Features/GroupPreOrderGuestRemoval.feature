@L3
Feature: Removing a guest from the group pre-order

  The owner can take a guest out of the group pre-order — someone who joined by mistake,
  or who will not come after all. The guest's cart goes with them: the owner sees the
  updated list of guests and a new total, and the removed guest's dishes never reach the
  restaurant. Only the owner can remove a guest, and only a guest of their own group.

  Background:
    Given a restaurant whose menu has
      | dish     | price |
      | Borscht  | 8.50  |
      | Varenyky | 6.00  |
      | Uzvar    | 3.00  |
    And a customer has booked a table for 6 guests

  Rule: The other guests stay and the total no longer counts the removed guest

    Background:
      Given these guests have joined the group
        | guest  | cart                  | cart submitted |
        | Olena  | 2 Borscht, 1 Varenyky | yes            |
        | Taras  | 3 Varenyky            | yes            |
        | Mariia | 1 Uzvar               | no             |

    @api @smoke
    Scenario: The owner removes a guest who has already submitted their cart
      When the owner removes Taras from the group
      Then the owner sees these guests
        | guest  | dishes                | subtotal | cart submitted |
        | Olena  | 2 Borscht, 1 Varenyky | 23.00    | yes            |
        | Mariia | 1 Uzvar               | 3.00     | no             |
      And the grand total is the sum of the participants' subtotals
      And the grand total is 26.00

    @ui @smoke
    Scenario: The owner removes a guest who has already submitted their cart on screen
      When the owner removes Taras from the group
      Then the owner sees these guests
        | guest  | dishes                | subtotal | cart submitted |
        | Olena  | 2 Borscht, 1 Varenyky | 23.00    | yes            |
        | Mariia | 1 Uzvar               | 3.00     | no             |
      And the grand total of the submitted carts is 23.00

    @api
    Scenario: The owner removes a guest who is still choosing
      When the owner removes Mariia from the group
      Then the owner sees these guests
        | guest | dishes                | subtotal | cart submitted |
        | Olena | 2 Borscht, 1 Varenyky | 23.00    | yes            |
        | Taras | 3 Varenyky            | 18.00    | yes            |
      And the grand total is 41.00

    @api
    Scenario: A removed guest is gone for good
      Given the owner has removed Taras from the group
      When the owner looks at the group pre-order
      Then the owner sees these guests
        | guest  | dishes                | subtotal | cart submitted |
        | Olena  | 2 Borscht, 1 Varenyky | 23.00    | yes            |
        | Mariia | 1 Uzvar               | 3.00     | no             |
      And the grand total is 26.00

  Rule: Removing the last guest leaves a group with nothing to show

    Background:
      Given these guests have joined the group
        | guest | cart      | cart submitted |
        | Olena | 2 Borscht | yes            |

    @api
    Scenario: The owner removes the only guest
      When the owner removes Olena from the group
      Then the group pre-order is empty

    @ui
    Scenario: The owner removes the only guest on screen
      When the owner removes Olena from the group
      Then the group pre-order is empty

  Rule: Only a guest who exists and belongs to the owner's group can be removed

    @api @negative
    Scenario: The owner tries to remove a guest of another group
      Given these guests have joined the group
        | guest | cart      | cart submitted |
        | Olena | 2 Borscht | yes            |
      And a second customer has booked another table for 4 guests
      And Bohdan has joined the second customer's group
      When the owner removes Bohdan from the group
      Then the owner is told the guest does not belong to this group
      And Olena is still in the group

    @api @negative
    Scenario: The owner tries to remove a guest who does not exist
      Given these guests have joined the group
        | guest | cart      | cart submitted |
        | Olena | 2 Borscht | yes            |
      When the owner removes a guest who does not exist
      Then the owner is told the guest could not be found
      And Olena is still in the group

  Rule: Only the owner of the group can remove a guest

    Background:
      Given these guests have joined the group
        | guest | cart      | cart submitted |
        | Olena | 2 Borscht | yes            |

    @api @negative
    Scenario Outline: Someone signed in who is not the owner cannot remove a guest
      When <viewer> removes Olena from the group
      Then they are refused because only the owner may remove guests
      And Olena is still in the group

      Examples: Signed in, but not the owner
        | viewer           |
        | another customer |
        | the waiter       |

    @api @negative
    Scenario Outline: Someone who is not signed in cannot remove a guest
      When <viewer> removes Olena from the group
      Then they are asked to sign in before removing guests
      And Olena is still in the group

      Examples: Not signed in
        | viewer                         |
        | a guest of the group           |
        | a visitor who is not signed in |

  Rule: A removed guest's dishes never reach the restaurant

    Background:
      Given the owner has pre-ordered 1 Uzvar
      And these guests have joined the group
        | guest  | cart                  | cart submitted |
        | Olena  | 2 Borscht, 1 Varenyky | yes            |
        | Taras  | 3 Varenyky            | yes            |
        | Mariia | 1 Borscht             | yes            |

    @api @e2e
    Scenario: Guests order, the owner removes one and submits, the waiter sees the rest
      When the owner looks at the group pre-order
      Then the owner sees these guests
        | guest  | dishes                | subtotal | cart submitted |
        | Olena  | 2 Borscht, 1 Varenyky | 23.00    | yes            |
        | Taras  | 3 Varenyky            | 18.00    | yes            |
        | Mariia | 1 Borscht             | 8.50     | yes            |
      When the owner removes Taras from the group
      And the owner submits the group pre-order
      Then the pre-order is sent to the restaurant
      When the waiter opens the booking's pre-order
      Then the waiter sees these dishes
        | dish     | quantity |
        | Borscht  | 3        |
        | Varenyky | 1        |
        | Uzvar    | 1        |

    @ui @e2e
    Scenario: Guests order, the owner removes one and submits, the waiter sees the rest on screen
      When the owner looks at the group pre-order
      Then the owner sees these guests
        | guest  | dishes                | subtotal | cart submitted |
        | Olena  | 2 Borscht, 1 Varenyky | 23.00    | yes            |
        | Taras  | 3 Varenyky            | 18.00    | yes            |
        | Mariia | 1 Borscht             | 8.50     | yes            |
      When the owner removes Taras from the group
      And the owner submits the group pre-order
      Then the pre-order is sent to the restaurant
      When the waiter opens the booking's pre-order
      Then the waiter sees these dishes
        | dish     | quantity |
        | Borscht  | 3        |
        | Varenyky | 1        |
        | Uzvar    | 1        |
