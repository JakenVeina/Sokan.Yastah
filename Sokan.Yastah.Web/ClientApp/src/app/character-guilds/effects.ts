import { HttpErrorResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";

import { Observable, of } from "rxjs";
import { catchError, concatMap, filter, map, switchMap, withLatestFrom } from "rxjs/operators";

import { Actions, createEffect, ofType } from "@ngrx/effects";
import { Action, Store } from "@ngrx/store";

import { IAppState } from "../state";

import { ApiClient } from "../api/api-client";

import { CharacterGuildsActionFactory } from "./actions";
import { ICharacterGuildDivisionIdentityViewModel, ICharacterGuildIdentityViewModel } from "./models";

@Injectable()
export class CharacterGuildsEffects {

    public constructor(
            actions: Actions,
            apiClient: ApiClient,
            appState: Store<IAppState>) {

        this.fetchDivisionIdentitiesEffect = createEffect(
            () => actions.pipe(
                ofType(CharacterGuildsActionFactory.beginFetchDivisionIdentities),
                switchMap(action => apiClient.get<ICharacterGuildDivisionIdentityViewModel[]>(`characters/guilds/${action.guildId}/divisions/identities`)
                    .pipe(
                        catchError((error: HttpErrorResponse, caught) => (error.status === 404)
                            ? of([])
                            : caught),
                        map(divisionIdentities => CharacterGuildsActionFactory.endFetchDivisionIdentities({
                            guildId: action.guildId,
                            divisionIdentities: divisionIdentities
                        }))))));

        this.fetchDivisionIdentityEffect = createEffect(
            () => actions.pipe(
                ofType(CharacterGuildsActionFactory.beginFetchDivisionIdentity),
                switchMap(action => apiClient.get<ICharacterGuildDivisionIdentityViewModel>(`characters/guilds/${action.guildId}/divisions/${action.divisionId}/identity`)
                    .pipe(
                        catchError((error: HttpErrorResponse, caught) => (error.status === 404)
                            ? of(null)
                            : caught),
                        map(divisionIdentity => (divisionIdentity == null)
                            ? CharacterGuildsActionFactory.removeDivisionIdentity({
                                    guildId: action.guildId,
                                    divisionId: action.divisionId
                                })
                            : CharacterGuildsActionFactory.endFetchDivisionIdentity({
                                    guildId: action.guildId,
                                    divisionIdentity: divisionIdentity
                                }))))));

        this.fetchIdentitiesEffect = createEffect(
            () => actions.pipe(
                ofType(CharacterGuildsActionFactory.beginFetchIdentities),
                switchMap(() => apiClient.get<ICharacterGuildIdentityViewModel[]>("characters/guilds/identities")
                    .pipe(map(identities => CharacterGuildsActionFactory.endFetchIdentities({
                        identities: identities
                    }))))));;

        this.fetchIdentityEffect = createEffect(
            () => actions.pipe(
                ofType(CharacterGuildsActionFactory.beginFetchIdentity),
                switchMap(action => apiClient.get<ICharacterGuildIdentityViewModel>(`characters/guilds/${action.guildId}/identity`)
                    .pipe(
                        catchError((error: HttpErrorResponse, caught) => (error.status === 404)
                            ? of(null)
                            : caught),
                        map(identity => (identity == null)
                            ? CharacterGuildsActionFactory.removeIdentity({
                                    guildId: action.guildId
                                })
                            : CharacterGuildsActionFactory.endFetchIdentity({
                                    identity: identity
                                }))))));

        this.tryFetchDivisionIdentitiesEffect = createEffect(
            () => actions.pipe(
                ofType(CharacterGuildsActionFactory.tryFetchDivisionIdentities),
                concatMap(action => of(action)
                    .pipe(withLatestFrom(appState.select(x => x.characterGuilds.divisionIdentities[action.guildId])
                        .pipe(filter(divisionIdentities =>
                            (((divisionIdentities == null) || divisionIdentities.needsFetched) && !divisionIdentities.isFetching)
                            || Object.values(divisionIdentities).some(divisionIdentity =>
                                (!divisionIdentity.isFetching
                                    && divisionIdentity.needsFetched))))))),
                map(([action]) => CharacterGuildsActionFactory.beginFetchDivisionIdentities({
                    guildId: action.guildId,
                }))));

        this.tryFetchDivisionIdentityEffect = createEffect(
            () => actions.pipe(
                ofType(CharacterGuildsActionFactory.tryFetchDivisionIdentity),
                concatMap(action => of(action)
                    .pipe(withLatestFrom(appState.select(x => x.characterGuilds.divisionIdentities[action.guildId])
                        .pipe(filter(divisionIdentities => (divisionIdentities == null)
                            || (!divisionIdentities.isFetching
                                && ((divisionIdentities.value[action.divisionId] == null)
                                    || (!divisionIdentities.value[action.divisionId].isFetching
                                        && divisionIdentities.value[action.divisionId].needsFetched)))))))),
                map(([action]) => CharacterGuildsActionFactory.beginFetchDivisionIdentity({
                    guildId: action.guildId,
                    divisionId: action.divisionId
                }))));

        this.tryFetchIdentitiesEffect = createEffect(
            () => actions.pipe(
                ofType(CharacterGuildsActionFactory.tryFetchIdentities),
                concatMap(() => appState.select(x => x.characterGuilds.identities)
                    .pipe(filter(identities => identities.needsFetched))),
                map(() => CharacterGuildsActionFactory.beginFetchIdentities())));

        this.tryFetchIdentityEffect = createEffect(
            () => actions.pipe(
                ofType(CharacterGuildsActionFactory.tryFetchIdentity),
                concatMap(action => of(action)
                    .pipe(withLatestFrom(appState.select(x => x.characterGuilds.identities)
                        .pipe(filter(identities => (identities == null)
                            || (!identities.isFetching
                                && ((identities.value[action.guildId] == null)
                                    || (!identities.value[action.guildId].isFetching
                                        && identities.value[action.guildId].needsFetched)))))))),
                map(([action]) => CharacterGuildsActionFactory.beginFetchIdentity({
                    guildId: action.guildId
                }))));
    }

    public readonly fetchDivisionIdentitiesEffect: Observable<Action>;
    public readonly fetchDivisionIdentityEffect: Observable<Action>;
    public readonly fetchIdentitiesEffect: Observable<Action>;
    public readonly fetchIdentityEffect: Observable<Action>;
    public readonly tryFetchDivisionIdentitiesEffect: Observable<Action>;
    public readonly tryFetchDivisionIdentityEffect: Observable<Action>;
    public readonly tryFetchIdentitiesEffect: Observable<Action>;
    public readonly tryFetchIdentityEffect: Observable<Action>;
}
