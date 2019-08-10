export interface IPermissionCategoryDescriptionViewModel {
    id: number;
    name: string;
    description: string;
    permissions: IPermissionDescriptionViewModel[];
}

export interface IPermissionDescriptionViewModel {
    id: number,
    name: string,
    description: string,
}
