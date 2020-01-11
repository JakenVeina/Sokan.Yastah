import { FetchedValue, IFetchedValue } from "../common/fetching-utils";
import { ImmutableObject } from "../common/immutable-utils";
import { Reducer, INativeHashTable } from "../common/types";

import {
    ICharacterGuildDivisionIdentityViewModel,
    ICharacterGuildIdentityViewModel
} from "./models";


export interface ICharacterGuildsState {
    readonly identities: IFetchedValue<ICharacterGuildIdentityViewModel[] | null>;
    readonly stateTable: Readonly<INativeHashTable<ICharacterGuildState>>;
}
export interface ICharacterGuildState {
    readonly divisions: ICharacterGuildDivisionsState;
}
export interface ICharacterGuildDivisionsState {
    readonly identities: IFetchedValue<ICharacterGuildDivisionIdentityViewModel[] | null>;
}


export const initialCharacterGuildsState: ICharacterGuildsState = {
    identities: FetchedValue.unfetched(null),
    stateTable: {}
};
export const initialCharacterGuildState: ICharacterGuildState = {
    divisions: {
        identities: FetchedValue.unfetched(null)
    }
};


export namespace CharacterGuildsState {

    export function mapIdentities(
                state: ICharacterGuildsState,
                identitiesReducer: Reducer<ICharacterGuildsState["identities"]>):
            ICharacterGuildsState {
        return ImmutableObject.mapOne(state, "identities", identitiesReducer);
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
