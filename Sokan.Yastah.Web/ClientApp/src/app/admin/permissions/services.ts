import { Injectable } from "@angular/core";

import { of, Observable } from "rxjs";
import { filter, shareReplay, switchMap, tap } from "rxjs/operators";

import { Store } from "@ngrx/store";

import { ApiClient } from "../../common/api-client";
import { IAppState } from "../../state";

import { IPermissionCategoryDescriptionViewModel } from "./models";
import { PermissionsActionFactory } from "./state.actions";
import { PermissionsSelectors } from "./state.selectors";


@Injectable({
    providedIn: "root"
})
export class PermissionsService {

    public constructor(
            apiClient: ApiClient,
            appState: Store<IAppState>) {

        this._apiClient = apiClient;
        this._appState = appState;
    }

    public observeDescriptions():
            Observable<IPermissionCategoryDescriptionViewModel[]> {
        return this._appState.select(PermissionsSelectors.descriptionsNeedsFetched)
            .pipe(
                tap(needsFetched => needsFetched
                    ? setTimeout(() => of(null)
                        .pipe(
                            tap(() => this._appState.dispatch(PermissionsActionFactory.beginFetchDescriptions())),
                            switchMap(() => this._apiClient.get<IPermissionCategoryDescriptionViewModel[]>("admin/permissions/descriptions")),
                            tap(descriptions => this._appState.dispatch(PermissionsActionFactory.storeDescriptions({
                                descriptions
                            }))))
                        .toPromise())
                    : null),
                switchMap(() => this._appState.select(PermissionsSelectors.descriptions)),
                filter(descriptions => descriptions != null),
                shareReplay());
    }

    private readonly _apiClient: ApiClient;
    private readonly _appState: Store<IAppState>;
}
