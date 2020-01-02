import { Injectable } from "@angular/core";
import { CanActivate } from "@angular/router";

import { Observable } from "rxjs";

import { AuthorizationService } from "../authorization/authorization-service";


@Injectable({
    providedIn: "root"
})
export class CharacterGuildsGuard
        implements CanActivate {

    public constructor(
            authorizationService: AuthorizationService) {
        this._authorizationService = authorizationService;
    }

    public canActivate(): Observable<boolean> {
        return this._authorizationService.hasCharacterAdminManageGuilds;
    }

    private readonly _authorizationService: AuthorizationService;
}
