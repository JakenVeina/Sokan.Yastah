import { FetchedValue, FetchState, IFetchedValue } from "../../common/fetching-utils";
import { ImmutableObject } from "../../common/immutable-utils";
import { Reducer } from "../../common/types";

import { IRoleIdentityViewModel } from "./models";


export interface IRolesState {
    readonly identities: IFetchedValue<IRoleIdentityViewModel[] | null>;
}


export const initialRolesState: IRolesState = {
    identities: FetchedValue.unfetched(null),
};


export namespace RolesState {

    export function mapIdentities(
                state: IRolesState,
                identitiesReducer: Reducer<IRolesState["identities"]>):
            IRolesState {
        return ImmutableObject.mapOne(state, "identities", identitiesReducer);
    }

    export function updateIdentities(
                state: IRolesState,
                identities: IRolesState["identities"]):
            IRolesState {
        return ImmutableObject.updateOne(state, "identities", identities);
    }
}
