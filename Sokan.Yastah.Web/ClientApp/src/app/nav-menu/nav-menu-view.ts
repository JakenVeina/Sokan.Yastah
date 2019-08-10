import { Component } from "@angular/core";
import { AuthenticationService } from "../authentication/authentication-service";
import { AuthorizationService } from "../authorization/authorization-service";

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

    public get avatarUri(): string {
        return (this._authenticationService.isAuthenticated && this._authenticationService.currentTicket.avatarHash != null)
            ? `https://cdn.discordapp.com/avatars/${this._authenticationService.currentTicket.userId}/${this._authenticationService.currentTicket.avatarHash}.png?size=32`
            : `https://cdn.discordapp.com/embed/avatars/0.png`;
    }

    public get isAuthenticated(): boolean {
        return this._authenticationService.isAuthenticated;
    }

    public get hasAdmin(): boolean {
        return this._authorizationService.hasAdmin;
    }

    public get hasAdminRoles(): boolean {
        return this._authorizationService.hasAdminManageRoles;
    }

    public get hasHome(): boolean {
        return this._authorizationService.hasAnyPermissions;
    }

    public get signoutUri(): string {
        return this._authenticationService.signoutUri;
    }

    public get userName(): string {
        return this._authenticationService.currentTicket.username;
    }

    private readonly _authenticationService: AuthenticationService;

    private readonly _authorizationService: AuthorizationService;
}
