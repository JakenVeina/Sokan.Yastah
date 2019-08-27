import * as JwtDecode from "jwt-decode";

import { IAuthenticationTicket, IRawAuthenticationTicket } from "./models";

export const authenticationTicketHeaderAndPayloadCookieName
    = "Yastah.Api.Authentication.Ticket.HeaderAndPayload";

export function readCurrentTicket(): IAuthenticationTicket | null {
    let headerAndPayloadCookie = document.cookie
        .split(";")
        .map(cookie => cookie.trim())
        .find(cookie => cookie.startsWith(`${authenticationTicketHeaderAndPayloadCookieName}=`));

    if (headerAndPayloadCookie == null) {
        return null;
    }

    let headerAndPayload = headerAndPayloadCookie
        .split("=")[1];

    let rawTicket = JwtDecode<IRawAuthenticationTicket>(headerAndPayload);

    if (rawTicket.exp < (new Date().valueOf() / 1000)) {
        return null;
    }

    return {
        id: rawTicket.tckt,
        userId: rawTicket.nameid,
        username: rawTicket.unique_name,
        discriminator: rawTicket.dscm,
        avatarHash: rawTicket.avtr,
        grantedPermissions: rawTicket.prms
    };
}
