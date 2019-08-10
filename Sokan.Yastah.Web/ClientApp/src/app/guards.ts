import { Injectable } from "@angular/core";
import { CanActivate } from "@angular/router";

import { AuthorizationService } from "./authorization/authorization-service";

@Injectable({
    providedIn: "root"
})
export class PortalGuard
        implements CanActivate {
    public constructor(
            authorizationService: AuthorizationService) {
        this._authorizationService = authorizationService;
    }

    public canActivate(): boolean {
        return this._authorizationService.hasAnyPermissions;
    }

    private readonly _authorizationService: AuthorizationService;
}
