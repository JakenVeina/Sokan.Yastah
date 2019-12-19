import { IAdminState } from "./admin/state";
import { IAuthenticationState } from "./authentication/state";
import { ICharacterGuildsState } from "./character-guilds/state";

export interface IAppState {
    readonly admin?: IAdminState;
    readonly authentication: IAuthenticationState;
    readonly characterGuilds: ICharacterGuildsState;
}

export interface IFetchedValue<T> {
    readonly isFetching: boolean;
    readonly needsFetched: boolean;
    readonly value: T;
}

export namespace FetchedValue {

    export function fetched<T>(
                value: T):
            IFetchedValue<T> {
        return {
            isFetching: false,
            needsFetched: false,
            value: value
        };
    }

    export function fetching<T>(
                value: T):
            IFetchedValue<T> {
        return {
            isFetching: true,
            needsFetched: false,
            value: value
        };
    }

    export function unfetched<T>(
                value: T):
            IFetchedValue<T> {
        return {
            isFetching: false,
            needsFetched: true,
            value: value
        };
    }
}
