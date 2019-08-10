import { ActionReducer } from "@ngrx/store";

import { PermissionsAction, PermissionsActionType, StoreDescriptionsAction } from "./actions";
import { initialPermissionsState, IPermissionsState } from "./state";


export const permissionsStateReducer: ActionReducer<IPermissionsState, PermissionsAction> = (
    state: IPermissionsState = initialPermissionsState,
    action: PermissionsAction):
    IPermissionsState => {
    switch (action.type) {

        case PermissionsActionType.StoreDescriptions:
            return {
                ...state,
                descriptions: (action as StoreDescriptionsAction).payload
            };

        default:
            return state;
    }
};
