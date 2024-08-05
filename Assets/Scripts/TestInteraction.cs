using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class TestInteraction : Interactable {

        public override void Interact(PlayerManager player) {

            player.transform.forward = -transform.forward;
            player.playerAnimatorManager.PlayTargetAnimation("Ladder_StartTop", false);
        }
    }
}