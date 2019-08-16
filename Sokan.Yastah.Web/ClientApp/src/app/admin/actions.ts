import { PermissionsAction } from "./permissions/actions";
import { RolesAction } from "./roles/actions";
import { UsersAction } from "./users/actions";

export type AdminAction
    = PermissionsAction
        | RolesAction
        | UsersAction;
