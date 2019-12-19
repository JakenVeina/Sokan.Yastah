import { Injectable } from "@angular/core";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";

import { IAppState } from "../state";

@Injectable({
    providedIn: "root"
})
export class AuthorizationService {
    public constructor(
            appState: Store<IAppState>) {

        let currentTicket = appState
            .select(x => x.authentication.currentTicket);

        this._hasAdmin = currentTicket
            .pipe(map(t => (t != null)
                && Object.values(t.grantedPermissions).some(p => p.startsWith("Administration."))));

        this._hasAdminManagePermissions = currentTicket
            .pipe(map(t => (t != null)
                && Object.values(t.grantedPermissions).some(p => p === "Administration.ManagePermissions")));

        this._hasAdminManageRoles = currentTicket
            .pipe(map(t => (t != null)
                && Object.values(t.grantedPermissions).some(p => p === "Administration.ManageRoles")));

        this._hasAdminManageUsers = currentTicket
            .pipe(map(t => (t != null)
                && Object.values(t.grantedPermissions).some(p => p === "Administration.ManageUsers")));

        this._hasAnyPermissions = currentTicket
            .pipe(map(t => (t != null)
                && (Object.keys(t.grantedPermissions).length > 0)));

        this._hasCharacterAdminManageGuilds = currentTicket
            .pipe(map(t => (t != null)
                && Object.values(t.grantedPermissions).some(p => p === "CharacterAdministration.ManageGuilds")));
    }

    public get hasAdmin(): Observable<boolean> {
        return this._hasAdmin;
    }

    public get hasAdminManagePermissions(): Observable<boolean> {
        return this._hasAdminManagePermissions;
    }

    public get hasAdminManageRoles(): Observable<boolean> {
        return this._hasAdminManageRoles;
    }

    public get hasAdminManageUsers(): Observable<boolean> {
        return this._hasAdminManageUsers;
    }

    public get hasAnyPermissions(): Observable<boolean> {
        return this._hasAnyPermissions;
    }

    public get hasCharacterAdminManageGuilds(): Observable<boolean> {
        return this._hasCharacterAdminManageGuilds;
    }

    private readonly _hasAdmin: Observable<boolean>;
    private readonly _hasAdminManagePermissions: Observable<boolean>;
    private readonly _hasAdminManageRoles: Observable<boolean>;
    private readonly _hasAdminManageUsers: Observable<boolean>;
    private readonly _hasAnyPermissions: Observable<boolean>;
    private readonly _hasCharacterAdminManageGuilds;
}
