using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class LeverInteraction : Interactable {
        public Transform playerStandingPosition;
        public Animator animator;
        // ���� ��ȣ�ۿ��� ���� ������ ������Ʈ
        // public Transform connectedObject;

        private void Awake() {
            animator = GetComponent<Animator>();
        }

        public override void Interact(PlayerManager playerManager) {
            base.Interact(playerManager);
            Vector3 rotationDirection = -transform.forward;
            rotationDirection.y = 0;
            rotationDirection.Normalize();

            Quaternion tr = Quaternion.LookRotation(rotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, tr, 300 * Time.deltaTime);
            playerManager.transform.rotation = targetRotation;

            playerManager.InteractionAtPosition("Pull Lever", playerStandingPosition);
            animator.Play("Lever Pulled");
            Destroy(this);
            ActivateObject();
        }

        // ������ �۵��������� �Ͼ ��
        private void ActivateObject() {
        }
    }
}