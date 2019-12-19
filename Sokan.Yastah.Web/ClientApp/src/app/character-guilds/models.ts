import { IUserIdentityViewModel } from "../admin/users/models";

export interface ICharacterGuildCreationModel {
    readonly name: string;
}

export interface ICharacterGuildDivisionCreationModel {
    readonly name: string;
}

export interface ICharacterGuildDivisionIdentityViewModel {
    readonly id: number;
    readonly name: string;
}

export interface ICharacterGuildIdentityViewModel {
    readonly id: number;
    readonly name: string;
}

export interface ICharacterGuildDivisionUpdateModel {
    readonly name: string;
}

export interface ICharacterGuildUpdateModel {
    readonly name: string;
}

export interface ICharacterIdentityViewModel {
    readonly id: number;
    readonly name: string;
    readonly owner: IUserIdentityViewModel;
}

export namespace CharacterGuildModelConverter {
    export function toUpdate(
                guildIdentity: ICharacterGuildIdentityViewModel | null):
            ICharacterGuildUpdateModel {
        return (guildIdentity == null)
            ? {
                name: null
            }
            : {
                name: guildIdentity.name
            };
    }
}
