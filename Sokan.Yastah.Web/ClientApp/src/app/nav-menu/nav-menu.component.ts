import { Component } from "@angular/core";
import { AuthenticationService } from "./../authentication/authentication-service";
import { AuthorizationService } from "./../authorization/authorization-service";

@Component({
  selector: "app-nav-menu",
  templateUrl: "./nav-menu.component.ts.html",
  styleUrls: ["./nav-menu.component.ts.css"]
})
export class NavMenuComponent {
    public constructor(
            authenticationService: AuthenticationService,
            authorizationService: AuthorizationService) {
        this._authenticationService = authenticationService;
        this._authorizationService = authorizationService;
    }

    public get avatarUri(): string {
        return (this._authenticationService.isAuthenticated && this._authenticationService.ticket.avatarHash != null)
            ? `https://cdn.discordapp.com/avatars/${this._authenticationService.ticket.userId}/${this._authenticationService.ticket.avatarHash}.png?size=32`
            : `https://cdn.discordapp.com/embed/avatars/0.png`;
    }

    public get isAuthenticated(): boolean {
        return this._authenticationService.isAuthenticated;
    }

    public get hasHome(): boolean {
        return this._authorizationService.canUsePortal;
    }

    public get signoutUri(): string {
        return this._authenticationService.signoutUri;
    }

    public get userName(): string {
        return this._authenticationService.ticket.userName;
    }

    private readonly _authenticationService: AuthenticationService;

    private readonly _authorizationService: AuthorizationService;
}
