import { ActionReducer } from "@ngrx/store";

import { initialRolesState, IRolesState } from "./state";
import { RolesAction, RolesActionType, StoreIdentitiesAction } from "./actions";

export const rolesStateReducer: ActionReducer<IRolesState, RolesAction> = (
            state: IRolesState = initialRolesState,
            action: RolesAction):
        IRolesState => {
    switch (action.type) {

        case RolesActionType.StoreIdentities:
            return {
                ...state,
                identities: (action as StoreIdentitiesAction).payload
            };

        default:
            return state;
    }
};
