import { createReducer, on, Action } from "@ngrx/store";

import { FetchedValue, FetchState } from "../../common/fetching-utils";

import { PermissionsActionFactory } from "./state.actions";
import { initialPermissionsState, IPermissionsState, PermissionsState } from "./state";


const _permissionsStateReducer
    = createReducer<IPermissionsState>(
        initialPermissionsState,
        on(
            PermissionsActionFactory.beginFetchDescriptions,
            state => PermissionsState.mapDescriptions(state,
                descriptions => FetchedValue.updateState(descriptions, FetchState.fetching))),
        on(
            PermissionsActionFactory.storeDescriptions,
            (state, action) => PermissionsState.updateDescriptions(state,
                FetchedValue.fetched(action.descriptions))));


export function permissionsStateReducer(
            state: IPermissionsState | undefined,
            action: Action):
        IPermissionsState {
    return _permissionsStateReducer(state, action);
}
