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

        protected override void HandlePoisonBuildUp() {
            if (poisonBuildUp <= 0) {
                UIManager.instance.poisonBuildUpBar.gameObject.SetActive(false); // 축적치가 하나도 없다면 화면에 보이지 않도록 한다.
            } else {
                UIManager.instance.poisonBuildUpBar.gameObject.SetActive(true); // 축적치가 조금이라도 있다면 화면에 표시
            }
            base.HandlePoisonBuildUp();
            UIManager.instance.poisonBuildUpBar.SetPoisonBuildUpAmount(poisonBuildUp); // 독 축적치를 나타낼 UI바에 현재 축적치를 반영한다.
        }

        protected override void HandlePoisonedEffect() {
            if (!isPoisoned) {
                UIManager.instance.poisonAmountBar.gameObject.SetActive(false); // 독 상태이상에 걸리지 않았다면 축적치 표시 X
            } else {
                UIManager.instance.poisonAmountBar.gameObject.SetActive(true); // 상태이상에 걸린 상태라면 축적치 표시
            }
            base.HandlePoisonedEffect();
            UIManager.instance.poisonAmountBar.SetPoisonAmount(poisonAmount); // 독 축적치를 나타낼 UI바에 현재 축적치를 반영
        }
    }
}
