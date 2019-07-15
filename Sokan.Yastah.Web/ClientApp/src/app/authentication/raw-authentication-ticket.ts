export interface RawAuthenticationTicket {
    avtr: string | undefined;
    dscm: string;
    exp: number;
    iat: number;
    nameid: number;
    nbf: number;
    prms: string[] | undefined;
    unique_name: string;
}
