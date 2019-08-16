import { IUserDetailViewModel, IUserUpdateFormModel, IUserUpdateModel } from "./models";

export function buildUserUpdateForm(
            detail: IUserDetailViewModel,
            permissionIds: number[],
            roleIds: number[]):
        IUserUpdateFormModel {
    return {
        id: detail.id,
        permissionMappings: Object.assign({}, ...permissionIds
            .map(id => ({
                [id]: detail.grantedPermissionIds.includes(id) ? "granted"
                    : detail.deniedPermissionIds.includes(id) ? "denied"
                    : "unmapped"
            }))),
        roleMappings: Object.assign({}, ...roleIds
            .map(id => ({
                [id]: detail.assignedRoleIds.includes(id)
            })))
    };
}

export function extractUserUpdate(form: IUserUpdateFormModel): IUserUpdateModel {
    return {
        grantedPermissionIds: Object.keys(form.permissionMappings)
            .filter(id => form.permissionMappings[id] === "granted")
            .map(id => Number(id)),
        deniedPermissionIds: Object.keys(form.permissionMappings)
            .filter(id => form.permissionMappings[id] === "denied")
            .map(id => Number(id)),
        assignedRoleIds: Object.keys(form.roleMappings)
            .filter(id => form.roleMappings[id] === true)
            .map(id => Number(id))
    };
}

