import { deletedFetchedValueTableState, initialFetchedValueTableState, IFetchedValueTable } from "../common/fetching-utils";
import { ImmutableObject } from "../common/immutable-utils";
import { Reducer, INativeHashTable } from "../common/types";

import {
    ICharacterGuildDivisionIdentityViewModel,
    ICharacterGuildIdentityViewModel
} from "./models";


export interface ICharacterGuildsState {
    readonly identities: IFetchedValueTable<ICharacterGuildIdentityViewModel | null>;
    readonly stateTable: Readonly<INativeHashTable<ICharacterGuildState | null>>;
}
export interface ICharacterGuildState {
    readonly divisions: ICharacterGuildDivisionsState;
}
export interface ICharacterGuildDivisionsState {
    readonly identities: IFetchedValueTable<ICharacterGuildDivisionIdentityViewModel | null>;
}


export const deletedCharacterGuildState: ICharacterGuildState = {
    divisions: {
        identities: deletedFetchedValueTableState
    }
};
export const initialCharacterGuildsState: ICharacterGuildsState = {
    identities: initialFetchedValueTableState,
    stateTable: {}
};
export const initialCharacterGuildState: ICharacterGuildState = {
    divisions: {
        identities: initialFetchedValueTableState
    }
};


export namespace CharacterGuildsState {

    export function mapIdentities(
                state: ICharacterGuildsState,
                identitiesReducer: Reducer<ICharacterGuildsState["identities"]>):
            ICharacterGuildsState {
        return ImmutableObject.mapOne(state, "identities", identitiesReducer);
    }

    export function mapIdentitiesAndStateTable(
                state: ICharacterGuildsState,
                identitiesReducer: Reducer<ICharacterGuildsState["identities"]>,
                stateTableReducer: Reducer<ICharacterGuildsState["stateTable"]>):
            ICharacterGuildsState {
        return ImmutableObject.mapOrUpdateMany(state,
            {
                key: "identities",
                reducer: identitiesReducer
            },
            {
                key: "stateTable",
                reducer: stateTableReducer
            });
    }

    export function mapStateTable(
                state: ICharacterGuildsState,
                stateTableReducer: Reducer<ICharacterGuildsState["stateTable"]>):
            ICharacterGuildsState {
        return ImmutableObject.mapOne(state, "stateTable", stateTableReducer);
    }

    export function updateIdentities(
                state: ICharacterGuildsState,
                identities: ICharacterGuildsState["identities"]):
            ICharacterGuildsState {
        return ImmutableObject.updateOne(state, "identities", identities);
    }
}

export namespace CharacterGuildDivisionsState {

    export function mapIdentities(
                state: ICharacterGuildDivisionsState,
                identitiesReducer: Reducer<ICharacterGuildDivisionsState["identities"]>):
            ICharacterGuildDivisionsState {
        return ImmutableObject.mapOne(state, "identities", identitiesReducer);
    }

    export function updateIdentities(
                state: ICharacterGuildDivisionsState,
                identities: ICharacterGuildDivisionsState["identities"]):
            ICharacterGuildDivisionsState {
        return ImmutableObject.updateOne(state, "identities", identities);
    }
}

export namespace CharacterGuildState {

    export function mapDivisions(
                state: ICharacterGuildState,
                divisionsReducer: Reducer<ICharacterGuildState["divisions"]>):
            ICharacterGuildState {
        return ImmutableObject.mapOne(state, "divisions", divisionsReducer);
    }
}
