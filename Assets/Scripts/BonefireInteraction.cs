using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class BonefireInteraction : Interactable {
        public bool isIgnited = false;

        private void Awake() {
            interactableText = "Lit";
        }

        public override void Interact(PlayerManager playerManager) {
            base.Interact(playerManager);
            Vector3 rotationDirection;
            Vector3 direction = transform.position - playerManager.transform.position;
            direction.y = 0;
            rotationDirection = transform.InverseTransformDirection(direction);
            rotationDirection.Normalize();

            Quaternion tr = Quaternion.LookRotation(rotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, tr, 300 * Time.deltaTime);
            playerManager.transform.rotation = targetRotation;
            if (!isIgnited) {
                isIgnited = true;
                // 화톳불 붙이는 모션
                playerManager.InteractionAtPosition("Bonfire_Ignite", playerManager.transform);
                interactableText = "Rest at Bonefire";
                
            } else {
                // 플레이어 휴식 모션
                playerManager.InteractionAtPosition("Bonfire_Start", playerManager.transform);
                playerManager.isAtBonefire = true;
                // 화톳불 UI를 화면에 출력
            }
        }
    }
}
