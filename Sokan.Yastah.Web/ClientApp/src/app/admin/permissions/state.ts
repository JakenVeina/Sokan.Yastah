import { IPermissionCategoryDescriptionViewModel } from "./models";

export interface IPermissionsState {
    descriptions: IPermissionCategoryDescriptionViewModel[]
}

export const initialPermissionsState: IPermissionsState = {
    descriptions: []
};
