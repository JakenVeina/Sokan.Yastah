export interface AsyncSelector<T, U> {
    (value: T): Promise<U>;
}

export interface INativeHashTable<T> {
    [key: number]: T;
    [key: string]: T;
};

export interface IOperationError {
    readonly code: string;
    readonly message: string;
}

export interface Reducer<T> {
    (value: T): T;
}
