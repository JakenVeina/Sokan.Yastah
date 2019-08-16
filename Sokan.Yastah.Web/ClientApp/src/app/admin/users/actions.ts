import { Action } from "@ngrx/store";

import { IUserOverviewViewModel } from "./models";

export enum UsersActionType {
    ReloadOverviews = "[Users] ReloadOverviews",
    StoreOverviews = "[Users] StoreOverviews"
}

export class ReloadOverviewsAction
    implements Action {

    public constructor() {
        this.type = UsersActionType.ReloadOverviews;
    }

    public readonly type: string;
}

export class StoreOverviewsAction
    implements Action {

    public constructor(
            payload: IUserOverviewViewModel[]) {
        this.payload = payload;
        this.type = UsersActionType.StoreOverviews;
    }

    public readonly payload: IUserOverviewViewModel[];
    public readonly type: string;
}

export type UsersAction
    = ReloadOverviewsAction
        | StoreOverviewsAction;
