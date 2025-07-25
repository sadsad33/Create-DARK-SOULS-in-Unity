using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    [CreateAssetMenu(menuName = "Items/Consumables/NetworkItem")]
    public class NetworkItem : ConsumableItem {

        public override void AttemptToConsumeItem(PlayerManager player) {
            Debug.Log("네트워크 아이템 사용");
            player.playerAnimatorManager.PlayTargetAnimation(consumeAnimation, isInteracting, true);
            GameSessionManager.instance.shutdownNetwork = true;
        }

        //public override void SuccessfullyConsumedItem(PlayerManager player) {
        //    GameSessionManager.instance.shutdownNetwork = true;
        //    GameSessionManager.instance.startGameAsClient = true;
        //}

        public override bool CanIUseThisItem(PlayerManager player) {
            return true;
        }
    }
}
