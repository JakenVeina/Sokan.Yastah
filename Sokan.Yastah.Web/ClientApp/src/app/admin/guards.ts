import { Injectable } from "@angular/core";
import { CanActivate } from "@angular/router";
import { Observable } from "rxjs";

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

    public canActivate(): Observable<boolean> {
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

    public canActivate(): Observable<boolean> {
        return this._authorizationService.hasAdminManageRoles;
    }

    private readonly _authorizationService: AuthorizationService;
}

@Injectable({
    providedIn: "root"
})
export class UsersGuard
        implements CanActivate {
    public constructor(
            authorizationService: AuthorizationService) {
        this._authorizationService = authorizationService;
    }

    public canActivate(): Observable<boolean> {
        return this._authorizationService.hasAdminManageUsers;
    }

    private readonly _authorizationService: AuthorizationService;
}
