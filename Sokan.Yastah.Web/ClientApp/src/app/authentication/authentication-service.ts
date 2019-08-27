import { Injectable } from "@angular/core";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs";

import { IAppState } from "../state";

@Injectable({
    providedIn: "root"
})
export class AuthenticationService {

    private static readonly _authenticationEndpoint: string
        = "/api/authentication";

    public constructor(
            appState: Store<IAppState>) {
        this._isAuthenticated = appState
            .select(x => x.authentication.currentTicket != null);
    }

    public get isAuthenticated(): Observable<boolean> {
        return this._isAuthenticated;
    }

    public get signinUri(): string {
        return `${AuthenticationService._authenticationEndpoint}/challenge`;
    }

    public get signoutUri(): string {
        return `${AuthenticationService._authenticationEndpoint}/signout`;
    }

    private readonly _isAuthenticated: Observable<boolean>
}
