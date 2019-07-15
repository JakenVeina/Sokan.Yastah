import { Component } from "@angular/core";
import { AuthenticationService } from "./../authentication/authentication-service";
import { AuthorizationService } from "./../authorization/authorization-service";

@Component({
  selector: "app-home",
  templateUrl: "./home.component.ts.html"
})
export class HomeComponent {
    public constructor(
            authenticationService: AuthenticationService,
            authorizationService: AuthorizationService) {
        this._authenticationService = authenticationService;
        this._authorizationService = authorizationService;
    }

    public get canUsePortal(): boolean {
        return this._authorizationService.canUsePortal;
    }

    public get isAuthenticated(): boolean {
        return this._authenticationService.isAuthenticated;
    }

    public get signinUri(): string {
        return this._authenticationService.signinUri;
    }

    private readonly _authenticationService: AuthenticationService;

    private readonly _authorizationService: AuthorizationService;
}
