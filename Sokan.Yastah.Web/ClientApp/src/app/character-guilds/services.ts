import { HttpErrorResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";

import { Store } from "@ngrx/store";

import { of, Observable } from "rxjs";
import { catchError, map, shareReplay, skip, switchMap, take, tap } from "rxjs/operators";

import { ApiClient } from "../api/api-client";
import { IOperationError } from "../common/types";
import { IAppState } from "../state";

import {
    ICharacterGuildDivisionCreationModel,
    ICharacterGuildDivisionIdentityViewModel,
    ICharacterGuildCreationModel,
    ICharacterGuildIdentityViewModel,    ICharacterGuildDivisionUpdateModel
} from "./models";
import {
    CharacterGuildDivisionsActionFactory,
    CharacterGuildsActionFactory,
} from "./state.actions";
import { CharacterGuildDivisionsSelectors, CharacterGuildsSelectors } from "./state.selectors";


export interface ICharacterGuildDivisionCreationResult {
    readonly divisionId?: number;
    readonly error?: IOperationError;
}
export interface ICharacterGuildCreationResult {
    readonly guildId?: number;
    readonly error?: IOperationError;
}


const guildsApiPath = "characters/guilds";

function divisionApiPath(
    guildId: number,
    divisionId: number):
    string {
    return `${divisionsApiPath(guildId)}/${divisionId}`;
}
function divisionsApiPath(
    guildId: number):
    string {
    return `${guildApiPath(guildId)}/divisions`;
}
function guildApiPath(
    guildId: number):
    string {
    return `${guildsApiPath}/${guildId}`;
}


@Injectable({
    providedIn: "root"
})
export class CharacterGuildDivisionsService {

    public constructor(
            apiClient: ApiClient,
            appState: Store<IAppState>) {

        this._apiClient = apiClient;
        this._appState = appState;

        this._observableIdentitiesMap = {};
        this._observableIdentityMap = {};
    }

    public create(
                guildId: number,
                model: ICharacterGuildDivisionCreationModel):
            Promise<ICharacterGuildDivisionCreationResult> {
        return this._apiClient.post<number>(`${divisionsApiPath(guildId)}/new`, model)
            .pipe(
                map(divisionId => ({
                    guildId,
                    divisionId
                })),
                tap(props => this._appState.dispatch(CharacterGuildDivisionsActionFactory.initialize(props))),
                catchError((error: HttpErrorResponse, caught) => (Math.trunc(error.status / 100) === 4)
                    ? of({
                        error: error.error
                    })
                    : caught))
            .toPromise();
    }

    public delete(
                guildId: number,
                divisionId: number):
            Promise<void> {
        return this._apiClient.delete<void>(divisionApiPath(guildId, divisionId))
            .pipe(
                catchError((error: HttpErrorResponse, caught) => (error.status === 404)
                    ? of(null)
                    : caught),
                tap(() => this._appState.dispatch(CharacterGuildDivisionsActionFactory.remove({
                    guildId,
                    divisionId,
                }))))
            .toPromise();
    }

    public fetchIdentities(
                guildId: number):
            Promise<ICharacterGuildDivisionIdentityViewModel[]> {
        return this._appState.select(CharacterGuildDivisionsSelectors.identitiesIsFetching(guildId))
            .pipe(
                take(1),
                switchMap(isFetching => isFetching
                    ? this._appState.select(CharacterGuildDivisionsSelectors.identities(guildId))
                        .pipe(
                            skip(1),
                            take(1))
                    : of(null)
                        .pipe(
                            tap(() => this._appState.dispatch(CharacterGuildDivisionsActionFactory.beginFetchIdentities({
                                guildId
                            }))),
                            switchMap(() => this._apiClient.get<ICharacterGuildDivisionIdentityViewModel[]>(`${divisionsApiPath(guildId)}/identities`)),
                            catchError((error: HttpErrorResponse, caught) => (error.status === 404)
                                ? of([])
                                : caught),
                            tap(identities => this._appState.dispatch(CharacterGuildDivisionsActionFactory.storeIdentities({
                                guildId,
                                identities
                            }))))))
            .toPromise();
    }

    public fetchIdentity(
                guildId: number,
                divisionId: number):
            Promise<ICharacterGuildDivisionIdentityViewModel | null> {
        return this._appState.select(CharacterGuildDivisionsSelectors.identityIsFetching(guildId, divisionId))
            .pipe(
                take(1),
                switchMap(isFetching => isFetching
                    ? this._appState.select(CharacterGuildDivisionsSelectors.identity(guildId, divisionId))
                        .pipe(
                            skip(1),
                            take(1))
                    : of(null)
                        .pipe(
                            tap(() => this._appState.dispatch(CharacterGuildDivisionsActionFactory.beginFetchIdentity({
                                guildId,
                                divisionId
                            }))),
                            switchMap(() => this._apiClient.get<ICharacterGuildDivisionIdentityViewModel>(`${divisionApiPath(guildId, divisionId)}/identity`)),
                            catchError((error: HttpErrorResponse, caught) => (error.status === 404)
                                ? of(null)
                                : caught),
                            tap(identity => this._appState.dispatch(CharacterGuildDivisionsActionFactory.storeIdentity({
                                guildId,
                                identity
                            }))))))
            .toPromise();
    }

    public observeIdentities(
                guildId: number):
            Observable<ICharacterGuildDivisionIdentityViewModel[]> {
        if (this._observableIdentitiesMap[guildId] == null) {
            this._observableIdentitiesMap[guildId] = this._appState.select(CharacterGuildDivisionsSelectors.idsNeedingIdentityFetch(guildId))
                .pipe(
                    tap(divisionIds => setTimeout(() => (divisionIds === "all")
                        ? this.fetchIdentities(guildId)
                        : divisionIds.forEach(divisionId => this.fetchIdentity(guildId, divisionId)))),
                    switchMap(() => this._appState.select(CharacterGuildDivisionsSelectors.identities(guildId))),
                    shareReplay());
        }

        return this._observableIdentitiesMap[guildId];
    }

    public observeIdentity(
                guildId: number,
                divisionId: number):
            Observable<ICharacterGuildDivisionIdentityViewModel | null> {
        if (this._observableIdentityMap[guildId] == null) {
            this._observableIdentityMap[guildId] = {};
        }

        if (this._observableIdentityMap[guildId][divisionId] == null) {
            this._observableIdentityMap[guildId][divisionId] = this._appState.select(CharacterGuildDivisionsSelectors.identityNeedsFetch(guildId, divisionId))
                .pipe(
                    tap(needsFetch => setTimeout(() => needsFetch
                        ? this.fetchIdentity(guildId, divisionId)
                        : null)),
                    switchMap(() => this._appState.select(CharacterGuildDivisionsSelectors.identity(guildId, divisionId))),
                    shareReplay());
        }

        return this._observableIdentityMap[guildId][divisionId];
    }

    public update(
                guildId: number,
                divisionId: number,
                model: ICharacterGuildDivisionUpdateModel):
            Promise<IOperationError | null> {
        return this._apiClient.put<void>(divisionApiPath(guildId, divisionId), model)
            .pipe(
                tap(() => this._appState.dispatch(CharacterGuildDivisionsActionFactory.scheduleFetchIdentity({
                    guildId: guildId,
                    divisionId: divisionId
                }))),
                map(() => null),
                catchError((error: HttpErrorResponse, caught) => (Math.trunc(error.status / 100) === 4)
                    ? of(error.error)
                    : caught))
            .toPromise();
    }

    private readonly _apiClient: ApiClient;
    private readonly _appState: Store<IAppState>;
    private readonly _observableIdentitiesMap: { [guildId: number]: Observable<ICharacterGuildDivisionIdentityViewModel[]> };
    private readonly _observableIdentityMap: { [guildId: number]: { [divisionId: number]: Observable<ICharacterGuildDivisionIdentityViewModel | null> } };
}

@Injectable({
    providedIn: "root"
})
export class CharacterGuildsService {

    public constructor(
            apiClient: ApiClient,
            appState: Store<IAppState>) {

        this._apiClient = apiClient;
        this._appState = appState;

        this._observableIdentities = this._appState.select(CharacterGuildsSelectors.idsNeedingIdentityFetch)
            .pipe(
                tap(guildIds => setTimeout(() => (guildIds === "all")
                    ? this.fetchIdentities()
                    : guildIds.forEach(guildId => this.fetchIdentity(guildId)))),
                switchMap(() => this._appState.select(CharacterGuildsSelectors.identities)),
                shareReplay());

        this._observableIdentityMap = {};
    }

    public create(
                model: ICharacterGuildCreationModel):
            Promise<ICharacterGuildCreationResult> {
        return this._apiClient.post<number>(`${guildsApiPath}/new`, model)
            .pipe(
                map(guildId => ({
                    guildId
                })),
                tap(props => this._appState.dispatch(CharacterGuildsActionFactory.initialize(props))),
                catchError((error: HttpErrorResponse, caught) => (Math.trunc(error.status / 100) === 4)
                    ? of({
                        error: error.error
                    })
                    : caught))
            .toPromise();
    }

    public delete(
                guildId: number):
            Promise<void> {
        return this._apiClient.delete<void>(guildApiPath(guildId))
            .pipe(
                catchError((error: HttpErrorResponse, caught) => (error.status === 404)
                    ? of(null)
                    : caught),
                tap(() => this._appState.dispatch(CharacterGuildsActionFactory.remove({
                    guildId
                }))))
            .toPromise();
    }

    public fetchIdentities():
            Promise<ICharacterGuildIdentityViewModel[]> {
        return this._appState.select(CharacterGuildsSelectors.identitiesIsFetching)
            .pipe(
                take(1),
                switchMap(isFetching => isFetching
                    ? this._appState.select(CharacterGuildsSelectors.identities)
                        .pipe(
                            skip(1),
                            take(1))
                    : of(null)
                        .pipe(
                            tap(() => this._appState.dispatch(CharacterGuildsActionFactory.beginFetchIdentities())),
                            switchMap(() => this._apiClient.get<ICharacterGuildIdentityViewModel[]>(`${guildsApiPath}/identities`)),
                            catchError((error: HttpErrorResponse, caught) => (error.status === 404)
                                ? of([])
                                : caught),
                            tap(identities => this._appState.dispatch(CharacterGuildsActionFactory.storeIdentities({
                                identities
                            }))))))
            .toPromise();
    }

    public fetchIdentity(
                guildId: number):
            Promise<ICharacterGuildIdentityViewModel | null> {
        return this._appState.select(CharacterGuildsSelectors.identityIsFetching(guildId))
            .pipe(
                take(1),
                switchMap(isFetching => isFetching
                    ? this._appState.select(CharacterGuildsSelectors.identity(guildId))
                        .pipe(
                            skip(1),
                            take(1))
                    : of(null)
                        .pipe(
                            tap(() => this._appState.dispatch(CharacterGuildsActionFactory.beginFetchIdentity({
                                guildId
                            }))),
                            switchMap(() => this._apiClient.get<ICharacterGuildIdentityViewModel>(`${guildApiPath(guildId)}/identity`)),
                            catchError((error: HttpErrorResponse, caught) => (error.status === 404)
                                ? of(null)
                                : caught),
                            tap(identity => this._appState.dispatch(CharacterGuildsActionFactory.storeIdentity({
                                identity
                            }))))))
            .toPromise();
    }

    public observeIdentities():
            Observable<ICharacterGuildIdentityViewModel[]> {
        return this._observableIdentities;
    }

    public observeIdentity(
                guildId: number):
            Observable<ICharacterGuildIdentityViewModel | null> {
        if (this._observableIdentityMap[guildId] == null) {
            this._observableIdentityMap[guildId] = this._appState.select(CharacterGuildsSelectors.identityNeedsFetch(guildId))
                .pipe(
                    tap(needsFetch => setTimeout(() => needsFetch
                        ? this.fetchIdentity(guildId)
                        : null)),
                    switchMap(() => this._appState.select(CharacterGuildsSelectors.identity(guildId))),
                    shareReplay());
        }

        return this._observableIdentityMap[guildId];
    }

    public update(
                guildId: number,
                model: ICharacterGuildDivisionUpdateModel):
            Promise<IOperationError | null> {
        return this._apiClient.put<void>(guildApiPath(guildId), model)
            .pipe(
                tap(() => this._appState.dispatch(CharacterGuildsActionFactory.scheduleFetchIdentity({
                    guildId: guildId
                }))),
                map(() => null),
                catchError((error: HttpErrorResponse, caught) => (Math.trunc(error.status / 100) === 4)
                    ? of(error.error)
                    : caught))
            .toPromise();
    }

    private readonly _apiClient: ApiClient;
    private readonly _appState: Store<IAppState>;
    private readonly _observableIdentities: Observable<ICharacterGuildIdentityViewModel[]>;
    private readonly _observableIdentityMap: { [guildId: number]: Observable<ICharacterGuildIdentityViewModel | null> };
}
