import { createFeatureSelector, createSelector, MemoizedSelector } from "@ngrx/store";

import { FetchedValueTable, FetchState } from "../common/fetching-utils";
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


export namespace CharacterGuildsSelectors {

    export const identities: MemoizedSelector<IAppState, ICharacterGuildIdentityViewModel[]>
        = createSelector(
            guildsState,
            state => Object.values(
                    state.identities.valueTable)
                .map(row => row.value)
                .filter(identity => identity != null));

    export const identitiesIsFetching: MemoizedSelector<IAppState, boolean>
        = createSelector(
            guildsState,
            state => state.identities.state === FetchState.fetching);

    export const idsNeedingIdentityFetch: MemoizedSelector<IAppState, "all" | number[]>
        = createSelector(
            guildsState,
            state => FetchedValueTable.allRowsNeedFetch(state.identities) ? "all"
                : FetchedValueTable.keysNeedingFetch(state.identities)
                    .map(guildId => Number(guildId)));

    export function identity(
                guildId: number):
            MemoizedSelector<IAppState, ICharacterGuildIdentityViewModel | null>{
        return createSelector(
            guildsState,
            state => state.identities.valueTable[guildId] && state.identities.valueTable[guildId].value);
    }
    export function identityIsFetching(
                guildId: number):
            MemoizedSelector<IAppState, boolean> {
        return createSelector(
            guildsState,
            state => (state.identities.valueTable[guildId] && state.identities.valueTable[guildId].state === FetchState.fetching) || false);
    }
    export function identityNeedsFetch(
                guildId: number):
            MemoizedSelector<IAppState, boolean> {
        return createSelector(
            guildsState,
            state => FetchedValueTable.rowNeedsFetched(state.identities, guildId));
    }
}

export namespace CharacterGuildDivisionsSelectors {

    export function identities(
                guildId: number):
            MemoizedSelector<IAppState, ICharacterGuildDivisionIdentityViewModel[]> {
        return createSelector(
            divisionsState(guildId),
            state => Object.values(state.identities.valueTable)
                .map(row => row.value)
                .filter(identity =>  identity != null));
    }
    export function identitiesIsFetching(
                guildId: number):
            MemoizedSelector<IAppState, boolean> {
        return createSelector(
            divisionsState(guildId),
            state => state.identities.state === FetchState.fetching);
    }
    export function identity(
                guildId: number,
                divisionId: number):
            MemoizedSelector<IAppState, ICharacterGuildIdentityViewModel | null>{
        return createSelector(
            divisionsState(guildId),
            state => state.identities.valueTable[divisionId] && state.identities.valueTable[divisionId].value);
    }
    export function identityIsFetching(
                guildId: number,
                divisionId: number):
            MemoizedSelector<IAppState, boolean> {
        return createSelector(
            divisionsState(guildId),
            state => (state.identities.valueTable[divisionId] && state.identities.valueTable[divisionId].state === FetchState.fetching) || false);
    }
    export function identityNeedsFetch(
                guildId: number,
                divisionId: number):
            MemoizedSelector<IAppState, boolean> {
        return createSelector(
            divisionsState(guildId),
            state => FetchedValueTable.rowNeedsFetched(state.identities, divisionId));
    }
    export function idsNeedingIdentityFetch(
                guildId: number):
            MemoizedSelector<IAppState, "all" | number[]> {
        return createSelector(
            divisionsState(guildId),
            state => FetchedValueTable.allRowsNeedFetch(state.identities) ? "all"
                : FetchedValueTable.keysNeedingFetch(state.identities)
                    .map(guildId => Number(guildId)));
    }
}
