import { ImmutableObject } from "./immutable-utils";
import { INativeHashTable, NativeHashTable, Reducer } from "./types";


export const enum FetchState {
    unfetched = 0,
    fetching = 1,
    fetched = 2
}

export interface IFetchedValue<T> {
    readonly state: FetchState;
    readonly value: T;
}
export interface IFetchedValueTable<T> {
    readonly state: FetchState,
    readonly valueTable: Readonly<INativeHashTable<IFetchedValue<T>>>
}

export const deletedFetchedValueState: IFetchedValue<any> = {
    state: FetchState.fetched,
    value: undefined
};
export const deletedFetchedValueTableState: IFetchedValueTable<any> = {
    state: FetchState.fetched,
    valueTable: {}
};
export const initialFetchedValueState: IFetchedValue<any> = {
    state: FetchState.unfetched,
    value: undefined
};
export const initialFetchedValueTableState: IFetchedValueTable<any> = {
    state: FetchState.unfetched,
    valueTable: {}
};


export namespace FetchedValue {

    export function mapState<T>(
                source: IFetchedValue<T>,
                stateReducer: Reducer<FetchState>):
            IFetchedValue<T> {
        return ImmutableObject.mapOne(source, "state", stateReducer);
    }

    export function updateState<T>(
                source: IFetchedValue<T>,
                state: FetchState):
            IFetchedValue<T> {
        return ImmutableObject.updateOne(source, "state", state);
    }
}

export namespace FetchedValueTable {

    export function allRowsNeedFetch<T>(
                source: IFetchedValueTable<T>):
            boolean {
        return (source.state === FetchState.unfetched);
    }

    export function keysNeedingFetch<T>(
                source: IFetchedValueTable<T>):
            string[] {
        return (source.state === FetchState.fetching)
            ? []
            : Object.entries(source.valueTable)
                .filter(([_, row]) => row.state === FetchState.unfetched)
                .map(([key]) => key);
    }

    export function fromValues<T>(
                state: FetchState,
                values: Iterable<T>,
                keySelector: (value: T) => number | string):
            IFetchedValueTable<T> {
        return {
            state: state,
            valueTable: NativeHashTable.fromItems(
                values,
                keySelector,
                value => ({
                    state: state,
                    value: value
                }))
        };
    }

    export function mapOneState<T>(
                source: IFetchedValueTable<T>,
                key: number | string,
                stateReducer: Reducer<FetchState>):
            IFetchedValueTable<T> {
        return mapValueTable(source, 
            valueTable => ImmutableObject.mapOne(valueTable, key,
                row => FetchedValue.mapState(row || initialFetchedValueState, stateReducer)));
    }

    export function mapValueTable<T>(
                source: IFetchedValueTable<T>,
                valueTableReducer: Reducer<IFetchedValueTable<T>["valueTable"]>):
            IFetchedValueTable<T> {
        return ImmutableObject.mapOne(source, "valueTable", valueTableReducer);
    }

    export function rowNeedsFetched<T>(
                source: IFetchedValueTable<T>,
                key: number | string):
            boolean {
        return (source.state !== FetchState.fetching)
            && (((source.valueTable[key] && source.valueTable[key].state) || FetchState.unfetched) === FetchState.unfetched);
    }

    export function updateAllStates<T>(
                source: IFetchedValueTable<T>,
                state: FetchState):
            IFetchedValueTable<T> {
        return ImmutableObject.mapOrUpdateMany(source,
            {
                key: "state",
                value: state
            },
            {
                key: "valueTable",
                reducer: valueTable => NativeHashTable.map(valueTable,
                    row => FetchedValue.updateState(row, state))
            });        
    }

    export function updateOneRow<T>(
                source: IFetchedValueTable<T>,
                key: number | string,
                row: IFetchedValue<T>):
            IFetchedValueTable<T> {
        return mapValueTable(source,
            valueTable => ImmutableObject.updateOne(valueTable, key, row));
    }

    export function updateOneState<T>(
                source: IFetchedValueTable<T>,
                key: number | string,
                state: FetchState):
            IFetchedValueTable<T> {
        return mapValueTable(source, 
            valueTable => ImmutableObject.mapOne(valueTable, key,
                row => FetchedValue.updateState(row || initialFetchedValueState, state)));
    }
}
