import { createAction, props } from "@ngrx/store";

import {
    ICharacterGuildDivisionIdentityViewModel,
    ICharacterGuildIdentityViewModel,} from "./models";


export interface ICharacterGuildActionProps {
    readonly guildId: number;
}
export interface ICharacterGuildDivisionIdentitiesStorageActionProps {
    readonly guildId: number;
    readonly identities: ICharacterGuildDivisionIdentityViewModel[];
}
export interface ICharacterGuildIdentitiesStorageActionProps {
    readonly identities: ICharacterGuildIdentityViewModel[];
}


export namespace CharacterGuildsActionFactory {

    export const beginFetchIdentities
        = createAction(
            "[CharacterGuilds] beginFetchIdentities");

    export const remove
        = createAction(
            "[CharacterGuilds] remove",
            props<ICharacterGuildActionProps>());

    export const scheduleFetchIdentities
        = createAction(
            "[CharacterGuilds] scheduleFetchIdentities");

    export const storeIdentities
        = createAction(
            "[CharacterGuilds] storeIdentities",
            props<ICharacterGuildIdentitiesStorageActionProps>());
}

export namespace CharacterGuildDivisionsActionFactory {

    export const beginFetchIdentities
        = createAction(
            "[CharacterGuildDivisions] beginFetchIdentities",
            props<ICharacterGuildActionProps>());

    export const scheduleFetchIdentities
        = createAction(
            "[CharacterGuildDivisions] scheduleFetchIdentities",
            props<ICharacterGuildActionProps>());

    export const storeIdentities
        = createAction(
            "[CharacterGuildDivisions] storeIdentities",
            props<ICharacterGuildDivisionIdentitiesStorageActionProps>());
}
