import { Injectable } from "@angular/core";
import * as JwtDecode from "jwt-decode";

import { IAuthenticationTicket, IRawAuthenticationTicket } from "./models";

@Injectable({
    providedIn: "root"
})
export class AuthenticationService {
    private static readonly _tokenHeaderAndPayloadCookieKey: string
        = "Yastah.Api.Token.HeaderAndPayload";

    private static readonly _authenticationEndpoint: string
        = "/api/authentication";

    public constructor() {
        this.updateTicket();
    }

    public get isAuthenticated(): boolean {
        return this._currentTicket != null;
    }

    public get signinUri(): string {
        return `${AuthenticationService._authenticationEndpoint}/challenge`;
    }

    public get signoutUri(): string {
        return `${AuthenticationService._authenticationEndpoint}/signout`;
    }

    public get currentTicket(): IAuthenticationTicket | null {
        return this._currentTicket;
    }

    public updateTicket(): void {
        let headerAndPayloadCookie = document.cookie
            .split(";")
            .find(cookie => cookie.startsWith(`${AuthenticationService._tokenHeaderAndPayloadCookieKey}=`));

        if (headerAndPayloadCookie != null) {
            let headerAndPayload = headerAndPayloadCookie
                .split("=")[1];

            let rawTicket = JwtDecode<IRawAuthenticationTicket>(headerAndPayload);

            this._currentTicket = {
                userId: rawTicket.nameid,
                username: rawTicket.unique_name,
                discriminator: rawTicket.dscm,
                avatarHash: rawTicket.avtr,
                grantedPermissions: rawTicket.prms
            };
        }
        else {
            this._currentTicket = null;
        }
    }

    private _currentTicket: IAuthenticationTicket | null;
}