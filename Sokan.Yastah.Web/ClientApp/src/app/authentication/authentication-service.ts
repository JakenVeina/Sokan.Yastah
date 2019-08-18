import { Injectable } from "@angular/core";
import * as JwtDecode from "jwt-decode";

import { IAuthenticationTicket, IRawAuthenticationTicket } from "./models";

@Injectable({
    providedIn: "root"
})
export class AuthenticationService {
    private static readonly _tokenHeaderAndPayloadCookieKey: string
        = "Yastah.Api.Authentication.Ticket.HeaderAndPayload";

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
            .map(cookie => cookie.trim())
            .find(cookie => cookie.startsWith(`${AuthenticationService._tokenHeaderAndPayloadCookieKey}=`));

        if (headerAndPayloadCookie == null) {
            this._currentHeaderAndPayload = null;
            this._currentTicket = null;
            return;
        }

        let headerAndPayload = headerAndPayloadCookie
            .split("=")[1];

        if (headerAndPayload === this._currentHeaderAndPayload) {
            return;
        }

        let rawTicket = JwtDecode<IRawAuthenticationTicket>(headerAndPayload);

        if (rawTicket.exp < (new Date().valueOf() / 1000)) {
            this._currentHeaderAndPayload = null;
            this._currentTicket = null;
            return;
        }

        this._currentHeaderAndPayload = headerAndPayload;
        this._currentTicket = {
            userId: rawTicket.nameid,
            username: rawTicket.unique_name,
            discriminator: rawTicket.dscm,
            avatarHash: rawTicket.avtr,
            grantedPermissions: rawTicket.prms
        };
    }

    private _currentHeaderAndPayload: string | null;
    private _currentTicket: IAuthenticationTicket | null;
}