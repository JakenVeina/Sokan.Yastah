import { createAction, props } from "@ngrx/store";

import { IAuthenticationTicket } from "./models";


export interface IAuthenticationTicketStorageActionProps {
    readonly ticket: IAuthenticationTicket | null;
}


export namespace AuthenticationActionFactory {

    export const storeTicket
        = createAction(
            "[Authentication] storeTicket",
            props<IAuthenticationTicketStorageActionProps>());
}
