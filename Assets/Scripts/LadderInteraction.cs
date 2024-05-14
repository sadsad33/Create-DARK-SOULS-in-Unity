using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class LadderInteraction : Interactable {
        public bool isTop;
        public Transform ladderTransform;

        [Header("Ladder Top Positions")]
        public Transform topStartingPosition;
        public Transform topFinishingPosition;

        [Header("Ladder Bottom Positions")]
        public Transform bottomStartingPosition;
        public Transform bottomFinishingPosition;

        public override void Interact(PlayerManager playerManager) {
            base.Interact(playerManager);

            Vector3 rotationDirection;
            //rotationDirection = transform.InverseTransformDirection(-transform.forward);
            rotationDirection = -ladderTransform.right;
            rotationDirection.Normalize();

            Quaternion tr = Quaternion.LookRotation(rotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, tr, 300 * Time.deltaTime);
            playerManager.transform.rotation = targetRotation;
            playerManager.isClimbing = true;

            playerManager.ladderEndPositionDetector.transform.gameObject.SetActive(true);
            if (isTop) { // ��ٸ��� ����⿡�� ��ȣ�ۿ��� �����ϴ� ���
                playerManager.InteractionAtPosition("Ladder_StartTop", topStartingPosition);
                playerManager.isLadderTop = true;
            } else { // ��ٸ��� �� �ؿ��� ��ȣ�ۿ��� �����ϴ� ���
                playerManager.InteractionAtPosition("Ladder_StartBottom", bottomStartingPosition);
                playerManager.isLadderTop = false;
            }
        }
    }
}