import { IAdminState } from "./admin/state";
import { IAuthenticationState } from "./authentication/state";

export interface IAppState {
    admin?: IAdminState;
    authentication: IAuthenticationState;
}
