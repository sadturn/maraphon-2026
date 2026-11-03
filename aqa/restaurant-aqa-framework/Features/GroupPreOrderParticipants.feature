@L1
Feature: Group pre-order participants

  A customer who books a table for several guests owns the booking's group pre-order.
  Guests join it by invitation and fill their own carts; the owner follows what everyone
  has chosen before one order goes to the restaurant. Only the owner may look at it.

  The owner's screen shows the consolidated order as its grand total, so it counts
  submitted carts only. The service's total counts every participant's cart.

  Background:
    Given a restaurant whose menu has
      | dish     | price |
      | Borscht  | 8.50  |
      | Varenyky | 6.00  |
      | Uzvar    | 3.00  |
    And a customer has booked a table for 6 guests

  Rule: The owner sees every guest with their dishes, quantities and subtotal

    Background:
      Given these guests have joined the group
        | guest | cart                  | cart submitted |
        | Olena | 2 Borscht, 1 Varenyky | yes            |
        | Taras | 3 Varenyky            | yes            |

    @api @smoke
    Scenario: The owner sees what each guest has chosen
      When the owner looks at the group pre-order
      Then the owner sees these guests
        | guest | dishes                | subtotal | cart submitted |
        | Olena | 2 Borscht, 1 Varenyky | 23.00    | yes            |
        | Taras | 3 Varenyky            | 18.00    | yes            |
      And the grand total is 41.00

    @ui @smoke
    Scenario: The owner sees what each guest has chosen on screen
      When the owner looks at the group pre-order
      Then the owner sees these guests
        | guest | dishes                | subtotal | cart submitted |
        | Olena | 2 Borscht, 1 Varenyky | 23.00    | yes            |
        | Taras | 3 Varenyky            | 18.00    | yes            |
      And the grand total is 41.00

  Rule: The grand total is the sum of the participants' subtotals

    @api
    Scenario: The grand total follows a guest changing their cart
      Given these guests have joined the group
        | guest | cart      | cart submitted |
        | Olena | 2 Borscht | no             |
        | Taras | 1 Uzvar   | yes            |
      When the owner looks at the group pre-order
      Then the grand total is the sum of the participants' subtotals
      And the grand total is 20.00
      When Olena puts 1 Uzvar in the cart
      And Olena takes 1 Borscht out of the cart
      And the owner looks at the group pre-order
      Then the owner sees these guests
        | guest | dishes             | subtotal | cart submitted |
        | Olena | 1 Borscht, 1 Uzvar | 11.50    | no             |
        | Taras | 1 Uzvar            | 3.00     | yes            |
      And the grand total is the sum of the participants' subtotals
      And the grand total is 14.50

  Rule: Guests who are not done yet are listed too

    Background:
      Given these guests have joined the group
        | guest  | cart       | cart submitted |
        | Olena  | 2 Borscht  | yes            |
        | Taras  | 1 Varenyky | no             |
        | Mariia |            | no             |

    @api
    Scenario: A guest still choosing and a guest without dishes are listed
      When the owner looks at the group pre-order
      Then the owner sees these guests
        | guest  | dishes     | subtotal | cart submitted |
        | Olena  | 2 Borscht  | 17.00    | yes            |
        | Taras  | 1 Varenyky | 6.00     | no             |
        | Mariia |            | 0.00     | no             |
      And the grand total is 23.00

    @ui
    Scenario: A guest still choosing and a guest without dishes are listed on screen
      When the owner looks at the group pre-order
      Then the owner sees these guests
        | guest  | dishes     | subtotal | cart submitted |
        | Olena  | 2 Borscht  | 17.00    | yes            |
        | Taras  | 1 Varenyky | 6.00     | no             |
        | Mariia |            | 0.00     | no             |
      And the grand total of the submitted carts is 17.00

  Rule: A group nobody has joined has nothing to show

    @api
    Scenario: The owner looks at a group nobody has joined yet
      When the owner looks at the group pre-order
      Then the group pre-order is empty

    @ui
    Scenario: The owner looks at a group nobody has joined yet on screen
      When the owner looks at the group pre-order
      Then the group pre-order is empty

  Rule: Only the owner of the group can see it

    @api @negative
    Scenario Outline: Someone signed in who is not the owner is refused
      When <viewer> looks at the group pre-order
      Then they are refused because only the owner may see it

      Examples: Signed in, but not the owner
        | viewer           |
        | another customer |
        | the waiter       |

    @api @negative
    Scenario Outline: Someone who is not signed in is asked to sign in
      When <viewer> looks at the group pre-order
      Then they are asked to sign in first

      Examples: Not signed in
        | viewer                         |
        | a guest of the group           |
        | a visitor who is not signed in |

    @ui @negative
    Scenario Outline: Someone signed in who is not the owner is refused on screen
      When <viewer> looks at the group pre-order
      Then they are refused because only the owner may see it

      Examples: Signed in, but not the owner
        | viewer           |
        | another customer |
        | the waiter       |

    @ui @negative
    Scenario: A visitor who is not signed in is asked to sign in on screen
      When a visitor who is not signed in looks at the group pre-order
      Then they are asked to sign in first

  Rule: A group pre-order that does not exist cannot be shown

    @api @negative
    Scenario: The owner asks for a group pre-order that does not exist
      When the owner looks at a group pre-order that does not exist
      Then the owner is told the group pre-order could not be found

    @ui @negative
    Scenario: The owner opens a group pre-order that does not exist
      When the owner looks at a group pre-order that does not exist
      Then the owner is told the group pre-order could not be found
