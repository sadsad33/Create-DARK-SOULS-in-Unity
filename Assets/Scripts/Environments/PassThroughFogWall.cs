using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
        
    public class PassThroughFogWall : Interactable {
        WorldEventManager worldEventManager;

        protected override void Awake() {
            worldEventManager = FindObjectOfType<WorldEventManager>();
        }

        // 안개의 벽 통과하기
        public override void Interact(PlayerManager player) {
            base.Interact(player);
            player.playerInteractionManager.PassThroughFogWallInteraction(transform);
            worldEventManager.ActivateBossFight();
        }
    }
}