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
        LegModelChanger legModelChanger;
        BootsModelChanger bootsModelChanger;
        GuntletModelChanger guntletModelChanger;
        CapeModelChanger capeModelChanger;

        [Header("Default Naked Models")]
        //public GameObject nakedHeadModel;
        public string nakedHeadModel;
        public string nakedTorsoModel;
        public string nakedLegModel;
        public string nakedBootsModel;
        public string nakedGuntletModel;
        public string nakedCapeModel;

        public BlockingCollider blockingCollider;

        private void Awake() {
            inputHandler = GetComponentInParent<InputHandler>();
            playerInventory = GetComponentInParent<PlayerInventory>();
            helmetModelChanger = GetComponentInChildren<HelmetModelChanger>();
            torsoModelChanger = GetComponentInChildren<TorsoModelChanger>();
            legModelChanger = GetComponentInChildren<LegModelChanger>();
            bootsModelChanger = GetComponentInChildren<BootsModelChanger>();
            guntletModelChanger = GetComponentInChildren<GuntletModelChanger>();
            capeModelChanger = GetComponentInChildren<CapeModelChanger>();
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
                helmetModelChanger.EquipHelmetModelByName(nakedHeadModel);
            }

            torsoModelChanger.UnEquipAllTorsoModels();
            capeModelChanger.UnEquipAllCapeModels();
            if (playerInventory.currentTorsoEquipment != null) {
                torsoModelChanger.EquipTorsoModelByName(playerInventory.currentTorsoEquipment.torsoModelName);
                capeModelChanger.EquipCapeModelByName(playerInventory.currentTorsoEquipment.capeModelName);
            } else {
                torsoModelChanger.EquipTorsoModelByName(nakedTorsoModel);
                capeModelChanger.EquipCapeModelByName(nakedCapeModel);
            }

            legModelChanger.UnEquipAllLegModels();
            bootsModelChanger.UnEquipAllBootsModels();
            if (playerInventory.currentLegEquipment != null) {
                legModelChanger.EquipLegModelByName(playerInventory.currentLegEquipment.legModelName);
                bootsModelChanger.EquipBootsModelByName(playerInventory.currentLegEquipment.bootsModelName);
            } else {
                legModelChanger.EquipLegModelByName(nakedLegModel);
                bootsModelChanger.EquipBootsModelByName(nakedBootsModel);
            }

            guntletModelChanger.UnEquipAllGuntletModels();
            if (playerInventory.currentGuntletEquipment != null) {
                guntletModelChanger.EquipGuntletModelByName(playerInventory.currentGuntletEquipment.guntletModelName);
            } else {
                guntletModelChanger.EquipGuntletModelByName(nakedGuntletModel);
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
