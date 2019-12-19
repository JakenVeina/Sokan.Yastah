import { createAction, props } from "@ngrx/store";
import { ICharacterGuildIdentityViewModel, ICharacterGuildDivisionIdentityViewModel } from "./models";

export namespace CharacterGuildsActionFactory {
    export const beginFetchDivisionIdentities
        = createAction(
            "[CharacterGuilds] BeginFetchDivisionIdentities",
            props<{
                readonly guildId: number
            }>());

    export const beginFetchDivisionIdentity
        = createAction(
            "[CharacterGuilds] BeginFetchDivisionIdentity",
            props<{
                readonly guildId: number,
                readonly divisionId: number
            }>());

    export const beginFetchIdentities
        = createAction(
            "[CharacterGuilds] BeginFetchIdentities");

    export const beginFetchIdentity
        = createAction(
            "[CharacterGuilds] BeginFetchIdentity",
            props<{
                readonly guildId: number
            }>());

    export const endFetchDivisionIdentities
        = createAction(
            "[CharacterGuilds] EndFetchDivisionIdentities",
            props<{
                readonly guildId: number,
                readonly divisionIdentities: ICharacterGuildDivisionIdentityViewModel[]
            }>());

    export const endFetchDivisionIdentity
        = createAction(
            "[CharacterGuilds] EndFetchDivisionIdentity",
            props<{
                readonly guildId: number,
                readonly divisionIdentity: ICharacterGuildDivisionIdentityViewModel
            }>());

    export const endFetchIdentities
        = createAction(
            "[CharacterGuilds] EndFetchIdentities",
            props<{
                readonly identities: ICharacterGuildIdentityViewModel[]
            }>());

    export const endFetchIdentity
        = createAction(
            "[CharacterGuilds] EndFetchIdentity",
            props<{
                readonly identity: ICharacterGuildIdentityViewModel
            }>());

    export const removeDivisionIdentity
        = createAction(
            "[CharacterGuilds] RemoveDivisionIdentity",
            props<{
                readonly guildId: number,
                readonly divisionId: number
            }>());

    export const removeIdentity
        = createAction(
            "[CharacterGuilds] RemoveIdentity",
            props<{
                readonly guildId: number
            }>());

    export const scheduleFetchDivisionIdentities
        = createAction(
            "[CharacterGuilds] ScheduleFetchDivisionIdentities",
            props<{
                readonly guildId: number
            }>());

    export const scheduleFetchDivisionIdentity
        = createAction(
            "[CharacterGuilds] ScheduleFetchDivisionIdentity",
            props<{
                readonly guildId: number,
                readonly divisionId: number
            }>());

    export const scheduleFetchIdentities
        = createAction(
            "[CharacterGuilds] ScheduleFetchIdentities");

    export const scheduleFetchIdentity
        = createAction(
            "[CharacterGuilds] ScheduleFetchIdentity",
            props<{
                readonly guildId: number
            }>());

    export const tryFetchDivisionIdentities
        = createAction(
            "[CharacterGuilds] TryFetchDivisionIdentities",
            props<{
                readonly guildId: number
            }>());

    export const tryFetchDivisionIdentity
        = createAction(
            "[CharacterGuilds] TryFetchDivisionIdentity",
            props<{
                readonly guildId: number,
                readonly divisionId: number
            }>());

    export const tryFetchIdentities
        = createAction(
            "[CharacterGuilds] TryFetchIdentities");

    export const tryFetchIdentity
        = createAction(
            "[CharacterGuilds] TryFetchIdentity",
            props<{
                readonly guildId: number
            }>());
}
