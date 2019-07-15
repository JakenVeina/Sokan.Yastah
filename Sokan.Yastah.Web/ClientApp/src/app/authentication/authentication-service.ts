import { Injectable } from "@angular/core";
import * as JwtDecode from "jwt-decode";
import { AuthenticationTicket } from "./authentication-ticket";
import { RawAuthenticationTicket } from "./raw-authentication-ticket";

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
        return this._ticket != null;
    }

    public get signinUri(): string {
        return `${AuthenticationService._authenticationEndpoint}/challenge`;
    }

    public get signoutUri(): string {
        return `${AuthenticationService._authenticationEndpoint}/signout`;
    }

    public get ticket(): AuthenticationTicket | null {
        return this._ticket;
    }

    public updateTicket(): void {
        let headerAndPayloadCookie = document.cookie
            .split(";")
            .find(cookie => cookie.startsWith(`${AuthenticationService._tokenHeaderAndPayloadCookieKey}=`));

        if (headerAndPayloadCookie != null) {
            let headerAndPayload = headerAndPayloadCookie
                .split("=")[1];

            this._ticket = new AuthenticationTicket(
                JwtDecode<RawAuthenticationTicket>(
                    headerAndPayload));
        }
        else {
            this._ticket = null;
        }
    }

    private _ticket: AuthenticationTicket | null;
}