import { Action } from "@ngrx/store";

import { IAuthenticationTicket } from "./models";

export enum AuthenticationActionType {
    ReloadTicket = "[Authentication] ReloadTicket",
    StoreTicket = "[Authentication] StoreTicket"
}

export class ReloadTicketAction
    implements Action {

    public constructor() {
        this.type = AuthenticationActionType.ReloadTicket;
    }

    public readonly type: string;
}

export class StoreTicketAction
    implements Action {

    public constructor(
        payload: IAuthenticationTicket | null) {
        this.payload = payload;
        this.type = AuthenticationActionType.StoreTicket;
    }

    public readonly payload: IAuthenticationTicket | null;
    public readonly type: string;
}

export type AuthenticationAction
    = ReloadTicketAction
        | StoreTicketAction;
