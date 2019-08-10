import { Component } from "@angular/core";

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
    }

    public get signinUri(): string {
        return this._authenticationService.signinUri;
    }

    public get showLogin(): boolean {
        return !this._authenticationService.isAuthenticated;
    }

    public get showPortal(): boolean {
        return this._authenticationService.isAuthenticated
            && this._authorizationService.hasAnyPermissions;
    }

    private readonly _authenticationService: AuthenticationService;
    private readonly _authorizationService: AuthorizationService;
}
