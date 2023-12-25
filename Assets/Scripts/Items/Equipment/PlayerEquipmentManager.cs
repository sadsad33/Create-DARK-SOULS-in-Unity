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
        PlayerStats playerStats;

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
            playerStats = GetComponentInParent<PlayerStats>();
        }


        private void Start() {
            EquipAllEquipmentModelsOnStart();
        }

        private void EquipAllEquipmentModelsOnStart() {
            helmetModelChanger.UnEquipAllHelmetModels();
            if (playerInventory.currentHelmetEquipment != null) {
                //nakedHeadModel.SetActive(false);
                helmetModelChanger.EquipHelmetModelByName(playerInventory.currentHelmetEquipment.helmetModelName);
                playerStats.physicalDamageAbsorptionHead = playerInventory.currentHelmetEquipment.physicalDefense;
                //Debug.Log("Head Absorption : " + playerStats.physicalDamageAbsorptionHead + "%");
            } else {
                //nakedHeadModel.SetActive(true);
                helmetModelChanger.EquipHelmetModelByName(nakedHeadModel);
                playerStats.physicalDamageAbsorptionHead = 0;
            }

            torsoModelChanger.UnEquipAllTorsoModels();
            capeModelChanger.UnEquipAllCapeModels();
            if (playerInventory.currentTorsoEquipment != null) {
                torsoModelChanger.EquipTorsoModelByName(playerInventory.currentTorsoEquipment.torsoModelName);
                capeModelChanger.EquipCapeModelByName(playerInventory.currentTorsoEquipment.capeModelName);
                playerStats.physicalDamageAbsorptionBody = playerInventory.currentTorsoEquipment.physicalDefense;
                //Debug.Log("Body Absorption : " + playerStats.physicalDamageAbsorptionBody + "%");
            } else {
                torsoModelChanger.EquipTorsoModelByName(nakedTorsoModel);
                capeModelChanger.EquipCapeModelByName(nakedCapeModel);
                playerStats.physicalDamageAbsorptionBody = 0;
            }

            legModelChanger.UnEquipAllLegModels();
            bootsModelChanger.UnEquipAllBootsModels();
            if (playerInventory.currentLegEquipment != null) {
                legModelChanger.EquipLegModelByName(playerInventory.currentLegEquipment.legModelName);
                bootsModelChanger.EquipBootsModelByName(playerInventory.currentLegEquipment.bootsModelName);
                playerStats.physicalDamageAbsorptionLegs = playerInventory.currentLegEquipment.physicalDefense;
                //Debug.Log("Legs Absorption : " + playerStats.physicalDamageAbsorptionLegs + "%");
            } else {
                legModelChanger.EquipLegModelByName(nakedLegModel);
                bootsModelChanger.EquipBootsModelByName(nakedBootsModel);
                playerStats.physicalDamageAbsorptionLegs = 0;
            }

            guntletModelChanger.UnEquipAllGuntletModels();
            if (playerInventory.currentGuntletEquipment != null) {
                guntletModelChanger.EquipGuntletModelByName(playerInventory.currentGuntletEquipment.guntletModelName);
                playerStats.physicalDamageAbsorptionHands = playerInventory.currentGuntletEquipment.physicalDefense;
                //Debug.Log("Hands Absorption : " + playerStats.physicalDamageAbsorptionHands + "%");
            } else {
                guntletModelChanger.EquipGuntletModelByName(nakedGuntletModel);
                playerStats.physicalDamageAbsorptionHands = 0;
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
