import { Component } from "@angular/core";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";

import { AuthorizationService } from "./authorization/authorization-service";
import { AuthenticationService } from "./authentication/authentication-service";

@Component({
    selector: "app-root",
    templateUrl: "./app-view.ts.html",
    styleUrls: ["./app-view.ts.css"]
})
export class AppView {

    public constructor(
            authenticationService: AuthenticationService,
            authorizationService: AuthorizationService) {
        this._authenticationService = authenticationService;
        this._authorizationService = authorizationService;

        this._showLogin = authenticationService.isAuthenticated
            .pipe(map(x => !x));
    }

    public get signinUri(): string {
        return this._authenticationService.signinUri;
    }

    public get showLogin(): Observable<boolean> {
        return this._showLogin;
    }

    public get showPortal(): Observable<boolean> {
        return this._authorizationService.hasAnyPermissions;
    }

    private readonly _authenticationService: AuthenticationService;
    private readonly _authorizationService: AuthorizationService;
    private readonly _showLogin: Observable<boolean>;
}
