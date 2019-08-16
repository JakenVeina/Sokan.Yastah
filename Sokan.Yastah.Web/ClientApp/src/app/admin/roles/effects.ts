import { Injectable } from "@angular/core";
import { Actions, Effect, ofType } from "@ngrx/effects";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs";
import { filter, map, switchMap, withLatestFrom } from "rxjs/operators";

import { IAppState } from "../../state";

import { LoadIdentitiesAction, ReloadIdentitiesAction, StoreIdentitiesAction, RolesActionType } from "./actions";
import { RolesService } from "./service";

@Injectable()
export class RolesEffects {

    public constructor(
            appState: Store<IAppState>,
            actions: Actions,
            rolesService: RolesService) {

        this.loadIdentitiesEffect = actions.pipe(
            ofType<LoadIdentitiesAction>(RolesActionType.LoadIdentities),
            withLatestFrom(appState.select(x => x.admin.roles.identities)),
            filter(([action, identities]) => (identities.length === 0)),
            switchMap(() => rolesService.getIdentities()),
            map(x => new StoreIdentitiesAction(x)));

        this.reloadIdentitiesEffect = actions.pipe(
            ofType<ReloadIdentitiesAction>(RolesActionType.ReloadIdentities),
            switchMap(() => rolesService.getIdentities()),
            map(roles => new StoreIdentitiesAction(roles)));
    }

    @Effect()
    public readonly loadIdentitiesEffect: Observable<StoreIdentitiesAction>;

    @Effect()
    public readonly reloadIdentitiesEffect: Observable<StoreIdentitiesAction>;
}
