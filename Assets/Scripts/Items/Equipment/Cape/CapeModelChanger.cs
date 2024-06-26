using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class CapeModelChanger : MonoBehaviour {
        public List<GameObject> capeModels;

        private void Awake() {
            GetAllCapeModels();
        }

        private void GetAllCapeModels() {
            int capeCount = transform.childCount;
            for (int i = 0; i < capeCount; i++) {
                capeModels.Add(transform.GetChild(i).gameObject);
            }
        }

        public void UnEquipAllCapeModels() {
            for (int i = 0; i < capeModels.Count; i++) {
                capeModels[i].SetActive(false);
            }
        }

        public void EquipCapeModelByName(string capeName) {
            for (int i = 0; i < capeModels.Count; i++) {
                if (capeModels[i].name == capeName) {
                    capeModels[i].SetActive(true);
                }
            }
        }
    }
}
