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
            colliders = Physics.OverlapSphere(npcManager.transform.position, npcManager.detectionRadius, npcManager.hostileLayer);
            npcAnimatorManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            npcAnimatorManager.anim.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);

            for (int i = 0; i < colliders.Length; i++) {
                CharacterStatsManager character = colliders[i].transform.GetComponent<CharacterStatsManager>();
                if (character != null && !character.isDead) {
                    teamCode = character.teamIDNumber;

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