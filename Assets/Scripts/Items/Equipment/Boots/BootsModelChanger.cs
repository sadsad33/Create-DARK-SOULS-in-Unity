using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class BootsModelChanger : MonoBehaviour {
        public List<GameObject> bootsModel;

        private void Awake() {
            GetAllBootsModels();
        }

        private void GetAllBootsModels() {
            int childrenGameObjects = transform.childCount;

            for (int i = 0; i < childrenGameObjects; i++) {
                bootsModel.Add(transform.GetChild(i).gameObject);
            }
        }

        public void UnEquipAllBootsModels() {
            foreach (GameObject helmetModel in bootsModel) {
                helmetModel.SetActive(false);
            }
        }

        public void EquipBootsModelByName(string bootsName) {
            for (int i = 0; i < bootsModel.Count; i++) {
                if (bootsModel[i].name == bootsName) {
                    bootsModel[i].SetActive(true);
                }
            }
        }
    }
}
