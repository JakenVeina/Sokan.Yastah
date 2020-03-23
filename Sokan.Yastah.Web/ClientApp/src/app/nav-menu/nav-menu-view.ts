import { Component } from "@angular/core";

import { BehaviorSubject, Observable } from "rxjs";

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

        this._isAdminSubmenuVisible = new BehaviorSubject<boolean>(false);
        this._isGameMasterSubmenuVisible = new BehaviorSubject<boolean>(false);
    }

    public get avatarUri(): Observable<string | null> {
        return this._authenticationService.observeAvatarUri;
    }
    public get isAdminSubmenuVisible(): Observable<boolean> {
        return this._isAdminSubmenuVisible;
    }
    public get isAuthenticated(): Observable<boolean> {
        return this._authenticationService.observeIsAuthenticated;
    }
    public get isGameMasterSubmenuVisible(): Observable<boolean> {
        return this._isGameMasterSubmenuVisible;
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
    public get hasGameMaster(): Observable<boolean> {
        return this._authorizationService.observeHasCharacterManage;
    }
    public get hasGameMasterGuilds(): Observable<boolean> {
        return this._authorizationService.observeHasCharacterManageGuilds;
    }
    public get hasGameMasterLevels(): Observable<boolean> {
        return this._authorizationService.observeHasCharacterManageLevels;
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

    public toggleAdminSubmenu():
            void {
        this._isAdminSubmenuVisible.next(!this._isAdminSubmenuVisible.value);
    }
    public toggleGameMasterSubmenu():
            void {
        this._isGameMasterSubmenuVisible.next(!this._isGameMasterSubmenuVisible.value);
    }

    private readonly _authenticationService: AuthenticationService;
    private readonly _authorizationService: AuthorizationService;
    private readonly _isAdminSubmenuVisible: BehaviorSubject<boolean>;
    private readonly _isGameMasterSubmenuVisible: BehaviorSubject<boolean>;
}
