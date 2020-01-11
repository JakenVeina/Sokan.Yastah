import { Reducer } from "./types";


export interface KeyValueMapping<TKey, TValue> {
    key: TKey;
    value?: TValue;
    reducer?: Reducer<TValue>
}


export namespace ImmutableObject {

    export function mapOrUpdateMany<TSource, TKey1 extends keyof TSource, TKey2 extends keyof TSource>(
            source: TSource,
            mapping1: KeyValueMapping<TKey1, TSource[TKey1]>,
            mapping2: KeyValueMapping<TKey2, TSource[TKey2]>):
        Readonly<TSource>;
    export function mapOrUpdateMany<TSource>(
                source: TSource,
                ...mappings: KeyValueMapping<keyof TSource, TSource[keyof TSource]>[]):
            Readonly<TSource> {
        let updatesHasChanges = false;
        let updates: Partial<TSource> = {};

        for (let mapping of mappings) {
            let oldValue = source[mapping.key];
            let newValue = (typeof mapping.value !== "undefined") ? mapping.value
                : (typeof mapping.reducer !== "undefined") ? mapping.reducer(oldValue)
                : undefined;
            updatesHasChanges = updatesHasChanges || (oldValue !== newValue);
            updates[mapping.key] = newValue;
        }

        return updatesHasChanges
            ? {
                ...source,
                ...updates
            }
            : source;
    }

    export function mapOne<TSource, TKey extends keyof TSource>(
                source: TSource,
                key: TKey,
                valueReducer: Reducer<TSource[TKey]>):
            Readonly<TSource> {
        return updateOne(source, key, valueReducer(source[key]));
    }

    export function removeOne<TSource, TKey extends keyof TSource>(
                source: TSource,
                key: TKey):
            Readonly<Exclude<TSource, TKey>> {
        if (typeof source[key] !== "undefined") {
            return <Readonly<Exclude<TSource, TKey>>>source;
        }

        const {
            [key]: _,
            ...result
        } = source;

        return <Readonly<Exclude<TSource, TKey>>>result;
    }

    export function updateOne<TSource, TKey extends keyof TSource>(
                source: TSource,
                key: TKey,
                value: TSource[TKey]):
            Readonly<TSource> {
        return (source[key] === value)
            ? source
            : {
                ...source,
                [key]: value
            };
    }
}
