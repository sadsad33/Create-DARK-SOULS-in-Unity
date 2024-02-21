using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    // 보스전 이벤트 트리거
    public class EventColliderBeginBossFight : MonoBehaviour {
        WorldEventManager worldEventManager;

        private void Awake() {
            worldEventManager = FindObjectOfType<WorldEventManager>();
        }

        private void OnTriggerEnter(Collider other) {
            if (other.tag == "Player") {
                worldEventManager.ActivateBossFight();
            }
        }
    }
}
