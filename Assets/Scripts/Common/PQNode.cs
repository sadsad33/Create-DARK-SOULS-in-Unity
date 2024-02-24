using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class PQNode<T> {
        T item;
        int priority;

        public PQNode(T _item, int _priority) {
            item = _item;
            priority = _priority;
        }

        public T GetItem() {
            return item;
        }

        public int GetPriority() {
            return priority;
        }
    }
}
