export interface IAuthenticationTicket {
    id: number;
    userId: number;
    username: string;
    discriminator: string;
    avatarHash: string;
    grantedPermissions: {
        [id: number]: string;
    };
}

export interface IRawAuthenticationTicket {
    avtr?: string;
    dscm: string;
    exp: number;
    iat: number;
    nameid: number;
    nbf: number;
    prms: {
        [id: number]: string;
    };
    tckt: number;
    unique_name: string;
}
