using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace SoulsLike {
    public class PlayerNetworkManager : CharacterNetworkManager {
        PlayerManager player;
        public NetworkVariable<bool> isAtBonfire = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
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

        [ServerRpc]
        public void NotifyServerOfReleaseProjectileServerRpc(ulong clientID,
            int projectileID,
            float xPosition,
            float yPosition,
            float zPosition,
            float yCharacterRotation,
            float playerLookAngle) {
            if (IsServer) {
                NotifyServerOfReleaseProjectileClientRpc(clientID, projectileID, xPosition, yPosition, zPosition, yCharacterRotation, playerLookAngle);
            }
        }

        [ClientRpc]
        public void NotifyServerOfReleaseProjectileClientRpc(ulong clientID,
            int projectileID,
            float xPosition,
            float yPosition,
            float zPosition,
            float yCharacterRotation,
            float playerLookAngle) {
            if (clientID != NetworkManager.Singleton.LocalClientId) {
                PerformReleaseProjectile(clientID, projectileID, xPosition, yPosition, zPosition, yCharacterRotation, playerLookAngle);
            }
        }

        public virtual void PerformReleaseProjectile(ulong clientID,
            int projectileID,
            float xPosition,
            float yPosition,
            float zPosition,
            float yCharacterRotation,
            float playerLookAngle) {

            // Instantiate the projectile
            //Transform projectileInstantiationLocation;
            //projectileInstantiationLocation = player.playerWeaponSlotManager.rightHandSlot.transform;

            //Animator bowAnimator;
            //bowAnimator = player.playerWeaponSlotManager.rightHandSlot.GetComponentInChildren<Animator>();
            //bowAnimator.SetBool("isDrawn", false);
            //bowAnimator.Play("Bow_TH_Fire_01");
            //Destroy(player.characterEffectsManager.instantiatedFXModel);

            GameObject instantiatedSpellFX = Instantiate(WorldItemDatabase.instance.GetSpellItemByID(projectileID).spellCastFX, player.playerWeaponSlotManager.rightHandSlot.transform.position, player.cameraHandler.cameraPivotTransform.rotation);
            Rigidbody rigidbody = instantiatedSpellFX.GetComponent<Rigidbody>();
            SpellDamageCollider spellDamageCollider = instantiatedSpellFX.GetComponent<SpellDamageCollider>();
            spellDamageCollider.characterSpelledThis = player;
            spellDamageCollider.teamIDNumber = player.playerStatsManager.teamIDNumber; // 피아식별을 위한 팀ID 설정

            if (player.currentTarget != null) {
                Quaternion flameRotation = Quaternion.LookRotation(player.currentTarget.lockOnTransform.position - instantiatedSpellFX.gameObject.transform.position);
                instantiatedSpellFX.transform.rotation = flameRotation;
            } else {
                instantiatedSpellFX.transform.rotation = Quaternion.Euler(playerLookAngle, yCharacterRotation, 0);
            }
            ProjectileSpell projectile = WorldItemDatabase.instance.GetSpellItemByID(projectileID) as ProjectileSpell;
            rigidbody.AddForce(instantiatedSpellFX.transform.forward * projectile.projectileForwardVelocity);
            rigidbody.AddForce(instantiatedSpellFX.transform.up * projectile.projectileUpwardVelocity);
            rigidbody.useGravity = projectile.isEffectedByGravity;
            rigidbody.mass = projectile.projectileMass;
            instantiatedSpellFX.transform.parent = null;
        }
    }
}
