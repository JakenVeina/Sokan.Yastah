import { Injectable } from "@angular/core";
import { AuthenticationService } from "./../authentication/authentication-service";

@Injectable({
    providedIn: "root"
})
export class AuthorizationService {
    public constructor(
            authenticationService : AuthenticationService) {
        this._authenticationService = authenticationService;
    }

    public get canUsePortal(): boolean {
        return this._authenticationService.isAuthenticated
            && this._authenticationService.ticket.hasAnyPermissions;
    }

    private readonly _authenticationService: AuthenticationService;
}
