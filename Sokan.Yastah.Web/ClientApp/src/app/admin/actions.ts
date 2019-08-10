import { PermissionsAction } from "./permissions/actions";
import { RolesAction } from "./roles/actions";

export type AdminAction
    = PermissionsAction
        | RolesAction;
