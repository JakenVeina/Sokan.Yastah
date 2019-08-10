import { IUserIdentityViewModel } from "../users/models";

export interface IRoleCreationModel {
    name: string;
    grantedPermissionIds: number[];
}

export interface IRoleCreationFormModel {
    name: string;
    permissionMappings: {
        [id: number]: boolean;
    };
}

export interface IRoleDetailViewModel
        extends IRoleIdentityViewModel {
    grantedPermissionIds: number[];
    members: IUserIdentityViewModel[];
}

export interface IRoleIdentityViewModel {
    id: number;
    name: string;
}

export interface IRoleUpdateFormModel {
    id: number;
    name: string;
    permissionMappings: {
        [id: number]: boolean;
    };
}

export interface IRoleUpdateModel {
    name: string;
    grantedPermissionIds: number[];
}

export const roleUpdateFormInitialState: IRoleUpdateFormModel = {
    id: null,
    name: null,
    permissionMappings: {}
};

export const roleCreationFormInitialState: IRoleCreationFormModel = {
    name: "New Role",
    permissionMappings: {}
};
