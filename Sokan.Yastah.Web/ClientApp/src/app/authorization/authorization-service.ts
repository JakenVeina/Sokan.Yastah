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

    public get hasAdmin(): boolean {
        return this.hasAdminManagePermissions
            || this.hasAdminManageRoles;
    }

    public get hasAdminManagePermissions(): boolean {
        return this._authenticationService.isAuthenticated
            && Object.values(this._authenticationService.currentTicket.grantedPermissions).some(x => x === "Administration.ManagePermissions");
    }

    public get hasAdminManageRoles(): boolean {
        return this._authenticationService.isAuthenticated
            && Object.values(this._authenticationService.currentTicket.grantedPermissions).some(x => x === "Administration.ManageRoles");
    }

    public get hasAnyPermissions(): boolean {
        return this._authenticationService.isAuthenticated
            && (Object.keys(this._authenticationService.currentTicket.grantedPermissions).length > 0);
    }

    private readonly _authenticationService: AuthenticationService;
}
