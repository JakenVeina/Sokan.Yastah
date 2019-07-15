import { RawAuthenticationTicket } from "./raw-authentication-ticket";

export class AuthenticationTicket {
    public constructor(rawTicket: RawAuthenticationTicket) {
        this._rawTicket = rawTicket;
        this._permissions = {};
        if (this._rawTicket.prms != null) {
            this._rawTicket.prms.forEach(prm => this._permissions[prm] = null);
        }
    }

    public get avatarHash(): string | null {
        return this._rawTicket.avtr || null;
    }

    public get hasAnyPermissions(): boolean {
        for (var x in this._permissions) {
            if (this._permissions.hasOwnProperty(x)) {
                return true;
            }
        }
        return false;
    }

    public get userId(): number | null {
        return (this._rawTicket != null)
            ? this._rawTicket.nameid
            : null;
    }

    public get userName(): string | null {
        return (this._rawTicket != null)
            ? this._rawTicket.unique_name
            : null;
    }

    public hasPermission(permission: string): boolean {
        return (typeof this._permissions[permission] !== "undefined");
    }

    private readonly _permissions: object;

    private readonly _rawTicket: RawAuthenticationTicket;
}
