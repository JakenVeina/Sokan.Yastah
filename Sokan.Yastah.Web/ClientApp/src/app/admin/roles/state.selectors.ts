import { createSelector, MemoizedSelector } from "@ngrx/store";

import { FetchState } from "../../common/fetching-utils";
import { IAppState } from "../../state";

import { AdminSelectors } from "../state.selectors";

import { IRoleIdentityViewModel } from "./models";


export namespace RolesSelectors {

    export const identities: MemoizedSelector<IAppState, IRoleIdentityViewModel[] | null>
        = createSelector(
            AdminSelectors.rolesState,
            state => state.identities.value);

    export const identitiesIsFetching: MemoizedSelector<IAppState, boolean>
        = createSelector(
            AdminSelectors.rolesState,
            state => state.identities.state === FetchState.fetching);

    export const identitiesNeedsFetch: MemoizedSelector<IAppState, boolean>
        = createSelector(
            AdminSelectors.rolesState,
            state => state.identities.state === FetchState.unfetched);

    export function identity(
                roleId: number):
            MemoizedSelector<IAppState, IRoleIdentityViewModel | null>{
        return createSelector(
            AdminSelectors.rolesState,
            state => state.identities.value
                && state.identities.value.find(identity => identity.id === roleId));
    }
}
