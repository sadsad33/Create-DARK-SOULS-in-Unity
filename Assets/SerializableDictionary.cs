using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace SoulsLike {
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue> , ISerializationCallbackReceiver{
        [SerializeField] private List<TKey> keys = new List<TKey>();
        [SerializeField] private List<TValue> values = new List<TValue>();

        // 직렬화 바로 전에 호출
        // 딕셔너리를 리스트로 저장
        public void OnBeforeSerialize() {
            keys.Clear();
            values.Clear();

            foreach(KeyValuePair<TKey, TValue> pair in this){
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        // 직렬화 바로 이후에 호출
        // 리스트로부터 딕셔너리를 로드
        public void OnAfterDeserialize() {
            Clear();
            if (keys.Count != values.Count) {
                Debug.LogError("딕셔너리를 역직렬화 하려고 시도했으나, key 값들의 개수가 value 값들의 개수와 맞지 않습니다");
            }

            for (int i = 0; i < keys.Count; i++) {
                Add(keys[i], values[i]);
            }
        }
    }
}
