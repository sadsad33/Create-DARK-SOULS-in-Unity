using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class BonfireInteraction : Interactable {
        public bool isIgnited = false;

        protected override void Awake() {
            interactableText = "불을 붙인다";
        }

        public override void Interact(PlayerManager player) {
            base.Interact(player);
            Vector3 rotationDirection;
            Vector3 direction = transform.position - player.transform.position;
            direction.y = 0;
            rotationDirection = transform.InverseTransformDirection(direction);
            rotationDirection.Normalize();

            Quaternion tr = Quaternion.LookRotation(rotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(player.transform.rotation, tr, 300 * Time.deltaTime);
            player.transform.rotation = targetRotation;
            if (!isIgnited) {
                isIgnited = true;
                // 화톳불 붙이는 모션
                player.playerInteractionManager.InteractionAtPosition("Bonfire_Ignite", player.transform);
                interactableText = "휴식한다";
            } else {
                // 플레이어 휴식 모션
                player.playerInteractionManager.InteractionAtPosition("Bonfire_Start", player.transform);
                player.isAtBonfire = true;
                UIManager.instance.OpenSelectedWindow(4);
            }
        }
    }
}
