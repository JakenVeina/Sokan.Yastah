import { Action, createReducer, on } from "@ngrx/store";

import {
    FetchedValue,
    FetchState
} from "../common/fetching-utils";
import { ImmutableObject } from "../common/immutable-utils";

import { CharacterGuildsActionFactory, CharacterGuildDivisionsActionFactory } from "./state.actions";
import {
    initialCharacterGuildsState,
    initialCharacterGuildState,
    CharacterGuildsState,
    CharacterGuildDivisionsState,
    CharacterGuildState,
    ICharacterGuildsState
} from "./state";


const _characterGuildsStateReducer
    = createReducer<ICharacterGuildsState>(
        initialCharacterGuildsState,
        on(
            CharacterGuildDivisionsActionFactory.beginFetchIdentities,
            (state, action) => CharacterGuildsState.mapStateTable(state,
                stateTable => ImmutableObject.mapOne(stateTable, action.guildId,
                    row => CharacterGuildState.mapDivisions(row || initialCharacterGuildState,
                        divisions => CharacterGuildDivisionsState.mapIdentities(divisions,
                            identities => FetchedValue.updateState(identities,
                                FetchState.fetching)))))),
        on(
            CharacterGuildDivisionsActionFactory.scheduleFetchIdentities,
            (state, action) => CharacterGuildsState.mapStateTable(state,
                stateTable => ImmutableObject.mapOne(stateTable, action.guildId,
                    row => CharacterGuildState.mapDivisions(row || initialCharacterGuildState,
                        divisions => CharacterGuildDivisionsState.mapIdentities(divisions,
                            identities => (identities.state === FetchState.fetching)
                                ? identities
                                : FetchedValue.updateState(identities,
                                    FetchState.unfetched)))))),
        on(
            CharacterGuildDivisionsActionFactory.storeIdentities,
            (state, action) => CharacterGuildsState.mapStateTable(state,
                stateTable => ImmutableObject.mapOne(stateTable, action.guildId,
                    row => CharacterGuildState.mapDivisions(row || initialCharacterGuildState,
                        divisions => CharacterGuildDivisionsState.updateIdentities(divisions,
                            FetchedValue.fetched(action.identities)))))),
        on(
            CharacterGuildsActionFactory.beginFetchIdentities,
            state => CharacterGuildsState.mapIdentities(state,
                identities => FetchedValue.updateState(identities,
                    FetchState.fetching))),
        on(
            CharacterGuildsActionFactory.remove,
            (state, action) => CharacterGuildsState.mapStateTable(state,
                stateTable => ImmutableObject.removeOne(stateTable, action.guildId))),
        on(
            CharacterGuildsActionFactory.scheduleFetchIdentities,
            state => CharacterGuildsState.mapIdentities(state,
                identities => (identities.state === FetchState.fetching)
                    ? identities
                    : FetchedValue.updateState(identities,
                        FetchState.unfetched))),
        on(
            CharacterGuildsActionFactory.storeIdentities,
            (state, action) => CharacterGuildsState.updateIdentities(state,
                FetchedValue.fetched(action.identities))));


export function characterGuildsStateReducer(
            state: ICharacterGuildsState | undefined,
            action: Action):
        ICharacterGuildsState {
    return _characterGuildsStateReducer(state, action);
}
