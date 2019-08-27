import { ActionReducer } from "@ngrx/store";

import { initialAuthenticationState, IAuthenticationState } from "./state";
import { AuthenticationAction, AuthenticationActionType, StoreTicketAction } from "./actions";

export const authenticationStateReducer: ActionReducer<IAuthenticationState, AuthenticationAction> = (
            state: IAuthenticationState = initialAuthenticationState,
            action: AuthenticationAction):
        IAuthenticationState => {

    switch (action.type) {

        case AuthenticationActionType.StoreTicket:
            return {
                ...state,
                currentTicket: (action as StoreTicketAction).payload
            };

        default:
            return state;
    }
};
