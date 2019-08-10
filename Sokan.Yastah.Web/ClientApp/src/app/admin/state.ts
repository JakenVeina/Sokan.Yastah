import { IPermissionsState } from "./permissions/state";
import { IRolesState } from "./roles/state";

export interface IAdminState {
    permissions: IPermissionsState;
    roles: IRolesState;
}
