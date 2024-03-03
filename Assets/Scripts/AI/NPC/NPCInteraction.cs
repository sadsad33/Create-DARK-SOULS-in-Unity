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
            Debug.Log("NPC�� ��ȭ");
            if (!npcManager.hadMetBefore) {
                npcManager.hadMetBefore = true;
                // ù ����� ���̾�α� ���
            } else {
                // ��� ���̾�α� ���
                // ����Ʈ�� �ִٸ� ����Ʈ ����
            }
        }
    }
}