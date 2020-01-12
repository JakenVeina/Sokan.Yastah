import { ImmutableObject } from "../common/immutable-utils";
import { Reducer } from "../common/types";

import { initialGuildsState, IGuildsState } from "./guilds/state";


export interface IGameMasterState {
    readonly guilds: IGuildsState
}


export const initialGameMasterState: IGameMasterState = {
    guilds: initialGuildsState
};


export namespace GameMasterState {

    export function mapGuilds(
                state: IGameMasterState,
                guildsReducer: Reducer<IGameMasterState["guilds"]>):
            IGameMasterState {
        return ImmutableObject.mapOne(state, "guilds", guildsReducer);
    }
}
