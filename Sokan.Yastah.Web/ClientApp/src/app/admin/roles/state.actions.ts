import { createAction, props } from "@ngrx/store";

import { IRoleIdentityViewModel } from "./models";


export interface IRoleIdentitiesStorageActionProps {
    readonly identities: IRoleIdentityViewModel[];
}


export namespace RolesActionFactory {

    export const beginFetchIdentities
        = createAction(
            "[Administration][Roles] beginFetchIdentities");

    export const scheduleFetchIdentities
        = createAction(
            "[Administration][Roles] scheduleFetchIdentities");

    export const storeIdentities
        = createAction(
            "[Administration][Roles] storeIdentities",
            props<IRoleIdentitiesStorageActionProps>());
}
