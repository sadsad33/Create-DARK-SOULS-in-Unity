using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class LadderEndPositionDetection : MonoBehaviour {
        public bool isTopEnd, isBottomEnd;
        LadderInteraction ladderInteraction;
        public Transform ladderTopFinishingPosition, ladderBottomFinishingPosition;

        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("LadderTopEnd")) {
                isTopEnd = true;
                ladderInteraction = other.transform.parent.GetComponentInChildren<LadderInteraction>();
                ladderTopFinishingPosition = ladderInteraction.topFinishingPosition;
            } else if (other.CompareTag("LadderBottomEnd")) {
                isBottomEnd = true;
                ladderInteraction = other.transform.parent.GetComponentInChildren<LadderInteraction>();
                ladderBottomFinishingPosition = ladderInteraction.bottomFinishingPosition;
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.CompareTag("LadderTopEnd")) {
                isTopEnd = false;
            } else if (other.CompareTag("LadderBottomEnd")) {
                isBottomEnd = false;
            }
        }
    }
}
