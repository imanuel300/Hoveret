export interface HashTable<T> {
    [key: string]: T;
}

export interface KeyValuePair<TKey, TValue> {
    Key: TKey;
    Value: TValue;
}
export interface KeyValuePairPlus<TKey, TTitle, TValue> {
    Key: TKey;
    Title: TTitle;
    Value: TValue;
}