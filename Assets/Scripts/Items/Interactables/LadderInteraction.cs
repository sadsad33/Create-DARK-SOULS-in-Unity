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

        public override void Interact(PlayerManager player) {
            base.Interact(player);

            Vector3 rotationDirection = -ladderTransform.right;
            rotationDirection.y = 0;
            rotationDirection.Normalize();

            Quaternion tr = Quaternion.LookRotation(rotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(player.transform.rotation, tr, 3000 * Time.deltaTime);
            player.transform.rotation = targetRotation;

            player.characterNetworkManager.isClimbing.Value = true;
            player.ladderEndPositionDetector.transform.gameObject.SetActive(true);
            if (isTop) { // 사다리의 꼭대기에서 상호작용을 시작하는 경우
                player.playerInteractionManager.InteractionAtPosition("Ladder_StartTop", topStartingPosition);
                player.isLadderTop = true;
            } else { // 사다리의 맨 밑에서 상호작용을 시작하는 경우
                player.playerInteractionManager.InteractionAtPosition("Ladder_StartBottom", bottomStartingPosition);
                player.isLadderTop = false;
            }
        }
        
    }
}