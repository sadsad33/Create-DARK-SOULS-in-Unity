using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class NPCInteraction : Interactable {
        //QuestManager questManager
        NPCManager npcManager;
        public NPCScript[] firstEncounterScripts;
        public NPCScript[] dialog;
        // public NPCScript[] suggestionDialog;
        private bool readyForSuggestion = false;

        public override void Interact(PlayerManager playerManager) {
            base.Interact(playerManager);
            StartConverstation(playerManager);
        }

        private void StartConverstation(PlayerManager playerManager) {
            Debug.Log("NPC와 대화 시작");
            playerManager.isInConversation = true;

            if (npcManager.interactCount == 0) {
                // 첫 조우시 출력할 다이얼로그 전달
                playerManager.currentDialog = firstEncounterScripts;
            } else {
                if (readyForSuggestion) {
                    // 퀘스트가 있다면 퀘스트 다이얼로그 전달
                    //playerManager.currentDialog = suggestionDialog;
                }
                // 평소 다이얼로그 전달
                playerManager.currentDialog = dialog;
            }
        }
    }
}