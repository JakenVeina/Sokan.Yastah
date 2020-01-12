import { createFeatureSelector, createSelector, MemoizedSelector } from "@ngrx/store";

import { IAppState } from "../state";

import { IGuildsState } from "./guilds/state";

import { IGameMasterState } from "./state";


const gameMasterState
    = createFeatureSelector<IAppState, IGameMasterState>("gameMaster");


export namespace GameMasterSelectors {

    export const guildsState: MemoizedSelector<IAppState, IGuildsState>
        = createSelector(
            gameMasterState,
            state => state.guilds);
}
