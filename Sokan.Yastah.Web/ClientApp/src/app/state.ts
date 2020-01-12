import { IAdminState } from "./admin/state";
import { IAuthenticationState } from "./auth/state";
import { IGameMasterState } from "./game-master/state";


export interface IAppState {
    readonly admin?: IAdminState;
    readonly authentication: IAuthenticationState;
    readonly gameMaster?: IGameMasterState;
}
