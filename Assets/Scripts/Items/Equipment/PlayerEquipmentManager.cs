using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class PlayerEquipmentManager : MonoBehaviour {
        InputHandler inputHandler;
        PlayerManager player;
        //PlayerInventoryManager playerInventoryManager;

        [Header("Equipment Model Changer")]
        HelmetModelChanger helmetModelChanger;
        TorsoModelChanger torsoModelChanger;
        LegModelChanger legModelChanger;
        BootsModelChanger bootsModelChanger;
        GuntletModelChanger guntletModelChanger;
        CapeModelChanger capeModelChanger;
        //PlayerStatsManager playerStatsManager;

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
            player = GetComponent<PlayerManager>();

            helmetModelChanger = GetComponentInChildren<HelmetModelChanger>();
            torsoModelChanger = GetComponentInChildren<TorsoModelChanger>();
            legModelChanger = GetComponentInChildren<LegModelChanger>();
            bootsModelChanger = GetComponentInChildren<BootsModelChanger>();
            guntletModelChanger = GetComponentInChildren<GuntletModelChanger>();
            capeModelChanger = GetComponentInChildren<CapeModelChanger>();
        }

        private void Start() {
            EquipAllEquipmentModels();
        }

        // 시작할때 플레이어에게 설정된 장비를 모두 장착한다
        public void EquipAllEquipmentModels() {
            float poisonResistance = 0;
            helmetModelChanger.UnEquipAllHelmetModels();

            if (player.playerInventoryManager.currentHelmetEquipment != null) {
                if (player.IsOwner) {
                    player.playerNetworkManager.currentHeadEquipmentID.Value = player.playerInventoryManager.currentHelmetEquipment.itemID;
                }
                //nakedHeadModel.SetActive(false);
                helmetModelChanger.EquipHelmetModelByName(player.playerInventoryManager.currentHelmetEquipment.helmetModelName);
                player.playerStatsManager.physicalDamageAbsorptionHead = player.playerInventoryManager.currentHelmetEquipment.physicalDefense;
                poisonResistance += player.playerInventoryManager.currentHelmetEquipment.poisonResistance;
            } else {
                //nakedHeadModel.SetActive(true);
                helmetModelChanger.EquipHelmetModelByName(nakedHeadModel);
                player.playerStatsManager.physicalDamageAbsorptionHead = 0;
                if (player.IsOwner) {
                    player.playerNetworkManager.currentHeadEquipmentID.Value = -1;
                }
            }

            torsoModelChanger.UnEquipAllTorsoModels();
            capeModelChanger.UnEquipAllCapeModels();
            
            if (player.playerInventoryManager.currentTorsoEquipment != null) {
                if (player.IsOwner) {
                    player.playerNetworkManager.currentTorsoEquipmentID.Value = player.playerInventoryManager.currentTorsoEquipment.itemID;
                }
                torsoModelChanger.EquipTorsoModelByName(player.playerInventoryManager.currentTorsoEquipment.torsoModelName);
                capeModelChanger.EquipCapeModelByName(player.playerInventoryManager.currentTorsoEquipment.capeModelName);
                player.playerStatsManager.physicalDamageAbsorptionBody = player.playerInventoryManager.currentTorsoEquipment.physicalDefense;
                poisonResistance += player.playerInventoryManager.currentTorsoEquipment.poisonResistance;
            } else {
                torsoModelChanger.EquipTorsoModelByName(nakedTorsoModel);
                capeModelChanger.EquipCapeModelByName(nakedCapeModel);
                player.playerStatsManager.physicalDamageAbsorptionBody = 0;
                if (player.IsOwner) {
                    player.playerNetworkManager.currentTorsoEquipmentID.Value = -1;
                }
            }

            legModelChanger.UnEquipAllLegModels();
            bootsModelChanger.UnEquipAllBootsModels();
            if (player.playerInventoryManager.currentLegEquipment != null) {
                if (player.IsOwner) {
                    player.playerNetworkManager.currentLegEquipmentID.Value = player.playerInventoryManager.currentLegEquipment.itemID;
                }
                legModelChanger.EquipLegModelByName(player.playerInventoryManager.currentLegEquipment.legModelName);
                bootsModelChanger.EquipBootsModelByName(player.playerInventoryManager.currentLegEquipment.bootsModelName);
                player.playerStatsManager.physicalDamageAbsorptionLegs = player.playerInventoryManager.currentLegEquipment.physicalDefense;
                poisonResistance += player.playerInventoryManager.currentLegEquipment.poisonResistance;
            } else {
                legModelChanger.EquipLegModelByName(nakedLegModel);
                bootsModelChanger.EquipBootsModelByName(nakedBootsModel);
                player.playerStatsManager.physicalDamageAbsorptionLegs = 0;
                if (player.IsOwner) {
                    player.playerNetworkManager.currentLegEquipmentID.Value = -1;
                }
            }

            guntletModelChanger.UnEquipAllGuntletModels();
            if (player.playerInventoryManager.currentGuntletEquipment != null) {
                if (player.IsOwner) {
                    player.playerNetworkManager.currentGuntletEquipmentID.Value = player.playerInventoryManager.currentGuntletEquipment.itemID;
                }
                guntletModelChanger.EquipGuntletModelByName(player.playerInventoryManager.currentGuntletEquipment.guntletModelName);
                player.playerStatsManager.physicalDamageAbsorptionHands = player.playerInventoryManager.currentGuntletEquipment.physicalDefense;
                poisonResistance += player.playerInventoryManager.currentGuntletEquipment.poisonResistance;
            } else {
                guntletModelChanger.EquipGuntletModelByName(nakedGuntletModel);
                player.playerStatsManager.physicalDamageAbsorptionHands = 0;
                if (player.IsOwner) {
                    player.playerNetworkManager.currentGuntletEquipmentID.Value = -1;
                }
            }
            player.playerStatsManager.poisonResistance = poisonResistance;
        }

        // 방어용 Collider 적용
        public void OpenBlockingCollider() {
            if (inputHandler.twoHandFlag) { // 양잡시 오른손 무기의 경감율 적용
                blockingCollider.SetColliderDamageAbsorption(player.playerInventoryManager.rightWeapon);
            } else { // 아닐경우 왼손 무기의 경감율 적용
                blockingCollider.SetColliderDamageAbsorption(player.playerInventoryManager.leftWeapon);
            }
            blockingCollider.EnableBlockingCollider();
        }

        public void CloseBlockingCollider() {
            blockingCollider.DisableBlockingCollider();
        }
    }
}
