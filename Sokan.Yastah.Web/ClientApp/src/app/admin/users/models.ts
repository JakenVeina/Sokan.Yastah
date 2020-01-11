export interface IUserDetailViewModel
        extends IUserOverviewViewModel {
    readonly grantedPermissionIds: number[],
    readonly deniedPermissionIds: number[],
    readonly assignedRoleIds: number[]
}

export interface IUserIdentityViewModel {
    readonly id: number;
    readonly username: string;
    readonly discriminator: string;
}

export interface IUserOverviewViewModel
        extends IUserIdentityViewModel {
    readonly firstSeen: string,
    readonly lastSeen: string
}

export interface IUserUpdateModel {
    readonly grantedPermissionIds: number[];
    readonly deniedPermissionIds: number[];
    readonly assignedRoleIds: number[];
}
