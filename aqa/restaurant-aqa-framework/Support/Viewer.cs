namespace Restaurant_Aqa_Framework.Support;

/// <summary>Who tries to look at a group pre-order. Only <see cref="Owner"/> may.</summary>
public enum Viewer
{
    Owner,
    AnotherCustomer,
    Waiter,
    GuestOfTheGroup,
    VisitorNotSignedIn,
}
