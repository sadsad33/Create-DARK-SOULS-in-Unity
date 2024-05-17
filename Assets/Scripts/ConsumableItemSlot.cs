using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulsLike {
    public class ConsumableItemSlot : MonoBehaviour {
        UIManager uiManager;
        public Image icon;
        ConsumableItem consumableItem;

        public bool slot1;
        public bool slot2;
        public bool slot3;

        private void Awake() {
            uiManager = FindObjectOfType<UIManager>();
        }
    }
}
