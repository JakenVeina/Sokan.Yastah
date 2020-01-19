import { Injectable } from "@angular/core";
import { CanActivate } from "@angular/router";

import { Observable } from "rxjs";

import { AuthorizationService } from "../auth/services";


@Injectable({
    providedIn: "root"
})
export class GuildsGuard
        implements CanActivate {

    public constructor(
            authorizationService: AuthorizationService) {
        this._authorizationService = authorizationService;
    }

    public canActivate(): Observable<boolean> {
        return this._authorizationService.observeHasCharacterAdminManageGuilds;
    }

    private readonly _authorizationService: AuthorizationService;
}

@Injectable({
    providedIn: "root"
})
export class LevelsGuard
    implements CanActivate {

    public constructor(
        authorizationService: AuthorizationService) {
        this._authorizationService = authorizationService;
    }

    public canActivate(): Observable<boolean> {
        return this._authorizationService.observeHasCharacterAdminManageLevels;
    }

    private readonly _authorizationService: AuthorizationService;
}
