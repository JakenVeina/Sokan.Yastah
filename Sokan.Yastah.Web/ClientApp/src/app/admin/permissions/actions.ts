import { Action } from "@ngrx/store";
import { IPermissionCategoryDescriptionViewModel } from "./models";

export enum PermissionsActionType {
    LoadDescriptions = "[Permissions] LoadDescriptions",
    StoreDescriptions = "[Permissions] StoreDescriptions"
}

export class LoadDescriptionsAction
        implements Action {

    public constructor() {
        this.type = PermissionsActionType.LoadDescriptions;
    }

    public readonly type: string;
}

export class StoreDescriptionsAction
    implements Action {

    public constructor(
        payload: IPermissionCategoryDescriptionViewModel[]) {
        this.payload = payload;
        this.type = PermissionsActionType.StoreDescriptions;
    }

    public readonly payload: IPermissionCategoryDescriptionViewModel[];
    public readonly type: string;
}

export type PermissionsAction
    = LoadDescriptionsAction
        | StoreDescriptionsAction;
