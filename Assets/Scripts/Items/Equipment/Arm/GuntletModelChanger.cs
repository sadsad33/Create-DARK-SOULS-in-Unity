using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class GuntletModelChanger : MonoBehaviour {
        public List<GameObject> guntletModels;

        private void Awake() {
            GetAllGuntletModels();
            Debug.Log("hi");
        }

        private void GetAllGuntletModels() {
            int childrenGameObjects = transform.childCount;

            for (int i = 0; i < childrenGameObjects; i++) {
                guntletModels.Add(transform.GetChild(i).gameObject);
            }
        }

        public void UnEquipAllGuntletModels() {
            for (int i = 0; i < guntletModels.Count; i++) {
                guntletModels[i].SetActive(false);
            }
        }

        public void EquipGuntletModelByName(string guntletName) {
            for (int i = 0; i < guntletModels.Count; i++) {
                if (guntletModels[i].name == guntletName) {
                    guntletModels[i].SetActive(true);
                }
            }
        }
    }
}
