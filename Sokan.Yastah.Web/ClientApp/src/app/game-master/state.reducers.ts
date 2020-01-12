import { ActionReducerMap, Action } from "@ngrx/store";

import { guildsStateReducer } from "./guilds/state.reducers";

import { IGameMasterState } from "./state";


export const gameMasterStateReducers: ActionReducerMap<IGameMasterState, Action> = {
    guilds: guildsStateReducer
};
