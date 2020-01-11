import { HttpErrorResponse } from "@angular/common/http";
import { Injectable, OnDestroy } from "@angular/core";

import { Store } from "@ngrx/store";

import { of, throwError, Observable, Subject } from "rxjs";
import { catchError, filter, map, shareReplay, skip, switchMap, take, tap } from "rxjs/operators";

import { ApiClient } from "../../common/api-client";
import { IOperationError } from "../../common/types";
import { IAppState } from "../../state";

import { IRoleCreationModel, IRoleDetailViewModel, IRoleIdentityViewModel, IRoleUpdateModel } from "./models";
import { RolesActionFactory } from "./state.actions";
import { RolesSelectors } from "./state.selectors";


const rolesApiPath
    = "admin/roles";

function roleApiPath(roleId: number): string {
    return `${rolesApiPath}/${roleId}`;
}


export interface IRoleCreationResult {
    readonly roleId?: number;
    readonly error?: IOperationError;
}


@Injectable({
    providedIn: "root"
})
export class RolesService
        implements OnDestroy {

    public constructor(
            apiClient: ApiClient,
            appState: Store<IAppState>) {

        this._apiClient = apiClient;
        this._appState = appState;

        this._onDeleted = new Subject();
    }

    public create(
                model: IRoleCreationModel):
            Promise<IRoleCreationResult> {
        return this._apiClient.post<number>(`${rolesApiPath}/new`, model)
            .pipe(
                tap(() => this._appState.dispatch(RolesActionFactory.scheduleFetchIdentities())),
                map(roleId => ({
                    roleId
                })),
                catchError((error: HttpErrorResponse) => (Math.trunc(error.status / 100) === 4)
                    ? of({
                        error: error.error
                    })
                    : throwError(error)))
            .toPromise();
    }

    public delete(
                roleId: number):
            Promise<void> {
        return this._apiClient.delete<void>(roleApiPath(roleId))
            .pipe(
                catchError((error: HttpErrorResponse) => (error.status === 404)
                    ? of(null)
                    : throwError(error)),
                tap(() => {
                    this._appState.dispatch(RolesActionFactory.scheduleFetchIdentities());
                    this._onDeleted.next(roleId);
                }))
            .toPromise();
    }

    public fetchDetail(
                roleId: number):
            Promise<IRoleDetailViewModel | null> {
        return this._apiClient.get<IRoleDetailViewModel>(`${roleApiPath(roleId)}/detail`)
            .pipe(
                catchError((error: HttpErrorResponse) => (error.status === 404)
                    ? of(null)
                    : throwError(error)))
            .toPromise();
    }

    public fetchIdentities():
            Promise<IRoleIdentityViewModel[]> {
        return this._appState.select(RolesSelectors.identitiesIsFetching)
            .pipe(
                take(1),
                switchMap(isFetching => isFetching
                    ? this._appState.select(RolesSelectors.identities)
                        .pipe(
                            skip(1),
                            take(1))
                    : of(null)
                        .pipe(
                            tap(() => this._appState.dispatch(RolesActionFactory.beginFetchIdentities())),
                            switchMap(() => this._apiClient.get<IRoleIdentityViewModel[]>(`${rolesApiPath}/identities`)),
                            tap(identities => this._appState.dispatch(RolesActionFactory.storeIdentities({
                                identities
                            }))))))
            .toPromise();
    }

    public observeIdentities():
            Observable<IRoleIdentityViewModel[]> {
        return this._appState.select(RolesSelectors.identitiesNeedsFetch)
            .pipe(
                tap(needsFetch => needsFetch
                    ? setTimeout(() => this.fetchIdentities())
                    : null),
                switchMap(() => this._appState.select(RolesSelectors.identities)),
                filter(identities => identities != null),
                shareReplay());
    }

    public onDeleted(
                roleId: number):
            Observable<void> {
        return this._onDeleted
            .pipe(
                filter(deletedRoleId => deletedRoleId == roleId),
                map(() => null));
    }

    public update(
                roleId: number,
                model: IRoleUpdateModel):
            Promise<IOperationError | null> {
        return this._apiClient.put<void>(roleApiPath(roleId), model)
            .pipe(
                tap(() => this._appState.dispatch(RolesActionFactory.scheduleFetchIdentities())),
                catchError((error: HttpErrorResponse) => (Math.trunc(error.status / 100) === 4)
                    ? of(error.error)
                    : throwError(error)))
            .toPromise();
    }

    public ngOnDestroy():
            void {
        this._onDeleted.complete();
    }

    private readonly _apiClient: ApiClient;
    private readonly _appState: Store<IAppState>;
    private readonly _onDeleted: Subject<number>;
}
