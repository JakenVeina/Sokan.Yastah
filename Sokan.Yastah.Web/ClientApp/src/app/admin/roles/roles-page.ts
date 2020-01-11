import { Component } from "@angular/core";

import { Observable } from "rxjs";

import { IRoleIdentityViewModel } from "./models";
import { RolesService } from "./services";


@Component({
    selector: "roles-page",
    templateUrl: "./roles-page.ts.html",
    styleUrls: ["./roles-page.ts.css"]
})
export class RolesPage {

    public constructor(
            rolesService: RolesService) {

        this._roles = rolesService.observeIdentities();

        setTimeout(() => rolesService.fetchIdentities());
    }

    public get roles(): Observable<IRoleIdentityViewModel[]> {
        return this._roles;
    }

    public roleTrackByFn(role: IRoleIdentityViewModel): number {
        return role.id;
    }

    private readonly _roles: Observable<IRoleIdentityViewModel[]>;
}
