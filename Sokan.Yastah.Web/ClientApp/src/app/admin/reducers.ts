import { ActionReducerMap } from "@ngrx/store";

import { permissionsStateReducer } from "./permissions/reducers";
import { rolesStateReducer } from './roles/reducers';

import { AdminAction } from "./actions";
import { IAdminState } from "./state";

export const adminStateReducers: ActionReducerMap<IAdminState, AdminAction> = {
    permissions: permissionsStateReducer,
    roles: rolesStateReducer
};
