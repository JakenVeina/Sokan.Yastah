import { IUserIdentityViewModel } from "../users/models";


export interface IRoleCreationModel {
    readonly name: string;
    readonly grantedPermissionIds: number[];
}

export interface IRoleDetailViewModel
        extends IRoleIdentityViewModel {
    readonly grantedPermissionIds: number[];
    readonly members: IUserIdentityViewModel[];
}

export interface IRoleIdentityViewModel {
    readonly id: number;
    readonly name: string;
}

export interface IRoleUpdateModel {
    readonly name: string;
    readonly grantedPermissionIds: number[];
}
