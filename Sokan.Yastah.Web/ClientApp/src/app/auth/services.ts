import { Injectable } from "@angular/core";

import { Store } from "@ngrx/store";

import { Observable } from "rxjs";

import * as JwtDecode from "jwt-decode";

import { IAppState } from "../state";

import { IRawAuthenticationTicket, RawAuthenticationTicket } from "./models";
import { AuthenticationActionFactory } from "./state.actions";
import { AuthenticationSelectors } from "./state.selectors";


const authenticationEndpoint
    = "/api/authentication";

const authenticationTicketHeaderAndPayloadCookieName
    = "Yastah.Api.Authentication.Ticket.HeaderAndPayload";

const signinUri
    = `${authenticationEndpoint}/challenge`;

const signoutUri
    = `${authenticationEndpoint}/signout`;


@Injectable({
    providedIn: "root"
})
export class AuthenticationService {

    public constructor(
            appState: Store<IAppState>) {

        this._appState = appState;

        this._avatarUriObservation = this._appState
            .select(AuthenticationSelectors.avatarUri);

        this._isAuthenticatedObservation = this._appState
            .select(AuthenticationSelectors.isAuthenticated);

        this._usernameObservation = this._appState
            .select(AuthenticationSelectors.username);
    }

    public get observeAvatarUri(): Observable<string | null> {
        return this._avatarUriObservation;
    }
    public get observeIsAuthenticated(): Observable<boolean> {
        return this._isAuthenticatedObservation;
    }
    public get observeUsername(): Observable<string | null> {
        return this._usernameObservation;
    }
    public get signinUri(): string {
        return signinUri;
    }
    public get signoutUri(): string {
        return signoutUri;
    }

    public reloadTicket(): void {
        let headerAndPayloadCookie = document.cookie
            .split(";")
            .map(cookie => cookie.trim())
            .find(cookie => cookie.startsWith(`${authenticationTicketHeaderAndPayloadCookieName}=`));

        if (headerAndPayloadCookie == null) {
            return;
        }

        let headerAndPayload = headerAndPayloadCookie
            .split("=")[1];

        let rawTicket = JwtDecode<IRawAuthenticationTicket>(headerAndPayload);

        if (rawTicket.exp < (new Date().valueOf() / 1000)) {
            return;
        }

        this._appState.dispatch(AuthenticationActionFactory.storeTicket({
            ticket: RawAuthenticationTicket.toTicket(rawTicket)
        }));
    }

    private readonly _appState: Store<IAppState>;
    private readonly _avatarUriObservation: Observable<string | null>;
    private readonly _isAuthenticatedObservation: Observable<boolean>;
    private readonly _usernameObservation: Observable<string | null>;
}

@Injectable({
    providedIn: "root"
})
export class AuthorizationService {

    public constructor(
            appState: Store<IAppState>) {

        this._observeHasAdmin = appState.select(AuthenticationSelectors.hasAdmin);
        this._observeHasAdminManagePermissions = appState.select(AuthenticationSelectors.hasAdminManagePermissions);
        this._observeHasAdminManageRoles = appState.select(AuthenticationSelectors.hasAdminManageRoles);
        this._observeHasAdminManageUsers = appState.select(AuthenticationSelectors.hasAdminManageUsers);
        this._observeHasAnyPermissions = appState.select(AuthenticationSelectors.hasAnyPermissions);
        this._observeHasCharacterManage = appState.select(AuthenticationSelectors.hasCharacterManage);
        this._observeHasCharacterManageGuilds = appState.select(AuthenticationSelectors.hasCharacterManageGuilds);
        this._observeHasCharacterManageLevels = appState.select(AuthenticationSelectors.hasCharacterManageLevels);
    }

    public get observeHasAdmin(): Observable<boolean> {
        return this._observeHasAdmin;
    }
    public get observeHasAdminManagePermissions(): Observable<boolean> {
        return this._observeHasAdminManagePermissions;
    }
    public get observeHasAdminManageRoles(): Observable<boolean> {
        return this._observeHasAdminManageRoles;
    }
    public get observeHasAdminManageUsers(): Observable<boolean> {
        return this._observeHasAdminManageUsers;
    }
    public get observeHasAnyPermissions(): Observable<boolean> {
        return this._observeHasAnyPermissions;
    }
    public get observeHasCharacterManage(): Observable<boolean> {
        return this._observeHasCharacterManage;
    }
    public get observeHasCharacterManageGuilds(): Observable<boolean> {
        return this._observeHasCharacterManageGuilds;
    }
    public get observeHasCharacterManageLevels(): Observable<boolean> {
        return this._observeHasCharacterManageLevels;
    }

    private readonly _observeHasAdmin: Observable<boolean>;
    private readonly _observeHasAdminManagePermissions: Observable<boolean>;
    private readonly _observeHasAdminManageRoles: Observable<boolean>;
    private readonly _observeHasAdminManageUsers: Observable<boolean>;
    private readonly _observeHasAnyPermissions: Observable<boolean>;
    private readonly _observeHasCharacterManage: Observable<boolean>;
    private readonly _observeHasCharacterManageGuilds: Observable<boolean>;
    private readonly _observeHasCharacterManageLevels: Observable<boolean>;
}
