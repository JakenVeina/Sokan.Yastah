import { Component } from "@angular/core";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";

import { IAppState } from "../state";

import { AuthenticationService } from "../authentication/authentication-service";
import { AuthorizationService } from "../authorization/authorization-service";

@Component({
  selector: "nav-menu-view",
  templateUrl: "./nav-menu-view.ts.html",
  styleUrls: ["./nav-menu-view.ts.css"]
})
export class NavMenuView {
    public constructor(
            appState: Store<IAppState>,
            authenticationService: AuthenticationService,
            authorizationService: AuthorizationService) {
        this._authenticationService = authenticationService;
        this._authorizationService = authorizationService;

        let currentTicket = appState
            .select(x => x.authentication.currentTicket);

        this._avatarUrl = currentTicket
            .pipe(map(x => ((x && x.avatarHash) != null)
                ? `https://cdn.discordapp.com/avatars/${x.userId}/${x.avatarHash}.png?size=32`
                : `https://cdn.discordapp.com/embed/avatars/0.png`));

        this._username = currentTicket
            .pipe(map(x => x && x.username));
    }

    public get avatarUri(): Observable<string> {
        return this._avatarUrl;
    }

    public get isAuthenticated(): Observable<boolean> {
        return this._authenticationService.isAuthenticated;
    }

    public get hasAdmin(): Observable<boolean> {
        return this._authorizationService.hasAdmin;
    }

    public get hasAdminRoles(): Observable<boolean> {
        return this._authorizationService.hasAdminManageRoles;
    }

    public get hasAdminUsers(): Observable<boolean> {
        return this._authorizationService.hasAdminManageUsers;
    }

    public get hasHome(): Observable<boolean> {
        return this._authorizationService.hasAnyPermissions;
    }

    public get signoutUri(): string {
        return this._authenticationService.signoutUri;
    }

    public get username(): Observable<string> {
        return this._username;
    }

    private readonly _authenticationService: AuthenticationService;
    private readonly _authorizationService: AuthorizationService;
    private readonly _avatarUrl: Observable<string>;
    private readonly _username: Observable<string>;
}
