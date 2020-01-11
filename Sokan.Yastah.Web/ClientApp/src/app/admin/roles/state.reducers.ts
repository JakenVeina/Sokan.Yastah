import { createReducer, on, Action } from "@ngrx/store";

import { FetchState, FetchedValue } from "../../common/fetching-utils";

import { RolesActionFactory } from "./state.actions";
import { initialRolesState, IRolesState, RolesState } from "./state";


const _rolesStateReducer
    = createReducer<IRolesState>(
        initialRolesState,
        on(
            RolesActionFactory.beginFetchIdentities,
            state => RolesState.mapIdentities(state,
                identities => FetchedValue.updateState(identities, FetchState.fetching))),
        on(
            RolesActionFactory.scheduleFetchIdentities,
            state => RolesState.mapIdentities(state,
                identities => (identities.state === FetchState.fetching)
                    ? identities
                    : FetchedValue.updateState(identities, FetchState.unfetched))),
        on(
            RolesActionFactory.storeIdentities,
            (state, action) => RolesState.updateIdentities(state,
                FetchedValue.fetched(action.identities))));


export function rolesStateReducer(
            state: IRolesState | undefined,
            action: Action):
        IRolesState {
    return _rolesStateReducer(state, action);
}
