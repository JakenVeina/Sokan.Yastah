import { FetchedValue, IFetchedValue } from "../../common/fetching-utils";
import { ImmutableObject } from "../../common/immutable-utils";
import { Reducer } from "../../common/types";

import { IPermissionCategoryDescriptionViewModel } from "./models";


export interface IPermissionsState {
    readonly descriptions: IFetchedValue<IPermissionCategoryDescriptionViewModel[] | null>;
}


export const initialPermissionsState: IPermissionsState = {
    descriptions: FetchedValue.unfetched(null)
};


export namespace PermissionsState {

    export function mapDescriptions(
                state: IPermissionsState,
                descriptionsReducer: Reducer<IPermissionsState["descriptions"]>):
            IPermissionsState {
        return ImmutableObject.mapOne(state, "descriptions", descriptionsReducer);
    }

    export function updateDescriptions(
                state: IPermissionsState,
                descriptions: IPermissionsState["descriptions"]):
            IPermissionsState {
        return ImmutableObject.updateOne(state, "descriptions", descriptions);
    }
}
