using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class NPCIdleState : NPCState {
        //public NPCPursueTargetState npcPursueTargetState;
        public NPCSelectTargetState npcSelectTargetState;
        Collider[] colliders;
        int teamCode;

        public override NPCState Tick(NPCManager npcManager, NPCStatsManager npcStatsManager, NPCAnimatorManager npcAnimatorManager) {
            if (npcManager.targets.Count > 0) npcManager.targets.Clear();
            colliders = Physics.OverlapSphere(npcManager.transform.position, npcManager.detectionRadius, npcManager.hostileLayer);
            npcManager.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            npcManager.animator.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);

            for (int i = 0; i < colliders.Length; i++) {
                CharacterManager character = colliders[i].transform.GetComponent<CharacterManager>();
                if (character != null && !character.characterStatsManager.isDead) {
                    teamCode = character.characterStatsManager.teamIDNumber;

                    if (teamCode == 1 && npcManager.aggravationToEnemy >= 30) {
                        npcManager.currentHostile |= LayerMask.GetMask("Character");
                        npcManager.targets.Add(character);
                    }
                    if (teamCode == 2 && npcManager.aggravationToPlayer >= 30) {
                        npcManager.currentHostile |= LayerMask.GetMask("Player");
                        npcManager.targets.Add(character);
                    }
                }
            }

            if (npcManager.targets.Count > 0) {
                if (!npcManager.drawnWeapon) {
                    npcManager.drawnWeapon = true;
                    npcAnimatorManager.PlayTargetAnimation("Equip", true);
                }
                return npcSelectTargetState;
            }
            return this;
        }
    }
}