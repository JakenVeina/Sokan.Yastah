import { IAuthenticationTicket } from "./models";

export interface IAuthenticationState {
    currentTicket: IAuthenticationTicket | null;
}

export const initialAuthenticationState: IAuthenticationState = {
    currentTicket: null
};
