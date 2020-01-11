import { createReducer, on, Action } from "@ngrx/store";

import { AuthenticationActionFactory } from "./state.actions";
import { initialAuthenticationState, AuthenticationState, IAuthenticationState } from "./state";


const _authenticationStateReducer
    = createReducer<IAuthenticationState>(
        initialAuthenticationState,
        on(
            AuthenticationActionFactory.storeTicket,
            (state, action) => AuthenticationState.updateCurrentTicket(state, action.ticket)));


export function authenticationStateReducer(
            state: IAuthenticationState | undefined,
            action: Action):
        IAuthenticationState {
    return _authenticationStateReducer(state, action);
}
