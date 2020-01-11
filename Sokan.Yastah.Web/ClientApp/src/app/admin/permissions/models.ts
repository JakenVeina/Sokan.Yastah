export interface IPermissionCategoryDescriptionViewModel {
    readonly id: number;
    readonly name: string;
    readonly description: string;
    readonly permissions: IPermissionDescriptionViewModel[];
}

export interface IPermissionDescriptionViewModel {
    readonly id: number,
    readonly name: string,
    readonly description: string,
}


export namespace PermissionCategoryDescriptionViewModel {

    export function mapPermissions(
                descriptions: IPermissionCategoryDescriptionViewModel[]):
            IPermissionDescriptionViewModel[] {
        return descriptions
            .map(category => category.permissions)
            .reduce((flattened, permissions) => flattened.concat(permissions), []);
    }
}
