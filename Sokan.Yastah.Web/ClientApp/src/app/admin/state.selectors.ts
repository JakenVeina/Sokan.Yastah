import { createFeatureSelector, createSelector, MemoizedSelector } from "@ngrx/store";

import { IAppState } from "../state";

import { IPermissionsState } from "./permissions/state";
import { IRolesState } from "./roles/state";
import { IAdminState } from "./state";


const adminState
    = createFeatureSelector<IAppState, IAdminState>("admin");


export namespace AdminSelectors {

    export const permissionsState: MemoizedSelector<IAppState, IPermissionsState>
        = createSelector(
            adminState,
            state => state.permissions);

    export const rolesState: MemoizedSelector<IAppState, IRolesState>
        = createSelector(
            adminState,
            state => state.roles);
}
