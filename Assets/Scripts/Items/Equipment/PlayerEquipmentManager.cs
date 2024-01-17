using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PlayerEquipmentManager : MonoBehaviour {
        InputHandler inputHandler;
        PlayerInventoryManager playerInventoryManager;

        [Header("Equipment Model Changer")]
        HelmetModelChanger helmetModelChanger;
        TorsoModelChanger torsoModelChanger;
        LegModelChanger legModelChanger;
        BootsModelChanger bootsModelChanger;
        GuntletModelChanger guntletModelChanger;
        CapeModelChanger capeModelChanger;
        PlayerStatsManager playerStatsManager;

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
            inputHandler = GetComponent<InputHandler>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();

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
            if (playerInventoryManager.currentHelmetEquipment != null) {
                //nakedHeadModel.SetActive(false);
                helmetModelChanger.EquipHelmetModelByName(playerInventoryManager.currentHelmetEquipment.helmetModelName);
                playerStatsManager.physicalDamageAbsorptionHead = playerInventoryManager.currentHelmetEquipment.physicalDefense;
                //Debug.Log("Head Absorption : " + playerStats.physicalDamageAbsorptionHead + "%");
            } else {
                //nakedHeadModel.SetActive(true);
                helmetModelChanger.EquipHelmetModelByName(nakedHeadModel);
                playerStatsManager.physicalDamageAbsorptionHead = 0;
            }

            torsoModelChanger.UnEquipAllTorsoModels();
            capeModelChanger.UnEquipAllCapeModels();
            if (playerInventoryManager.currentTorsoEquipment != null) {
                torsoModelChanger.EquipTorsoModelByName(playerInventoryManager.currentTorsoEquipment.torsoModelName);
                capeModelChanger.EquipCapeModelByName(playerInventoryManager.currentTorsoEquipment.capeModelName);
                playerStatsManager.physicalDamageAbsorptionBody = playerInventoryManager.currentTorsoEquipment.physicalDefense;
                //Debug.Log("Body Absorption : " + playerStats.physicalDamageAbsorptionBody + "%");
            } else {
                torsoModelChanger.EquipTorsoModelByName(nakedTorsoModel);
                capeModelChanger.EquipCapeModelByName(nakedCapeModel);
                playerStatsManager.physicalDamageAbsorptionBody = 0;
            }

            legModelChanger.UnEquipAllLegModels();
            bootsModelChanger.UnEquipAllBootsModels();
            if (playerInventoryManager.currentLegEquipment != null) {
                legModelChanger.EquipLegModelByName(playerInventoryManager.currentLegEquipment.legModelName);
                bootsModelChanger.EquipBootsModelByName(playerInventoryManager.currentLegEquipment.bootsModelName);
                playerStatsManager.physicalDamageAbsorptionLegs = playerInventoryManager.currentLegEquipment.physicalDefense;
                //Debug.Log("Legs Absorption : " + playerStats.physicalDamageAbsorptionLegs + "%");
            } else {
                legModelChanger.EquipLegModelByName(nakedLegModel);
                bootsModelChanger.EquipBootsModelByName(nakedBootsModel);
                playerStatsManager.physicalDamageAbsorptionLegs = 0;
            }

            guntletModelChanger.UnEquipAllGuntletModels();
            if (playerInventoryManager.currentGuntletEquipment != null) {
                guntletModelChanger.EquipGuntletModelByName(playerInventoryManager.currentGuntletEquipment.guntletModelName);
                playerStatsManager.physicalDamageAbsorptionHands = playerInventoryManager.currentGuntletEquipment.physicalDefense;
                //Debug.Log("Hands Absorption : " + playerStats.physicalDamageAbsorptionHands + "%");
            } else {
                guntletModelChanger.EquipGuntletModelByName(nakedGuntletModel);
                playerStatsManager.physicalDamageAbsorptionHands = 0;
            }
        }

        // 방어용 Collider 적용
        public void OpenBlockingCollider() {
            if (inputHandler.twoHandFlag) { // 양잡시 오른손 무기의 경감율 적용
                blockingCollider.SetColliderDamageAbsorption(playerInventoryManager.rightWeapon);
            } else { // 아닐경우 왼손 무기의 경감율 적용
                blockingCollider.SetColliderDamageAbsorption(playerInventoryManager.leftWeapon);
            }
            blockingCollider.EnableBlockingCollider();
        }

        public void CloseBlockingCollider() {
            blockingCollider.DisableBlockingCollider();
        }
    }
}
