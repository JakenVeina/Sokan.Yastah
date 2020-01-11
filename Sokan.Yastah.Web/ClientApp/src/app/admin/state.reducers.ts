import { ActionReducerMap, Action } from "@ngrx/store";

import { permissionsStateReducer } from "./permissions/state.reducers";
import { rolesStateReducer } from './roles/state.reducers';

import { IAdminState } from "./state";


export const adminStateReducers: ActionReducerMap<IAdminState, Action> = {
    permissions: permissionsStateReducer,
    roles: rolesStateReducer
};
