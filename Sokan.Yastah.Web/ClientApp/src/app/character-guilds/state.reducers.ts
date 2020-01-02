import { Action, createReducer, on } from "@ngrx/store";

import {
    deletedFetchedValueState,
    initialFetchedValueState,
    FetchState,
    FetchedValueTable
} from "../common/fetching-utils";
import { ImmutableObject } from "../common/immutable-utils";

import { CharacterGuildsActionFactory, CharacterGuildDivisionsActionFactory } from "./state.actions";
import {
    deletedCharacterGuildState,
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
                            identities => FetchedValueTable.updateAllStates(identities, FetchState.fetching)))))),
        on(
            CharacterGuildDivisionsActionFactory.beginFetchIdentity,
            (state, action) => CharacterGuildsState.mapStateTable(state,
                stateTable => ImmutableObject.mapOne(stateTable, action.guildId,
                    row => CharacterGuildState.mapDivisions(row || initialCharacterGuildState,
                        divisions => CharacterGuildDivisionsState.mapIdentities(divisions,
                            identities => FetchedValueTable.updateOneState(identities, action.divisionId, FetchState.fetching)))))),
        on(
            CharacterGuildDivisionsActionFactory.initialize,
            (state, action) => CharacterGuildsState.mapStateTable(state,
                stateTable => ImmutableObject.mapOne(stateTable, action.guildId,
                    row => CharacterGuildState.mapDivisions(row || initialCharacterGuildState,
                        divisions => CharacterGuildDivisionsState.mapIdentities(divisions,
                            identities => FetchedValueTable.updateOneRow(identities, action.divisionId, initialFetchedValueState)))))),
        on(
            CharacterGuildDivisionsActionFactory.remove,
            (state, action) => CharacterGuildsState.mapStateTable(state,
                stateTable => ImmutableObject.mapOne(stateTable, action.guildId,
                    row => CharacterGuildState.mapDivisions(row || initialCharacterGuildState,
                        divisions => CharacterGuildDivisionsState.mapIdentities(divisions,
                            identities => FetchedValueTable.updateOneRow(identities, action.divisionId, deletedFetchedValueState)))))),
        on(
            CharacterGuildDivisionsActionFactory.scheduleFetchIdentities,
            (state, action) => CharacterGuildsState.mapStateTable(state,
                stateTable => ImmutableObject.mapOne(stateTable, action.guildId,
                    row => CharacterGuildState.mapDivisions(row || initialCharacterGuildState,
                        divisions => CharacterGuildDivisionsState.mapIdentities(divisions,
                            identities => (identities.state === FetchState.fetching)
                                ? identities
                                : FetchedValueTable.updateAllStates(identities, FetchState.unfetched)))))),
        on(
            CharacterGuildDivisionsActionFactory.scheduleFetchIdentity,
            (state, action) => CharacterGuildsState.mapStateTable(state,
                stateTable => ImmutableObject.mapOne(stateTable, action.guildId,
                    row => CharacterGuildState.mapDivisions(row || initialCharacterGuildState,
                        divisions => CharacterGuildDivisionsState.mapIdentities(divisions,
                            identities => FetchedValueTable.mapOneState(identities, action.divisionId,
                                state => (state === FetchState.fetching)
                                    ? FetchState.fetching
                                    : FetchState.unfetched)))))),
        on(
            CharacterGuildDivisionsActionFactory.storeIdentities,
            (state, action) => CharacterGuildsState.mapStateTable(state,
                stateTable => ImmutableObject.mapOne(stateTable, action.guildId,
                    row => CharacterGuildState.mapDivisions(row || initialCharacterGuildState,
                        divisions => CharacterGuildDivisionsState.updateIdentities(divisions,
                            FetchedValueTable.fromValues(FetchState.fetched, action.identities, identity => identity.id)))))),
        on(
            CharacterGuildDivisionsActionFactory.storeIdentity,
            (state, action) => CharacterGuildsState.mapStateTable(state,
                stateTable => ImmutableObject.mapOne(stateTable, action.guildId,
                    row => CharacterGuildState.mapDivisions(row || initialCharacterGuildState,
                        divisions => CharacterGuildDivisionsState.mapIdentities(divisions,
                            identities => FetchedValueTable.updateOneRow(identities, action.identity.id, {
                                state: FetchState.fetched,
                                value: action.identity
                            })))))),
        on(
            CharacterGuildsActionFactory.beginFetchIdentities,
            state => CharacterGuildsState.mapIdentities(state,
                identities => FetchedValueTable.updateAllStates(identities, FetchState.fetching))),
        on(
            CharacterGuildsActionFactory.beginFetchIdentity,
            (state, action) => CharacterGuildsState.mapIdentities(state,
                identities => FetchedValueTable.updateOneState(identities, action.guildId, FetchState.fetching))),
        on(
            CharacterGuildsActionFactory.initialize,
            (state, action) => CharacterGuildsState.mapIdentitiesAndStateTable(state,
                identities => FetchedValueTable.updateOneRow(identities, action.guildId, initialFetchedValueState),
                stateTable => ImmutableObject.updateOne(stateTable, action.guildId, initialCharacterGuildState))),
        on(
            CharacterGuildsActionFactory.remove,
            (state, action) => CharacterGuildsState.mapIdentitiesAndStateTable(state,
                identities => FetchedValueTable.updateOneRow(identities, action.guildId, deletedFetchedValueState),
                stateTable => ImmutableObject.updateOne(stateTable, action.guildId, deletedCharacterGuildState))),
        on(
            CharacterGuildsActionFactory.scheduleFetchIdentities,
            state => CharacterGuildsState.mapIdentities(state,
                identities => (identities.state === FetchState.fetching)
                    ? identities
                    : FetchedValueTable.updateAllStates(identities, FetchState.unfetched))),
        on(
            CharacterGuildsActionFactory.scheduleFetchIdentity,
            (state, action) => CharacterGuildsState.mapIdentities(state,
                identities => FetchedValueTable.mapOneState(identities, action.guildId,
                    state => (state === FetchState.fetching)
                        ? FetchState.fetching
                        : FetchState.unfetched))),
        on(
            CharacterGuildsActionFactory.storeIdentities,
            (state, action) => CharacterGuildsState.updateIdentities(state,
                FetchedValueTable.fromValues(FetchState.fetched, action.identities, identity => identity.id))),
        on(
            CharacterGuildsActionFactory.storeIdentity,
            (state, action) => CharacterGuildsState.mapIdentities(state,
                identities => FetchedValueTable.updateOneRow(identities, action.identity.id, {
                    state: FetchState.fetched,
                    value: action.identity
                }))));


export function characterGuildsStateReducer(
        state: ICharacterGuildsState | undefined,
        action: Action) {
    return _characterGuildsStateReducer(state, action);
}
