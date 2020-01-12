import { createAction, props } from "@ngrx/store";

import {
    ICharacterGuildDivisionIdentityViewModel,
    ICharacterGuildIdentityViewModel,} from "./models";


export interface IGuildActionProps {
    readonly guildId: number;
}
export interface IGuildDivisionIdentitiesStorageActionProps {
    readonly guildId: number;
    readonly identities: ICharacterGuildDivisionIdentityViewModel[];
}
export interface IGuildIdentitiesStorageActionProps {
    readonly identities: ICharacterGuildIdentityViewModel[];
}


export namespace GuildsActionFactory {

    export const beginFetchIdentities
        = createAction(
            "[GameMaster][Guilds] beginFetchIdentities");

    export const remove
        = createAction(
            "[GameMaster][Guilds] remove",
            props<IGuildActionProps>());

    export const scheduleFetchIdentities
        = createAction(
            "[GameMaster][Guilds] scheduleFetchIdentities");

    export const storeIdentities
        = createAction(
            "[GameMaster][Guilds] storeIdentities",
            props<IGuildIdentitiesStorageActionProps>());
}

export namespace GuildDivisionsActionFactory {

    export const beginFetchIdentities
        = createAction(
            "[GameMaster][GuildDivisions] beginFetchIdentities",
            props<IGuildActionProps>());

    export const scheduleFetchIdentities
        = createAction(
            "[GameMaster][GuildDivisions] scheduleFetchIdentities",
            props<IGuildActionProps>());

    export const storeIdentities
        = createAction(
            "[GameMaster][GuildDivisions] storeIdentities",
            props<IGuildDivisionIdentitiesStorageActionProps>());
}
