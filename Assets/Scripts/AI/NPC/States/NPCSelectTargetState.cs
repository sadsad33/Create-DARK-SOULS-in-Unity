using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class NPCSelectTargetState : NPCState {
        public NPCIdleState npcIdleState;
        public NPCPursueTargetState npcPursueTargetState;
        bool currentTarget;

        public override NPCState Tick(NPCManager npcManager, NPCStatsManager npcStatsManager, NPCAnimatorManager npcAnimatorManager) {
            
            // ���� �ֺ� ���� ���� ������Ʈ����� �Ÿ��� ����
            // �ּ� �Ÿ��� �ִ� ������Ʈ�� ��ǥ�� ����
            float shortestPath = Mathf.Infinity;
            for (int i = 0; i < npcManager.targets.Count; i++) {
                CharacterStatsManager character = npcManager.targets[i].transform.GetComponent<CharacterStatsManager>();
                if (character != null) {
                    float distance = Vector3.Distance(npcManager.transform.position, character.transform.position);
                    if (distance < shortestPath) {
                        shortestPath = distance;
                        npcManager.currentTarget = character;
                    }
                }
            }

            if (npcManager.currentTarget != null) {
                return npcPursueTargetState;
            } else {
                return npcIdleState;
            }
        }
    }
}