using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PlayerEquipmentManager : MonoBehaviour {
        InputHandler inputHandler;
        PlayerInventory playerInventory;

        [Header("Equipment Model Changer")]
        HelmetModelChanger helmetModelChanger;
        TorsoModelChanger torsoModelChanger;

        [Header("Default Naked Models")]
        //public GameObject nakedHeadModel;
        public string nakedHeadModel;
        public string nakedTorsoModel;

        public BlockingCollider blockingCollider;

        private void Awake() {
            inputHandler = GetComponentInParent<InputHandler>();
            playerInventory = GetComponentInParent<PlayerInventory>();
            helmetModelChanger = GetComponentInChildren<HelmetModelChanger>();
            torsoModelChanger = GetComponentInChildren<TorsoModelChanger>();
        }

        private void Start() {
            EquipAllEquipmentModelsOnStart();
        }

        private void EquipAllEquipmentModelsOnStart() {
            helmetModelChanger.UnEquipAllHelmetModels();
            if (playerInventory.currentHelmetEquipment != null) {
                //nakedHeadModel.SetActive(false);
                helmetModelChanger.EquipHelmetModelByName(playerInventory.currentHelmetEquipment.helmetModelName);
            } else {
                //nakedHeadModel.SetActive(true);
                Debug.Log("Player Hair");
                helmetModelChanger.EquipHelmetModelByName(nakedHeadModel);
            }

            torsoModelChanger.UnEquipAllTorsoModels();
            if (playerInventory.currentTorsoEquipment != null) {
                torsoModelChanger.EquipTorsoModelByName(playerInventory.currentTorsoEquipment.torsoModelName);
            } else {
                torsoModelChanger.EquipTorsoModelByName(nakedTorsoModel);
            }
        }

        // 방어용 Collider 적용
        public void OpenBlockingCollider() {
            if (inputHandler.twoHandFlag) { // 양잡시 오른손 무기의 경감율 적용
                blockingCollider.SetColliderDamageAbsorption(playerInventory.rightWeapon);
            } else { // 아닐경우 왼손 무기의 경감율 적용
                blockingCollider.SetColliderDamageAbsorption(playerInventory.leftWeapon);
            }
            blockingCollider.EnableBlockingCollider();
        }

        public void CloseBlockingCollider() {
            blockingCollider.DisableBlockingCollider();
        }
    }
}
