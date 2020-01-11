import { Injectable } from "@angular/core";
import { CanActivate } from "@angular/router";

import { Observable } from "rxjs";

import { AuthorizationService } from "./auth/services";


@Injectable({
    providedIn: "root"
})
export class PortalGuard
        implements CanActivate {
    public constructor(
            authorizationService: AuthorizationService) {
        this._authorizationService = authorizationService;
    }

    public canActivate(): Observable<boolean> {
        return this._authorizationService.observeHasAnyPermissions;
    }

    private readonly _authorizationService: AuthorizationService;
}
