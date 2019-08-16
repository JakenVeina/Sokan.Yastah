import { IPermissionsState } from "./permissions/state";
import { IRolesState } from "./roles/state";
import { IUsersState } from "./users/state";

export interface IAdminState {
    permissions: IPermissionsState;
    roles: IRolesState;
    users: IUsersState;
}
