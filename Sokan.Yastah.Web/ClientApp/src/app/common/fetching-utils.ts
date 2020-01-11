import { ImmutableObject } from "./immutable-utils";
import { Reducer } from "./types";


export const enum FetchState {
    unfetched = 0,
    fetching = 1,
    fetched = 2
}

export interface IFetchedValue<T> {
    readonly state: FetchState;
    readonly value: T;
}


export namespace FetchedValue {

    export function create<T>(
                state: FetchState,
                value: T):
            IFetchedValue<T> {
        return {
            state: state,
            value: value
        };
    }

    export function fetched<T>(
                value: T):
            IFetchedValue<T> {
        return create(FetchState.fetched, value);
    }

    export function mapState<T>(
                source: IFetchedValue<T>,
                stateReducer: Reducer<FetchState>):
            IFetchedValue<T> {
        return ImmutableObject.mapOne(source, "state", stateReducer);
    }

    export function unfetched<T>(
                value: T):
            IFetchedValue<T> {
        return create(FetchState.unfetched, value);
    }

    export function updateState<T>(
                source: IFetchedValue<T>,
                state: FetchState):
            IFetchedValue<T> {
        return ImmutableObject.updateOne(source, "state", state);
    }

    export function updateValue<T>(
                source: IFetchedValue<T>,
                value: T):
            IFetchedValue<T>{
        return ImmutableObject.updateOne(source, "value", value);
    }
}
