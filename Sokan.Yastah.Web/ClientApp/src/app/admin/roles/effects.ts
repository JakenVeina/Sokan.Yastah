import { Injectable } from "@angular/core";
import { Actions, Effect, ofType } from "@ngrx/effects";
import { Observable } from "rxjs";
import { map, switchMap } from "rxjs/operators";

import { StoreIdentitiesAction, ReloadIdentitiesAction, RolesActionType } from "./actions";
import { RolesService } from "./service";

@Injectable()
export class RolesEffects {

    public constructor(
            actions: Actions,
            rolesService: RolesService) {
        this.reloadIdentitiesEffect = actions.pipe(
            ofType<ReloadIdentitiesAction>(RolesActionType.ReloadIdentities),
            switchMap(() => rolesService.getIdentities()),
            map(roles => new StoreIdentitiesAction(roles)));
    }

    @Effect()
    public readonly reloadIdentitiesEffect: Observable<StoreIdentitiesAction>;
}
