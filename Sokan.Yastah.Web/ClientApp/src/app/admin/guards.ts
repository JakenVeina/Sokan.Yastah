import { Injectable } from "@angular/core";
import { CanActivate } from "@angular/router";

import { AuthorizationService } from "../authorization/authorization-service";

@Injectable({
    providedIn: "root"
})
export class AdminGuard
        implements CanActivate {
    public constructor(
            authorizationService: AuthorizationService) {
        this._authorizationService = authorizationService;
    }

    public canActivate(): boolean {
        return this._authorizationService.hasAdmin;
    }

    private readonly _authorizationService: AuthorizationService;
}

@Injectable({
    providedIn: "root"
})
export class RolesGuard
        implements CanActivate {
    public constructor(
            authorizationService: AuthorizationService) {
        this._authorizationService = authorizationService;
    }

    public canActivate(): boolean {
        return this._authorizationService.hasAdminManageRoles;
    }

    private readonly _authorizationService: AuthorizationService;
}
