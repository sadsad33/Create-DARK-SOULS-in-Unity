using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class PlayerEffectsManager : CharacterEffectsManager {
        PlayerManager player;
        PoisonBuildUpBar poisonBuildUpBar;
        public PoisonAmountBar poisonAmountBar;
        public GameObject currentParticleFX; // 현재 플레이어의 상태에 따른 이펙트
        
        public float amountToBeHealed;

        // 데미지와 같은 즉각적인 효과
        // 반지나, 갑옷 효과 같은 정적 효과
        // 시간 혹은 상태에 따른 효과

        protected override void Awake() {
            base.Awake();
            player = GetComponent<PlayerManager>();
            poisonBuildUpBar = UIManager.instance.poisonBuildUpBar;
            poisonAmountBar = UIManager.instance.poisonAmountBar;
        }

        //protected override void Start() {
        //    base.Start();
        //}

        public override void ProcessEffectInstantly(CharacterEffect effect) {
            base.ProcessEffectInstantly(effect);
            if (effect is TakeDamageEffect && player.IsOwner) {
                UIManager.instance.healthBar.SetCurrentHealth(player.playerStatsManager.currentHealth);
            }
        }

        public void HealPlayerFromEffect() {
            player.playerStatsManager.HealPlayer(amountToBeHealed);
            GameObject healParticles = Instantiate(currentParticleFX, player.playerStatsManager.transform); // 회복 이펙트
            Destroy(healParticles, 2f);
        }

        public void UnloadFlask() {
            Destroy(instantiatedFXModel.gameObject); // 에스트병 제거
        }

        protected override void ProcessBuildUpDecay() {
            if (player.characterStatsManager.poisonBuildUp >= 0) {
                player.characterStatsManager.poisonBuildUp -= 1;

                poisonBuildUpBar.gameObject.SetActive(true);
                poisonBuildUpBar.SetPoisonBuildUpAmount(player.characterStatsManager.poisonBuildUp);
            }
        }
    }
}
