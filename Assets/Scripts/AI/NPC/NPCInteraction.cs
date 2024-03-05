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
            Debug.Log("NPC�� ��ȭ ����");
            playerManager.isInConversation = true;

            if (npcManager.interactCount == 0) {
                // ù ����� ����� ���̾�α� ����
                playerManager.currentDialog = firstEncounterScripts;
            } else {
                if (readyForSuggestion) {
                    // ����Ʈ�� �ִٸ� ����Ʈ ���̾�α� ����
                    //playerManager.currentDialog = suggestionDialog;
                }
                // ��� ���̾�α� ����
                playerManager.currentDialog = dialog;
            }
        }
    }
}