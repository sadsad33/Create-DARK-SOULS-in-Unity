using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class LegModelChanger : MonoBehaviour {
        public List<GameObject> legModels;

        private void Awake() {
            GetAllLegModels();
        }

        private void GetAllLegModels() {
            int childrenGameObjects = transform.childCount;

            for (int i = 0; i < childrenGameObjects; i++) {
                legModels.Add(transform.GetChild(i).gameObject);
            }
        }

        public void UnEquipAllLegModels() {
            foreach (GameObject helmetModel in legModels) {
                helmetModel.SetActive(false);
            }
        }

        public void EquipLegModelByName(string hipName) {
            for (int i = 0; i < legModels.Count; i++) {
                if (legModels[i].name == hipName) {
                    legModels[i].SetActive(true);
                }
            }
        }
    }
}