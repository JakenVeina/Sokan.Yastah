import { createSelector, MemoizedSelector } from "@ngrx/store";

import { FetchState } from "../../common/fetching-utils";
import { IAppState } from "../../state";

import { AdminSelectors } from "../state.selectors";

import { IPermissionCategoryDescriptionViewModel } from "./models";


export namespace PermissionsSelectors {

    export const descriptions: MemoizedSelector<IAppState, IPermissionCategoryDescriptionViewModel[] | null>
        = createSelector(
            AdminSelectors.permissionsState,
            state => state.descriptions.value);

    export const descriptionsNeedsFetched: MemoizedSelector<IAppState, boolean>
        = createSelector(
            AdminSelectors.permissionsState,
            state => state.descriptions.state === FetchState.unfetched);
}
