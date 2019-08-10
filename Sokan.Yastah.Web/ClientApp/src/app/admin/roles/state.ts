import { IRoleIdentityViewModel } from "./models";

export interface IRolesState {
    identities: IRoleIdentityViewModel[];
}

export const initialRolesState: IRolesState = {
    identities: [],
};
