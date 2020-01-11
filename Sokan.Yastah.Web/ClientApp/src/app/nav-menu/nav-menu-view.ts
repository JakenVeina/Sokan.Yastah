import { Component } from "@angular/core";
import { Observable } from "rxjs";

import { AuthenticationService, AuthorizationService } from "../auth/services";


@Component({
  selector: "nav-menu-view",
  templateUrl: "./nav-menu-view.ts.html",
  styleUrls: ["./nav-menu-view.ts.css"]
})
export class NavMenuView {
    public constructor(
            authenticationService: AuthenticationService,
            authorizationService: AuthorizationService) {

        this._authenticationService = authenticationService;
        this._authorizationService = authorizationService;
    }

    public get avatarUri(): Observable<string | null> {
        return this._authenticationService.observeAvatarUri;
    }
    public get isAuthenticated(): Observable<boolean> {
        return this._authenticationService.observeIsAuthenticated;
    }
    public get hasAdmin(): Observable<boolean> {
        return this._authorizationService.observeHasAdmin;
    }
    public get hasAdminRoles(): Observable<boolean> {
        return this._authorizationService.observeHasAdminManageRoles;
    }
    public get hasAdminUsers(): Observable<boolean> {
        return this._authorizationService.observeHasAdminManageUsers;
    }
    public get hasCharacterGuilds(): Observable<boolean> {
        return this._authorizationService.observeHasCharacterAdminManageGuilds;
    }
    public get hasHome(): Observable<boolean> {
        return this._authorizationService.observeHasAnyPermissions;
    }
    public get signoutUri(): string {
        return this._authenticationService.signoutUri;
    }
    public get username(): Observable<string | null> {
        return this._authenticationService.observeUsername;
    }

    private readonly _authenticationService: AuthenticationService;
    private readonly _authorizationService: AuthorizationService;
}
