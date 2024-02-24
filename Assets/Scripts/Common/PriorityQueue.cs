using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SoulsLike {
    public class PriorityQueue<T> where T : PQNode<T>, IPQNode<T> {

        // PQNode의 priority 값이 작을수록 앞에 배치
        T[] pqueue;
        int usedSize;
        public PriorityQueue(int size) {
            pqueue = new T[size];
            usedSize = 0;
        }

        public void SwapNodes(T a, T b) {

        }

        //public int GetParent(int index) {

        //}

        //public int GetLeftChild(int index) {
        //}

        public void Enqueue(T node) {
            node.PQIndex = usedSize;
            pqueue[usedSize] = node;

        }

        private void Resize(int size) {
            T[] temp = new T[size];
            for (int i = 0; i < usedSize; i++) {
                temp[i] = pqueue[i];
            }
            size *= 2;
            pqueue = new T[size];

            for (int i = 0; i < usedSize; i++) {
                pqueue[i] = temp[i];
            }
        }

        public void Dequeue(out T popped) {
            popped = pqueue[0];

        }

        public int IsEmpty() {
            if (usedSize == 0) return 1;
            return 0;
        }
    }

    public interface IPQNode<T> : IComparable<T> { 
        int PQIndex {
            get; set;
        }
    }
}
