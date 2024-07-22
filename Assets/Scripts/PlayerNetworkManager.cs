using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace SoulsLike {
    public class PlayerNetworkManager : CharacterNetworkManager {
        PlayerManager player;

        protected override void Awake() {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }
        // 갑옷 변경
        public void OnHeadEquipmentChange(int oldEquipmentID, int newEquipmentID) {
            if (player.IsOwner) return;
            EquipmentItem equipment = WorldItemDatabase.instance.GetEquipmentItemByID(newEquipmentID);

            if (equipment != null) {
                player.characterInventoryManager.currentHelmetEquipment = equipment as HelmetEquipment;
            } else {
                player.characterInventoryManager.currentHelmetEquipment = null;
            }
            player.playerEquipmentManager.EquipAllEquipmentModels();
        }
        public void OnTorsoEquipmentChange(int oldEquipmentID, int newEquipmentID) {
            if (player.IsOwner) return;
            EquipmentItem equipment = WorldItemDatabase.instance.GetEquipmentItemByID(newEquipmentID);

            if (equipment != null) {
                player.characterInventoryManager.currentTorsoEquipment = equipment as TorsoEquipment;
            } else {
                player.characterInventoryManager.currentTorsoEquipment = null;
            }
            player.playerEquipmentManager.EquipAllEquipmentModels();
        }
        public void OnGuntletEquipmentChange(int oldEquipmentID, int newEquipmentID) {
            if (player.IsOwner) return;
            EquipmentItem equipment = WorldItemDatabase.instance.GetEquipmentItemByID(newEquipmentID);

            if (equipment != null) {
                player.characterInventoryManager.currentGuntletEquipment = equipment as GuntletEquipment;
            } else {
                player.characterInventoryManager.currentGuntletEquipment = null;
            }
            player.playerEquipmentManager.EquipAllEquipmentModels();
        }
        public void OnLegEquipmentChange(int oldEquipmentID, int newEquipmentID) {
            if (player.IsOwner) return;
            EquipmentItem equipment = WorldItemDatabase.instance.GetEquipmentItemByID(newEquipmentID);

            if (equipment != null) {
                player.characterInventoryManager.currentLegEquipment = equipment as LegEquipment;
            } else {
                player.characterInventoryManager.currentLegEquipment = null;
            }
            player.playerEquipmentManager.EquipAllEquipmentModels();
        }
    }
}
