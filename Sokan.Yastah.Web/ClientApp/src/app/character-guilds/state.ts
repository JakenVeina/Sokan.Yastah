import { FetchedValue, IFetchedValue } from "../state";

import { ICharacterGuildIdentityViewModel, ICharacterGuildDivisionIdentityViewModel } from "./models";

export interface ICharacterGuildsState {
    readonly identities: IFetchedValue<{
        [guildId: number]: IFetchedValue<ICharacterGuildIdentityViewModel | null>;
    }>;
    readonly divisionIdentities: {
        [guildId: number]: IFetchedValue<{
            [divisionId: number]: IFetchedValue<ICharacterGuildDivisionIdentityViewModel | null>;
        }>
    };
}

export const initialCharacterGuildsState: ICharacterGuildsState = {
    identities: FetchedValue.unfetched({}),
    divisionIdentities: {}
};
