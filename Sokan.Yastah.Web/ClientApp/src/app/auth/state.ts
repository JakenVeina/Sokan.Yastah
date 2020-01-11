import { ImmutableObject } from "../common/immutable-utils";

import { IAuthenticationTicket } from "./models";


export interface IAuthenticationState {
    readonly currentTicket: IAuthenticationTicket | null;
}

export const initialAuthenticationState: IAuthenticationState = {
    currentTicket: null
};


export namespace AuthenticationState {

    export function updateCurrentTicket(
                state: IAuthenticationState,
                currentTicket: IAuthenticationTicket | null):
            IAuthenticationState {
        return ImmutableObject.updateOne(state, "currentTicket", currentTicket);
    }
}
