import { Injectable } from "@angular/core";
import { Actions, Effect, ofType } from "@ngrx/effects";
import { Observable } from "rxjs";
import { map, switchMap } from "rxjs/operators";

import { LoadDescriptionsAction, PermissionsActionType, StoreDescriptionsAction } from "./actions";
import { PermissionsService } from "./service";

@Injectable()
export class PermissionsEffects {

    public constructor(
            actions: Actions,
            permissionsService: PermissionsService) {

        this.loadDescriptionsEffect = actions.pipe(
            ofType<LoadDescriptionsAction>(PermissionsActionType.LoadDescriptions),
            switchMap(() => permissionsService.getDescriptions()),
            map(x => new StoreDescriptionsAction(x)));
    }

    @Effect()
    public readonly loadDescriptionsEffect: Observable<StoreDescriptionsAction>;
}
