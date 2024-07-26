using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class PlayerEffectsManager : CharacterEffectsManager {
        PlayerManager player;
        public GameObject currentParticleFX; // 현재 플레이어의 상태에 따른 오라(?) 효과
        public GameObject instantiatedFXModel;
        public float amountToBeHealed;

        // 데미지와 같은 즉각적인 효과
        // 반지나, 갑옷 효과 같은 정적 효과
        // 시간 혹은 상태에 따른 효과

        protected override void Awake() {
            base.Awake();
            player = GetComponent<PlayerManager>();
            //poisonBuildUpBar = FindObjectOfType<PoisonBuildUpBar>();
            //poisonAmountBar = FindObjectOfType<PoisonAmountBar>();
        }
        protected override void Start() {
            base.Start();
        }

        public void HealPlayerFromEffect() {
            player.playerStatsManager.HealPlayer(amountToBeHealed);
            GameObject healParticles = Instantiate(currentParticleFX, player.playerStatsManager.transform); // 회복 이펙트
            Destroy(healParticles, 2f);
        }

        public void UnloadFlask() {
            Destroy(instantiatedFXModel.gameObject); // 에스트병 제거
        }
    }
}
