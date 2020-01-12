import { Action, createReducer, on } from "@ngrx/store";

import {
    FetchedValue,
    FetchState
} from "../../common/fetching-utils";
import { ImmutableObject } from "../../common/immutable-utils";

import { GuildsActionFactory, GuildDivisionsActionFactory } from "./state.actions";
import {
    initialGuildsState,
    initialGuildState,
    GuildsState,
    GuildDivisionsState,
    GuildState,
    IGuildsState
} from "./state";


const _guildsStateReducer
    = createReducer<IGuildsState>(
        initialGuildsState,
        on(
            GuildDivisionsActionFactory.beginFetchIdentities,
            (state, action) => GuildsState.mapStateTable(state,
                stateTable => ImmutableObject.mapOne(stateTable, action.guildId,
                    row => GuildState.mapDivisions(row || initialGuildState,
                        divisions => GuildDivisionsState.mapIdentities(divisions,
                            identities => FetchedValue.updateState(identities,
                                FetchState.fetching)))))),
        on(
            GuildDivisionsActionFactory.scheduleFetchIdentities,
            (state, action) => GuildsState.mapStateTable(state,
                stateTable => ImmutableObject.mapOne(stateTable, action.guildId,
                    row => GuildState.mapDivisions(row || initialGuildState,
                        divisions => GuildDivisionsState.mapIdentities(divisions,
                            identities => (identities.state === FetchState.fetching)
                                ? identities
                                : FetchedValue.updateState(identities,
                                    FetchState.unfetched)))))),
        on(
            GuildDivisionsActionFactory.storeIdentities,
            (state, action) => GuildsState.mapStateTable(state,
                stateTable => ImmutableObject.mapOne(stateTable, action.guildId,
                    row => GuildState.mapDivisions(row || initialGuildState,
                        divisions => GuildDivisionsState.updateIdentities(divisions,
                            FetchedValue.fetched(action.identities)))))),
        on(
            GuildsActionFactory.beginFetchIdentities,
            state => GuildsState.mapIdentities(state,
                identities => FetchedValue.updateState(identities,
                    FetchState.fetching))),
        on(
            GuildsActionFactory.remove,
            (state, action) => GuildsState.mapStateTable(state,
                stateTable => ImmutableObject.removeOne(stateTable, action.guildId))),
        on(
            GuildsActionFactory.scheduleFetchIdentities,
            state => GuildsState.mapIdentities(state,
                identities => (identities.state === FetchState.fetching)
                    ? identities
                    : FetchedValue.updateState(identities,
                        FetchState.unfetched))),
        on(
            GuildsActionFactory.storeIdentities,
            (state, action) => GuildsState.updateIdentities(state,
                FetchedValue.fetched(action.identities))));


export function guildsStateReducer(
            state: IGuildsState | undefined,
            action: Action):
        IGuildsState {
    return _guildsStateReducer(state, action);
}
