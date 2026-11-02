import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap } from '@angular/router';
import { NEVER, of } from 'rxjs';
import { GroupOrdersService } from '@app/services/group-orders/group-orders.service';
import { ToastService } from '@app/services/toast/toast.service';
import { GuestInvite } from '@app/models/reservations/guest-invite.model';
import { GroupOrderPageComponent } from './group-order-page.component';

const INVITE = { GroupReservationId: 'grp-1', LocationAddress: 'Rustaveli Avenue 48', MaxGuestCount: 4,
  StartTime: '2026-11-03T16:00:00Z', EndTime: '2026-11-03T17:30:00Z' } as GuestInvite;

describe('GroupOrderPageComponent · load order', () => {
  let service: jasmine.SpyObj<GroupOrdersService>;

  function create() {
    TestBed.configureTestingModule({
      imports: [GroupOrderPageComponent],
      providers: [
        { provide: GroupOrdersService, useValue: service },
        { provide: ToastService, useValue: jasmine.createSpyObj('ToastService', ['success', 'error', 'info']) },
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: convertToParamMap({ reservationId: 'res-1' }) } } },
      ],
    });
    const fixture = TestBed.createComponent(GroupOrderPageComponent);
    fixture.detectChanges();
    return fixture;
  }

  beforeEach(() => {
    service = jasmine.createSpyObj<GroupOrdersService>('GroupOrdersService', [
      'getGroupInvite$', 'getSessions$', 'getOwnerSessionId$', 'getStatus$', 'submit$',
    ]);
    service.getGroupInvite$.and.returnValue(of(INVITE));
    service.getSessions$.and.returnValue(of({ participants: [], total: 0 }));
    service.getStatus$.and.returnValue(of({ currentParticipants: 1, maxCapacity: 4, status: 'Open' as const }));
  });

  it('does not load sessions until the owner session id is known (the backend creates it on that call)', () => {
    service.getOwnerSessionId$.and.returnValue(NEVER);
    const fixture = create();

    expect(service.getOwnerSessionId$).toHaveBeenCalledOnceWith('res-1');
    expect(service.getSessions$).not.toHaveBeenCalled();
    fixture.destroy();
  });

  it('loads sessions once the owner session id has arrived', () => {
    service.getOwnerSessionId$.and.returnValue(of('owner'));
    const fixture = create();

    expect(service.getSessions$).toHaveBeenCalledOnceWith('grp-1');
    fixture.destroy();
  });
});
