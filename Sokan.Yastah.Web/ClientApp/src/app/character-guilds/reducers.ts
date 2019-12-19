import { Action, createReducer, on } from "@ngrx/store";

import { FetchedValue } from "../state";

import { CharacterGuildsActionFactory } from "./actions";
import { initialCharacterGuildsState, ICharacterGuildsState } from "./state";

const _characterGuildsStateReducer
    = createReducer<ICharacterGuildsState>(
        initialCharacterGuildsState,
        on(
            CharacterGuildsActionFactory.beginFetchDivisionIdentities,
            (state, action) => ({
                ...state,
                divisionIdentities: {
                    ...state.divisionIdentities,
                    [action.guildId]: FetchedValue.fetching(
                        Object.entries(state.divisionIdentities[action.guildId].value)
                            .reduce((value, [divisionId, divisionIdentity]) => {
                                value[divisionId] = FetchedValue.fetching(divisionIdentity.value);
                                return value;
                            }, {}))
                }
            })),
        on(
            CharacterGuildsActionFactory.beginFetchDivisionIdentity,
            (state, action) => ({
                ...state,
                divisionIdentities: {
                    ...state.divisionIdentities,
                    [action.guildId]: {
                        ...state.divisionIdentities[action.guildId],
                        value: {
                            ...state.divisionIdentities[action.guildId].value,
                            [action.divisionId]: FetchedValue.fetching(
                                state.divisionIdentities[action.guildId].value[action.divisionId]
                                    && state.divisionIdentities[action.guildId].value[action.divisionId].value)
                        }
                    }
                }
            })),
        on(
            CharacterGuildsActionFactory.beginFetchIdentities,
            (state) => ({
                ...state,
                identities: FetchedValue.fetching(
                    Object.entries(state.identities.value)
                        .reduce((value, [guildId, identity]) => {
                            value[guildId] = FetchedValue.fetching(identity.value);
                            return value;
                        }, {}))
            })),
        on(
            CharacterGuildsActionFactory.beginFetchIdentity,
            (state, action) => ({
                ...state,
                identities: {
                    ...state.identities,
                    value: {
                        ...state.identities.value,
                        [action.guildId]: FetchedValue.fetching(
                            state.identities.value[action.guildId]
                                && state.identities.value[action.guildId].value)
                    }
                }
            })),
        on(
            CharacterGuildsActionFactory.removeDivisionIdentity,
            (state, action) => ({
                ...state,
                divisionIdentities: {
                    ...state.divisionIdentities,
                    [action.guildId]: {
                        ...state.divisionIdentities[action.guildId],
                        value: {
                            ...state.divisionIdentities[action.guildId].value,
                            [action.divisionId]: FetchedValue.fetched(null)
                        }
                    }
                }
            })),
        on(
            CharacterGuildsActionFactory.removeIdentity,
            (state, action) => ({
                ...state,
                identities: {
                    ...state.identities,
                    value: {
                        ...state.identities.value,
                        [action.guildId]: FetchedValue.fetched(null)
                    }
                },
                divisionIdentities: {
                    ...state.divisionIdentities,
                    [action.guildId]: FetchedValue.fetched({}),
                }
            })),
        on(
            CharacterGuildsActionFactory.scheduleFetchDivisionIdentities,
            (state, action) => ({
                ...state,
                divisionIdentities: {
                    ...state.divisionIdentities,
                    [action.guildId]: FetchedValue.unfetched(
                        (state.divisionIdentities[action.guildId]
                            && state.divisionIdentities[action.guildId].value)
                        || {})
                }
            })),
        on(
            CharacterGuildsActionFactory.scheduleFetchDivisionIdentity,
            (state, action) => ({
                ...state,
                divisionIdentities: {
                    ...state.divisionIdentities,
                    [action.guildId]: {
                        ...state.divisionIdentities[action.guildId],
                        value: {
                            ...state.divisionIdentities[action.guildId].value,
                            [action.divisionId]: FetchedValue.unfetched(
                                state.divisionIdentities[action.guildId].value[action.divisionId]
                                    && state.divisionIdentities[action.guildId].value[action.divisionId].value)
                        }
                    }
                }
            })),
        on(
            CharacterGuildsActionFactory.scheduleFetchIdentities,
            (state) => ({
                ...state,
                identities: FetchedValue.unfetched(
                    state.identities.value)
            })),
        on(
            CharacterGuildsActionFactory.scheduleFetchIdentity,
            (state, action) => ({
                ...state,
                identities: {
                    ...state.identities,
                    value: {
                        ...state.identities.value,
                        [action.guildId]: FetchedValue.unfetched(
                            state.identities.value[action.guildId]
                                && state.identities.value[action.guildId].value)
                    }
                }
            })),
        on(
            CharacterGuildsActionFactory.endFetchDivisionIdentities,
            (state, action) => ({
                ...state,
                divisionIdentities: {
                    ...state.divisionIdentities,
                    [action.guildId]: FetchedValue.fetched(
                        action.divisionIdentities.reduce(
                            (value, divisionIdentity) => {
                                value[divisionIdentity.id] = FetchedValue.fetched(divisionIdentity);
                                return value;
                            },
                            {}))
                }
            })),
        on(
            CharacterGuildsActionFactory.endFetchDivisionIdentity,
            (state, action) => ({
                ...state,
                divisionIdentities: {
                    ...state.divisionIdentities,
                    [action.guildId]: {
                        ...state.divisionIdentities[action.guildId],
                        value: {
                            ...state.divisionIdentities[action.guildId].value,
                            [action.divisionIdentity.id]: FetchedValue.fetched(action.divisionIdentity)
                        }
                    }
                }
            })),
        on(
            CharacterGuildsActionFactory.endFetchIdentities,
            (state, action) => ({
                ...state,
                identities: FetchedValue.fetched(
                    action.identities.reduce(
                        (value, identity) => {
                            value[identity.id] = FetchedValue.fetched(identity)
                            return value;
                        },
                        {}))
            })),
        on(
            CharacterGuildsActionFactory.endFetchIdentity,
            (state, action) => ({
                ...state,
                identities: {
                    ...state.identities,
                    value: {
                        ...state.identities.value,
                        [action.identity.id]: FetchedValue.fetched(action.identity)
                    }
                }
            })),
    )

export function characterGuildsStateReducer(
        state: ICharacterGuildsState | undefined,
        action: Action) {
    return _characterGuildsStateReducer(state, action);
}
