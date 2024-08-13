using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class NPCIdleState : State {
        public NPCSelectTargetState npcSelectTargetState;
        Collider[] colliders;
        int teamCode;

        public override State Tick(AICharacterManager aiCharacter) {

            NPCManager npc = aiCharacter as NPCManager;
            if (npc.targets.Count > 0) npc.targets.Clear();
            colliders = Physics.OverlapSphere(npc.transform.position, npc.detectionRadius, npc.hostileLayer);
            npc.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            npc.animator.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);

            for (int i = 0; i < colliders.Length; i++) {
                CharacterManager character = colliders[i].transform.GetComponent<CharacterManager>();
                if (character != null && !character.characterStatsManager.isDead) {
                    teamCode = character.characterStatsManager.teamIDNumber;

                    if (teamCode == 1 && npc.aggravationToEnemy >= 30) {
                        npc.currentHostile |= LayerMask.GetMask("Character");
                        npc.targets.Add(character);
                    }
                    if (teamCode == 2 && npc.aggravationToPlayer >= 30) {
                        npc.currentHostile |= LayerMask.GetMask("Player");
                        npc.targets.Add(character);
                    }
                }
            }

            if (npc.targets.Count > 0) {
                if (!npc.hasDrawnWeapon) {
                    npc.hasDrawnWeapon = true;
                    npc.aiAnimatorManager.PlayTargetAnimation("Equip", true);
                }
                return npcSelectTargetState;
            }
            return this;
        }
    }
}