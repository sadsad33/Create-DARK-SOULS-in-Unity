using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
        
    public class PassThroughFogWall : Interactable {
        // 안개의 벽 통과하기
        public override void Interact(PlayerManager player) {
            base.Interact(player);
            //Debug.Log("플레이어와 상호작용");
            player.playerInteractionManager.PassThroughFogWallInteraction(transform);
        }
    }
}