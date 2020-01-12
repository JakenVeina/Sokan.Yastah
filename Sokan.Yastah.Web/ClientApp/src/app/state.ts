import { IAdminState } from "./admin/state";
import { IAuthenticationState } from "./auth/state";
import { IGuildsState } from "./game-master/guilds/state";


export interface IAppState {
    readonly admin?: IAdminState;
    readonly authentication: IAuthenticationState;
    readonly characterGuilds: IGuildsState;
}
