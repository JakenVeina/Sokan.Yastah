export interface IRawUserOverviewViewModel
        extends IUserIdentityViewModel {
    firstSeen: string,
    lastSeen: string
}

export interface IUserDetailViewModel
        extends IUserOverviewViewModel {
    grantedPermissionIds: number[],
    deniedPermissionIds: number[],
    assignedRoleIds: number[]
}

export interface IUserIdentityViewModel {
    id: number;
    username: string;
    discriminator: string;
}

export interface IUserOverviewViewModel
        extends IUserIdentityViewModel {
    firstSeen: Date,
    lastSeen: Date
}

export interface IUserUpdateFormModel {
    id: number;
    permissionMappings: {
        [id: number]: "granted" | "unmapped" | "denied";
    };
    roleMappings: {
        [id: number]: boolean;
    };
}

export interface IUserUpdateModel {
    grantedPermissionIds: number[];
    deniedPermissionIds: number[];
    assignedRoleIds: number[];
}

export const userUpdateFormInitialState: IUserUpdateFormModel = {
    id: null,
    permissionMappings: {},
    roleMappings: {}
};
