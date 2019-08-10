﻿import { Action } from "@ngrx/store";

import { IRoleIdentityViewModel } from "./models";

export enum RolesActionType {
    ReloadIdentities = "[Roles] ReloadIdentities",
    StoreIdentities = "[Roles] StoreIdentities"
}

export class ReloadIdentitiesAction
    implements Action {

    public constructor() {
        this.type = RolesActionType.ReloadIdentities;
    }

    public readonly type: string;
}

export class StoreIdentitiesAction
        implements Action {

    public constructor(
            payload: IRoleIdentityViewModel[]) {
        this.payload = payload;
        this.type = RolesActionType.StoreIdentities;
    }

    public readonly payload: IRoleIdentityViewModel[];
    public readonly type: string;
}

export type RolesAction
    = ReloadIdentitiesAction
        | StoreIdentitiesAction;
