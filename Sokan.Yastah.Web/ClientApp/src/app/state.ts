import { IAdminState } from "./admin/state";
import { IAuthenticationState } from "./auth/state";
import { ICharacterGuildsState } from "./character-guilds/state";


export interface IAppState {
    readonly admin?: IAdminState;
    readonly authentication: IAuthenticationState;
    readonly characterGuilds: ICharacterGuildsState;
}
