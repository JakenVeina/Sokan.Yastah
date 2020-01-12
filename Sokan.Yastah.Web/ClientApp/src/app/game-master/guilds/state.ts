import { FetchedValue, IFetchedValue } from "../../common/fetching-utils";
import { ImmutableObject } from "../../common/immutable-utils";
import { Reducer, INativeHashTable } from "../../common/types";

import {
    ICharacterGuildDivisionIdentityViewModel,
    ICharacterGuildIdentityViewModel
} from "./models";


export interface IGuildsState {
    readonly identities: IFetchedValue<ICharacterGuildIdentityViewModel[] | null>;
    readonly stateTable: Readonly<INativeHashTable<IGuildState>>;
}
export interface IGuildState {
    readonly divisions: IGuildDivisionsState;
}
export interface IGuildDivisionsState {
    readonly identities: IFetchedValue<ICharacterGuildDivisionIdentityViewModel[] | null>;
}


export const initialGuildsState: IGuildsState = {
    identities: FetchedValue.unfetched(null),
    stateTable: {}
};
export const initialGuildState: IGuildState = {
    divisions: {
        identities: FetchedValue.unfetched(null)
    }
};


export namespace GuildsState {

    export function mapIdentities(
                state: IGuildsState,
                identitiesReducer: Reducer<IGuildsState["identities"]>):
            IGuildsState {
        return ImmutableObject.mapOne(state, "identities", identitiesReducer);
    }

    export function mapStateTable(
                state: IGuildsState,
                stateTableReducer: Reducer<IGuildsState["stateTable"]>):
            IGuildsState {
        return ImmutableObject.mapOne(state, "stateTable", stateTableReducer);
    }

    export function updateIdentities(
                state: IGuildsState,
                identities: IGuildsState["identities"]):
            IGuildsState {
        return ImmutableObject.updateOne(state, "identities", identities);
    }
}

export namespace GuildDivisionsState {

    export function mapIdentities(
                state: IGuildDivisionsState,
                identitiesReducer: Reducer<IGuildDivisionsState["identities"]>):
            IGuildDivisionsState {
        return ImmutableObject.mapOne(state, "identities", identitiesReducer);
    }

    export function updateIdentities(
                state: IGuildDivisionsState,
                identities: IGuildDivisionsState["identities"]):
            IGuildDivisionsState {
        return ImmutableObject.updateOne(state, "identities", identities);
    }
}

export namespace GuildState {

    export function mapDivisions(
                state: IGuildState,
                divisionsReducer: Reducer<IGuildState["divisions"]>):
            IGuildState {
        return ImmutableObject.mapOne(state, "divisions", divisionsReducer);
    }
}
