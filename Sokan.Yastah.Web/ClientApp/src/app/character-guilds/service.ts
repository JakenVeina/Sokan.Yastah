import { Injectable } from "@angular/core";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs";
import { map, share, shareReplay, tap } from "rxjs/operators";

import { ApiClient } from "../api/api-client";
import { IAppState } from "../state";

import { CharacterGuildsActionFactory } from "./actions";
import {
    ICharacterGuildCreationModel,
    ICharacterGuildDivisionCreationModel,
    ICharacterGuildDivisionIdentityViewModel,
    ICharacterGuildIdentityViewModel,
    ICharacterGuildUpdateModel
} from "./models";

@Injectable({
    providedIn: "root"
})
export class CharacterGuildsService {

    public constructor(
            apiClient: ApiClient,
            appState: Store<IAppState>) {
        this._appState = appState;
        this._apiClient = apiClient;

        this._identities = this._appState
            .select(state => state.characterGuilds.identities)
            .pipe(
                tap(identities => {
                    if (identities.needsFetched) {
                        setTimeout(() => this._appState.dispatch(CharacterGuildsActionFactory.tryFetchIdentities()));
                    }
                }),
                map(identities => Object.values(identities.value)
                    .map(identity => identity.value)
                    .filter(identity => identity != null)),
                shareReplay({
                    refCount: true
                }));

        this._divisionIdentitiesObservations = {};
        this._divisionIdentityObservations = {};
        this._identityObservations = {};
    }

    public get identities(): Observable<ICharacterGuildIdentityViewModel[]> {
        return this._identities;
    }

    public create(
                model: ICharacterGuildCreationModel):
            Observable<number> {
        return this._apiClient.post<number>("characters/guilds/new", model)
            .pipe(
                tap(guildId => this._appState.dispatch(CharacterGuildsActionFactory.scheduleFetchIdentity({
                    guildId: guildId
                }))));
    }

    public createDivision(
                guildId: number,
                model: ICharacterGuildDivisionCreationModel):
            Observable<number> {
        return this._apiClient.post<number>(`characters/guilds/${guildId}/divisions/new`, model)
            .pipe(
                tap(divisionId => this._appState.dispatch(CharacterGuildsActionFactory.scheduleFetchDivisionIdentity({
                    guildId: guildId,
                    divisionId: divisionId
                }))));
    }

    public delete(
                guildId: number):
            Observable<void> {
        return this._apiClient.delete<void>(`characters/guilds/${guildId}`)
            .pipe(
                tap(() => this._appState.dispatch(CharacterGuildsActionFactory.removeIdentity({
                    guildId: guildId
                }))));
    }

    public deleteDivision(
                guildId: number,
                divisionId: number):
            Observable<void> {
        return this._apiClient.delete<void>(`characters/guilds/${guildId}/divisions/${divisionId}`)
            .pipe(
                tap(() => this._appState.dispatch(CharacterGuildsActionFactory.removeDivisionIdentity({
                    guildId: guildId,
                    divisionId: divisionId
                }))));
    }

    public observeDivisionIdentities(
                guildId: number):
            Observable<ICharacterGuildDivisionIdentityViewModel[]> {
        if (this._divisionIdentitiesObservations[guildId] == null) {
            this._divisionIdentitiesObservations[guildId] = this._appState
                .select(state => state.characterGuilds.divisionIdentities[guildId])
                .pipe(
                    tap(divisionIdentities => {
                        if (divisionIdentities.needsFetched) {
                            setTimeout(() => this._appState.dispatch(CharacterGuildsActionFactory.tryFetchDivisionIdentities({
                                guildId: guildId
                            })));
                        }
                    }),
                    map(divisionIdentities => Object.values(divisionIdentities.value)
                        .map(divisionIdentity => divisionIdentity.value)
                        .filter(divisionIdentity => divisionIdentity != null)),
                    shareReplay({
                        refCount: true
                    }));
        }

        return this._divisionIdentitiesObservations[guildId];
    }

    public observeDivisionIdentity(
                guildId: number,
                divisionId: number):
            Observable<ICharacterGuildDivisionIdentityViewModel> {
        if (this._divisionIdentityObservations[guildId] == null) {
            this._divisionIdentityObservations[guildId] = {};
        }

        if (this._divisionIdentityObservations[guildId][divisionId]== null) {
            this._divisionIdentityObservations[guildId][divisionId] = this._appState
                .select(state => state.characterGuilds.divisionIdentities[guildId]
                    && state.characterGuilds.divisionIdentities[guildId].value[divisionId])
                .pipe(
                    tap(divisionIdentity => {
                        if ((divisionIdentity == null) || divisionIdentity.needsFetched) {
                            setTimeout(() => this._appState.dispatch(CharacterGuildsActionFactory.tryFetchDivisionIdentity({
                                guildId: guildId,
                                divisionId: divisionId
                            })));
                        }
                    }),
                    map(divisionIdentity => divisionIdentity && divisionIdentity.value),
                    shareReplay({
                        refCount: true
                    }));
        }

        return this._divisionIdentityObservations[guildId][divisionId];
    }

    public observeIdentity(
                guildId: number):
            Observable<ICharacterGuildIdentityViewModel> {
        if (this._identityObservations[guildId] == null) {
            this._identityObservations[guildId] = this._appState
                .select(state => state.characterGuilds.identities.value[guildId])
                .pipe(
                    tap(identity => {
                        if ((identity == null) || identity.needsFetched) {
                            setTimeout(() => this._appState.dispatch(CharacterGuildsActionFactory.tryFetchIdentity({
                                guildId: guildId
                            })));
                        }
                    }),
                    map(identity => identity && identity.value),
                    shareReplay({
                        refCount: true
                    }));
        }

        return this._identityObservations[guildId];
    }

    public reloadDivisionIdentities(
                guildId: number):
            void {
        this._appState.dispatch(CharacterGuildsActionFactory.scheduleFetchDivisionIdentities({
            guildId: guildId
        }));
    }

    public reloadDivisionIdentity(
                guildId: number,
                divisionId: number):
            void {
        this._appState.dispatch(CharacterGuildsActionFactory.scheduleFetchDivisionIdentity({
            guildId: guildId,
            divisionId: divisionId
        }));
    }

    public reloadIdentities():
            void {
        this._appState.dispatch(CharacterGuildsActionFactory.scheduleFetchIdentities());
    }

    public reloadIdentity(
                guildId: number):
            void {
        this._appState.dispatch(CharacterGuildsActionFactory.scheduleFetchIdentity({
            guildId: guildId
        }));
    }

    public update(
                guildId: number,
                model: ICharacterGuildUpdateModel):
            Observable<void> {
        return this._apiClient.put<void>(`characters/guilds/${guildId}`, model)
            .pipe(
                tap(() => this._appState.dispatch(CharacterGuildsActionFactory.scheduleFetchIdentity({
                    guildId: guildId
                }))));
    }

    public updateDivision(
                guildId: number,
                divisionId: number,
                model: ICharacterGuildUpdateModel):
            Observable<void> {
        return this._apiClient.put<void>(`characters/guilds/${guildId}/divisions/${divisionId}`, model)
            .pipe(
                tap(() => this._appState.dispatch(CharacterGuildsActionFactory.scheduleFetchDivisionIdentity({
                    guildId: guildId,
                    divisionId: divisionId
                }))));
    }

    private readonly _apiClient: ApiClient;
    private readonly _appState: Store<IAppState>;
    private readonly _divisionIdentitiesObservations: { [guildId: number]: Observable<ICharacterGuildDivisionIdentityViewModel[]> };
    private readonly _divisionIdentityObservations: { [guildId: number]: { [divisionId: number]: Observable<ICharacterGuildDivisionIdentityViewModel> } };
    private readonly _identityObservations: { [guildId: number]: Observable<ICharacterGuildIdentityViewModel> };
    private readonly _identities: Observable<ICharacterGuildIdentityViewModel[]>;
}
