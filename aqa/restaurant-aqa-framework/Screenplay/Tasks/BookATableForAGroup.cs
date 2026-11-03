using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Api.Dtos.GroupPreOrder;
using Restaurant_Aqa_Framework.Api.Dtos.Reservations;
using Restaurant_Aqa_Framework.Api.Dtos.Tables;
using Restaurant_Aqa_Framework.Api.Seeding;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks;

/// <summary>The customer remembered as <paramref name="by"/> books the scenario's table for <paramref name="guests"/>
/// guests; a booking for two or more opens a group pre-order. Remembers the reservation and its
/// <see cref="GroupBooking"/>, and cancels the reservation when the scenario ends (which also closes the group).</summary>
public sealed class BookATableForAGroup(int guests, string by = "owner") : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var owner = actor.Recall<Account>(by);
        var table = actor.Recall<TableResponse>();
        var reservations = new ReservationsApi(actor.UsingTheApi());
        var groups = new GroupPreOrderApi(actor.UsingTheApi());
        var response = actor.TheLastResponse();

        // The table belongs to this scenario alone, so any time far enough ahead is free.
        var start = DateTime.UtcNow.Date.AddDays(2).AddHours(12);
        await response.CaptureAsync(await reservations.CreateReservationAsync(
            SampleDataFactory.CreateReservationRequest(table.Id, start, guests), owner.IdToken));
        response.ThrowUnlessSucceeded("Booking the table");

        var reservation = response.Json<ReservationResponse>();
        actor.Remember(reservation);
        actor.Recall<CleanupScope>().TrackDelete(
            $"reservation:{reservation.Id}", () => reservations.CancelReservationAsync(reservation.Id, owner.IdToken));

        // Only the invitation tells which group belongs to the reservation.
        await response.CaptureAsync(await groups.GetInviteTokenAsync(reservation.Id, owner.IdToken));
        response.ThrowUnlessSucceeded("Getting the invitation");
        var inviteToken = response.Json<InviteTokenResponse>().Token;

        await response.CaptureAsync(await groups.GetInviteAsync(inviteToken));
        response.ThrowUnlessSucceeded("Opening the invitation");

        actor.Remember(new GroupBooking(reservation.Id, response.Json<GroupInviteResponse>().GroupReservationId));
    }
}
