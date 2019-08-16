import { Injectable } from "@angular/core";
import { Actions, Effect, ofType } from "@ngrx/effects";
import { Observable } from "rxjs";
import { map, switchMap } from "rxjs/operators";

import { StoreOverviewsAction, ReloadOverviewsAction, UsersActionType } from "./actions";
import { UsersService } from "./service";

@Injectable()
export class UsersEffects {

    public constructor(
            actions: Actions,
            usersService: UsersService) {
        this.reloadOverviewsEffect = actions.pipe(
            ofType<ReloadOverviewsAction>(UsersActionType.ReloadOverviews),
            switchMap(() => usersService.getOverviews()),
            map(overviews => new StoreOverviewsAction(overviews)));
    }

    @Effect()
    public readonly reloadOverviewsEffect: Observable<StoreOverviewsAction>;
}
