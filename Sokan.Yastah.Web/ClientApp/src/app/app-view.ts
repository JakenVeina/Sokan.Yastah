import { Component, OnInit } from "@angular/core";

import { combineLatest, Observable } from "rxjs";
import { map } from "rxjs/operators";

import { AuthenticationService, AuthorizationService } from "./auth/services";


@Component({
    selector: "app-root",
    templateUrl: "./app-view.ts.html",
    styleUrls: ["./app-view.ts.css"]
})
export class AppView
        implements OnInit {

    public constructor(
            authenticationService: AuthenticationService,
            authorizationService: AuthorizationService) {
        this._authenticationService = authenticationService;
        this._authorizationService = authorizationService;

        this._showLogin = authenticationService.observeIsAuthenticated
            .pipe(map(x => !x));

        this._showUnauthorized = combineLatest(
                authenticationService.observeIsAuthenticated,
                authorizationService.observeHasAnyPermissions)
            .pipe(map(([isAuthenticated, hasAnyPermissions]) => isAuthenticated && !hasAnyPermissions));
    }

    public get signinUri(): string {
        return this._authenticationService.signinUri;
    }
    public get showLogin(): Observable<boolean> {
        return this._showLogin;
    }
    public get showPortal(): Observable<boolean> {
        return this._authorizationService.observeHasAnyPermissions;
    }
    public get showUnauthorized(): Observable<boolean> {
        return this._showUnauthorized;
    }

    public ngOnInit(): void {
        this._authenticationService.reloadTicket();
    }

    private readonly _authenticationService: AuthenticationService;
    private readonly _authorizationService: AuthorizationService;
    private readonly _showLogin: Observable<boolean>;
    private readonly _showUnauthorized: Observable<boolean>;
}
