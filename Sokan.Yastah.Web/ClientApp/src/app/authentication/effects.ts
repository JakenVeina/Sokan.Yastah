import { Injectable } from "@angular/core";
import { Actions, Effect, ofType, ROOT_EFFECTS_INIT } from "@ngrx/effects";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";

import { ReloadTicketAction, StoreTicketAction, AuthenticationActionType } from "./actions";
import { readCurrentTicket } from "./utils";

@Injectable()
export class AuthenticationEffects {

    public constructor(
        actions: Actions) {

        this.initializeTicketEffect = actions.pipe(
            ofType(ROOT_EFFECTS_INIT),
            map(() => new ReloadTicketAction()));

        this.reloadTicketEffect = actions.pipe(
            ofType<ReloadTicketAction>(AuthenticationActionType.ReloadTicket),
            map(() => new StoreTicketAction(readCurrentTicket())));
    }

    @Effect()
    public readonly initializeTicketEffect: Observable<ReloadTicketAction>;

    @Effect()
    public readonly reloadTicketEffect: Observable<StoreTicketAction>;
}
