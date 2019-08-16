import { Injectable } from "@angular/core";
import { Actions, Effect, ofType } from "@ngrx/effects";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs";
import { filter, map, switchMap, withLatestFrom } from "rxjs/operators";

import { IAppState } from "../../state";

import { LoadDescriptionsAction, PermissionsActionType, StoreDescriptionsAction } from "./actions";
import { PermissionsService } from "./service";

@Injectable()
export class PermissionsEffects {

    public constructor(
            appState: Store<IAppState>,
            actions: Actions,
            permissionsService: PermissionsService) {

        this.loadDescriptionsEffect = actions.pipe(
            ofType<LoadDescriptionsAction>(PermissionsActionType.LoadDescriptions),
            withLatestFrom(appState.select(x => x.admin.permissions.descriptions)),
            filter(([action, descriptions]) => (descriptions.length === 0)),
            switchMap(() => permissionsService.getDescriptions()),
            map(x => new StoreDescriptionsAction(x)));
    }

    @Effect()
    public readonly loadDescriptionsEffect: Observable<StoreDescriptionsAction>;
}
