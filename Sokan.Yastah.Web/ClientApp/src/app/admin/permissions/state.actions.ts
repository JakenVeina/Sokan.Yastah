import { createAction, props } from "@ngrx/store";

import { IPermissionCategoryDescriptionViewModel } from "./models";


export interface IPermissionsActionProps {
    readonly descriptions: IPermissionCategoryDescriptionViewModel[];
}


export namespace PermissionsActionFactory {

    export const beginFetchDescriptions
        = createAction(
            "[Administration][Permissions] beginFetchDescriptions");

    export const storeDescriptions
        = createAction(
            "[Administration][Permissions] storeDescriptions",
            props<IPermissionsActionProps>());
}
