import { Selector } from "@ngrx/store";


export interface AsyncSelector<T, U> {
    (value: T): Promise<U>;
}

export interface FormOnDeletingHandler {
    (): Promise<void>;
}
export interface FormOnResettingHandler<TModel> {
    (isInit: boolean): Promise<TModel>;
}
export interface FormOnSavingHandler<TModel> {
    (model: TModel): Promise<IOperationError | null>;
}

export interface INativeHashTable<T> {
    [key: number]: T;
    [key: string]: T;
};

export interface IOperationError {
    readonly code: string;
    readonly message: string;
}

export interface KeyedReducer<TKey, TValue> {
    (value: TValue, key: TKey): TValue;
}



export namespace NativeHashTable {

    export function fromItems<TItem, TValue>(
                items: Iterable<TItem>,
                keySelector: Selector<TItem, number | string>,
                valueSelector: Selector<TItem, TValue>):
            INativeHashTable<TValue> {
        let result = {};

        for (let item of items) {
            result[keySelector(item)] = valueSelector(item);
        }

        return result;
    }

    export function map<TValue>(
                source: INativeHashTable<TValue>,
                valueReducer: KeyedReducer<string, TValue>):
            INativeHashTable<TValue> {
        let resultHasChanges = false;
        let result: INativeHashTable<TValue> = {};

        for (let key in source) {
            let oldValue = source[key];
            let newValue = valueReducer(oldValue, key);
            resultHasChanges = resultHasChanges || (oldValue !== newValue);
            result[key] = newValue;
        }

        return <Readonly<INativeHashTable<TValue>>>(resultHasChanges
            ? result
            : source);
    }
}

export interface Reducer<T> {
    (value: T): T;
}
