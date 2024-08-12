using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class NPCAnimatorManager : AICharacterAnimatorManager {
        public NPCInventoryManager npcInventory;
        public NPCWeaponSlotManager npcWeaponSlotManager;
        //NPCManager npcManager;
        protected override void Awake() {
            base.Awake();
            //npcManager = GetComponent<NPCManager>();
            //npcManager.animator = GetComponent<Animator>();
        }

        //public override void OnAnimatorMove() {
        //    float delta = Time.deltaTime;
        //    npcManager.npcRigidbody.drag = 0;
        //    Vector3 deltaPosition = npcManager.animator.deltaPosition;
        //    deltaPosition.y = 0;
        //    Vector3 velocity = deltaPosition / delta;
        //    npcManager.npcRigidbody.velocity = velocity;

        //    if (character.isRotatingWithRootMotion) {
        //        character.transform.rotation *= npcManager.animator.deltaRotation;
        //    }
        //}

        //protected override void AwardSoulsOnDeath() {
        //    PlayerStatsManager playerStats = FindObjectOfType<PlayerStatsManager>();
        //    SoulCountBar soulCountBar = FindObjectOfType<SoulCountBar>();

        //    if (playerStats != null) {
        //        playerStats.AddSouls(characterStatsManager.soulsAwardedOnDeath);
        //        if (soulCountBar != null) {
        //            soulCountBar.SetSoulCountText(playerStats.soulCount);
        //        }
        //    }
        //}

        public void EquipWeapon() {
            npcInventory.currentRightWeaponIndex += 1;
            npcInventory.rightWeapon = npcInventory.weaponsInRightHandSlots[npcInventory.currentRightWeaponIndex];
            npcWeaponSlotManager.LoadBothWeaponsOnSlots();
        }
    }
}