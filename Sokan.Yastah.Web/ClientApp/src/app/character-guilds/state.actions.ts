import { createAction, props } from "@ngrx/store";

import {
    ICharacterGuildDivisionIdentityViewModel,
    ICharacterGuildIdentityViewModel,} from "./models";


export interface ICharacterGuildActionProps {
    readonly guildId: number;
}
export interface ICharacterGuildDivisionActionProps {
    readonly guildId: number;
    readonly divisionId: number;
}
export interface ICharacterGuildDivisionIdentitiesStorageActionProps {
    readonly guildId: number;
    readonly identities: ICharacterGuildDivisionIdentityViewModel[];
}
export interface ICharacterGuildDivisionIdentityStorageActionProps {
    readonly guildId: number;
    readonly identity: ICharacterGuildDivisionIdentityViewModel;
}
export interface ICharacterGuildIdentitiesStorageActionProps {
    readonly identities: ICharacterGuildIdentityViewModel[];
}
export interface ICharacterGuildIdentityStorageActionProps {
    readonly identity: ICharacterGuildIdentityViewModel;
}


export namespace CharacterGuildsActionFactory {

    export const beginFetchIdentities
        = createAction(
            "[CharacterGuilds] beginFetchIdentities");

    export const beginFetchIdentity
        = createAction(
            "[CharacterGuilds] beginFetchIdentity",
            props<ICharacterGuildActionProps>());

    export const initialize
        = createAction(
            "[CharacterGuilds] initialize",
            props<ICharacterGuildActionProps>());

    export const remove
        = createAction(
            "[CharacterGuilds] remove",
            props<ICharacterGuildActionProps>());

    export const scheduleFetchIdentities
        = createAction(
            "[CharacterGuilds] scheduleFetchIdentities");

    export const scheduleFetchIdentity
        = createAction(
            "[CharacterGuilds] scheduleFetchIdentity",
            props<ICharacterGuildActionProps>());

    export const storeIdentities
        = createAction(
            "[CharacterGuilds] storeIdentities",
            props<ICharacterGuildIdentitiesStorageActionProps>());

    export const storeIdentity
        = createAction(
            "[CharacterGuilds] storeIdentity",
            props<ICharacterGuildIdentityStorageActionProps>());
}

export namespace CharacterGuildDivisionsActionFactory {

    export const beginFetchIdentities
        = createAction(
            "[CharacterGuildDivisions] beginFetchIdentities",
            props<ICharacterGuildActionProps>());

    export const beginFetchIdentity
        = createAction(
            "[CharacterGuildDivisions] beginFetchIdentity",
            props<ICharacterGuildDivisionActionProps>());

    export const initialize
        = createAction(
            "[CharacterGuildDivisions] initialize",
            props<ICharacterGuildDivisionActionProps>());

    export const remove
        = createAction(
            "[CharacterGuildDivisions] remove",
            props<ICharacterGuildDivisionActionProps>());

    export const scheduleFetchIdentities
        = createAction(
            "[CharacterGuildDivisions] scheduleFetchIdentities",
            props<ICharacterGuildActionProps>());

    export const scheduleFetchIdentity
        = createAction(
            "[CharacterGuildDivisions] scheduleFetchIdentity",
            props<ICharacterGuildDivisionActionProps>());

    export const storeIdentities
        = createAction(
            "[CharacterGuildDivisions] storeIdentities",
            props<ICharacterGuildDivisionIdentitiesStorageActionProps>());

    export const storeIdentity
        = createAction(
            "[CharacterGuildDivisions] storeIdentity",
            props<ICharacterGuildDivisionIdentityStorageActionProps>());
}
