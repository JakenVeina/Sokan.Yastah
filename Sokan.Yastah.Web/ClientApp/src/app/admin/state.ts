import { IPermissionsState } from "./permissions/state";
import { IRolesState } from "./roles/state";


export interface IAdminState {
    readonly permissions: IPermissionsState;
    readonly roles: IRolesState;
}
