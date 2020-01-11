export interface IAuthenticationTicket {
    readonly id: number;
    readonly userId: number;
    readonly username: string;
    readonly discriminator: string;
    readonly avatarHash: string;
    readonly grantedPermissions: {
        readonly [id: number]: string;
    };
}

export interface IRawAuthenticationTicket {
    readonly avtr?: string;
    readonly dscm: string;
    readonly exp: number;
    readonly iat: number;
    readonly nameid: number;
    readonly nbf: number;
    readonly prms: {
        readonly [id: number]: string;
    };
    readonly tckt: number;
    readonly unique_name: string;
}

export namespace RawAuthenticationTicket {

    export function toTicket(
                rawTicket: IRawAuthenticationTicket):
            IAuthenticationTicket {
        return {
            id: rawTicket.tckt,
            userId: rawTicket.nameid,
            username: rawTicket.unique_name,
            discriminator: rawTicket.dscm,
            avatarHash: rawTicket.avtr,
            grantedPermissions: rawTicket.prms
        };
    }
}
