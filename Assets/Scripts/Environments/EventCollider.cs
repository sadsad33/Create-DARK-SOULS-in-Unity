using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    // 이벤트 트리거
    public class EventCollider : MonoBehaviour {
        public WorldEvent worldEvent;
        
        // 발생시킬 이벤트
        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Character")) {
                WorldEventManager.instance.ProcessCurrentWorldEvent(worldEvent);
            }
        }
    }
}
