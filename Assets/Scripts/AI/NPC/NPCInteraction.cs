using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class NPCInteraction : Interactable {
        NPCManager npcManager;
        public override void Interact(PlayerManager playerManager) {
            base.Interact(playerManager);
            Conversation(playerManager);
        }

        private void Conversation(PlayerManager playerManager) {
            Debug.Log("NPC와 대화");
            if (!npcManager.hadMetBefore) {
                npcManager.hadMetBefore = true;
                // 첫 조우시 다이얼로그 출력
            } else {
                // 평소 다이얼로그 출력
                // 퀘스트가 있다면 퀘스트 제안
            }
        }
    }
}