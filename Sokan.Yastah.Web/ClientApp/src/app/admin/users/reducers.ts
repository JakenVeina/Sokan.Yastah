import { ActionReducer } from "@ngrx/store";

import { initialUsersState, IUsersState } from "./state";
import { UsersAction, UsersActionType, StoreOverviewsAction } from "./actions";

export const usersStateReducer: ActionReducer<IUsersState, UsersAction> = (
            state: IUsersState = initialUsersState,
            action: UsersAction):
        IUsersState => {
    switch (action.type) {

        case UsersActionType.StoreOverviews:
            return {
                ...state,
                overviews: (action as StoreOverviewsAction).payload
            };

        default:
            return state;
    }
};
