import { createFeatureSelector, createSelector, MemoizedSelector } from "@ngrx/store";

import { FetchState } from "../common/fetching-utils";
import { IAppState } from "../state";

import {
    ICharacterGuildDivisionIdentityViewModel,
    ICharacterGuildIdentityViewModel
} from "./models";
import {
    initialCharacterGuildState,
    ICharacterGuildsState,
    ICharacterGuildDivisionsState,
    ICharacterGuildState} from "./state";


const guildsState
    = createFeatureSelector<IAppState, ICharacterGuildsState>("characterGuilds");

function divisionsState(
            guildId: number):
        MemoizedSelector<IAppState, ICharacterGuildDivisionsState> {
    return createSelector(
        guildState(guildId),
        state => state.divisions);
}
function guildState(
            guildId: number):
        MemoizedSelector<IAppState, ICharacterGuildState> {
    return createSelector(
        guildsState,
        state => state.stateTable[guildId] || initialCharacterGuildState);
}


export namespace CharacterGuildDivisionsSelectors {

    export function identities(
                guildId: number):
            MemoizedSelector<IAppState, ICharacterGuildDivisionIdentityViewModel[] | null> {
        return createSelector(
            divisionsState(guildId),
            state => state.identities.value);
    }
    export function identitiesIsFetching(
                guildId: number):
            MemoizedSelector<IAppState, boolean> {
        return createSelector(
            divisionsState(guildId),
            state => state.identities.state === FetchState.fetching);
    }
    export function identitiesNeedsFetch(
                guildId: number):
            MemoizedSelector<IAppState, boolean> {
        return createSelector(
            divisionsState(guildId),
            state => state.identities.state === FetchState.unfetched);
    }
    export function identity(
                guildId: number,
                divisionId: number):
            MemoizedSelector<IAppState, ICharacterGuildIdentityViewModel | null>{
        return createSelector(
            divisionsState(guildId),
            state => state.identities.value && state.identities.value.find(identity => identity.id === divisionId) || null);
    }
}

export namespace CharacterGuildsSelectors {

    export const identities: MemoizedSelector<IAppState, ICharacterGuildIdentityViewModel[] | null>
        = createSelector(
            guildsState,
            state => state.identities.value);

    export const identitiesIsFetching: MemoizedSelector<IAppState, boolean>
        = createSelector(
            guildsState,
            state => state.identities.state === FetchState.fetching);

    export const identitiesNeedsFetch: MemoizedSelector<IAppState, boolean>
        = createSelector(
            guildsState,
            state => state.identities.state === FetchState.unfetched);

    export function identity(
                guildId: number):
            MemoizedSelector<IAppState, ICharacterGuildIdentityViewModel | null>{
        return createSelector(
            guildsState,
            state => state.identities.value && state.identities.value.find(identity => identity.id === guildId) || null);
    }
}
