using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class BonfireInteraction : Interactable {
        public bool isIgnited = false;
        //public UIManager playerUIManager;
        protected override void Awake() {
            interactableText = "���� ���δ�";
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
                // ȭ��� ���̴� ���
                playerManager.InteractionAtPosition("Bonfire_Ignite", playerManager.transform);
                interactableText = "�޽��Ѵ�";
            } else {
                // �÷��̾� �޽� ���
                playerManager.InteractionAtPosition("Bonfire_Start", playerManager.transform);
                playerManager.isAtBonfire = true;
                UIManager.instance.OpenSelectedWindow(4);
            }
        }
    }
}
