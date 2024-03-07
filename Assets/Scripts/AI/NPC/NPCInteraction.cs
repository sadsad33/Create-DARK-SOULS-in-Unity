using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class NPCInteraction : Interactable {
        //QuestManager questManager
        public NPCManager npcManager;
        [SerializeField]
        int[] startIndex;
        public NPCScript[] dialogs;
        public NPCScript[] currentDialog;

        private void Awake() {
            //Debug.Log("Awake");
            npcManager = GetComponent<NPCManager>();
            int cnt = 0;
            for (int i = 0; i < dialogs.Length; i++) {
                if (dialogs[i].isEnd) cnt++;
            }

            startIndex = new int[cnt];
            int idx = 0;
            for (int i = 0; i < dialogs.Length; i++) {
                if (dialogs[i].isStart) {
                    startIndex[idx++] = i;
                }
            }
        }

        private void OnEnable() {
            //Debug.Log("OnEnable");
            GetStartIndex();
        }

        public override void Interact(PlayerManager playerManager) {
            base.Interact(playerManager);
            StartConverstation(playerManager);
        }


        private void GetStartIndex() {
            int idx = 0;
            for (int i = 0; i < dialogs.Length; i++) {
                if (dialogs[i].isStart) {
                    startIndex[idx++] = i;
                }
            }
        }

        private void StartConverstation(PlayerManager playerManager) {
            Debug.Log("NPC와 대화 시작");
            playerManager.isInConversation = true;
            
            // 첫 조우시 출력할 다이얼로그 전달
            if (npcManager.interactCount == 0) {
                CopyScript(startIndex[0]);
            }
            // 대화를 모두 마쳤고, 퀘스트가 있다면 퀘스트 다이얼로그 전달
            else if (npcManager.interactCount >= startIndex.Length - 1) {
                CopyScript(startIndex[^1]);
            }
            // 평소 다이얼로그 전달
            else {
                CopyScript(startIndex[npcManager.interactCount]);
            }
            npcManager.interactCount++;
            playerManager.currentDialog = currentDialog;
        }

        private void CopyScript(int start) {
            int i;
            for (i = start; i < dialogs.Length; i++) {
                if (dialogs[i].isEnd) break;
            }
            currentDialog = new NPCScript[i-start + 1];
            int idx = 0;
            for (i = start; i < start + currentDialog.Length; i++) {
                currentDialog[idx++] = dialogs[i];
            }
        }
    }
}