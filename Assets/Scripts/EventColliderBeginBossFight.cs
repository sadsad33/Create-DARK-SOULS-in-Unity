using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    // ������ �̺�Ʈ Ʈ����
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