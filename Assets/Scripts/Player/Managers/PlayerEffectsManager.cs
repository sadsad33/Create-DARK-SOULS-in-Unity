using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PlayerEffectsManager : CharacterEffectsManager {
        PlayerStatsManager playerStatsManager;
        PlayerWeaponSlotManager playerWeaponSlotManager;
        public GameObject currentParticleFX; // 현재 플레이어의 상태에 따른 오라(?) 효과
        public GameObject instantiatedFXModel;
        public float amountToBeHealed;

        PoisonBuildUpBar poisonBuildUpBar;
        PoisonAmountBar poisonAmountBar;

        protected override void Awake() {
            base.Awake();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerWeaponSlotManager = GetComponent<PlayerWeaponSlotManager>();

            poisonBuildUpBar = FindObjectOfType<PoisonBuildUpBar>();
            poisonAmountBar = FindObjectOfType<PoisonAmountBar>();
        }
        public void HealPlayerFromEffect() {
            playerStatsManager.HealPlayer(amountToBeHealed);
            GameObject healParticles = Instantiate(currentParticleFX, playerStatsManager.transform); // 회복 이펙트
            Destroy(healParticles, 2f);
        }

        public void UnloadFlask() {
            Destroy(instantiatedFXModel.gameObject); // 에스트병 제거
        }

        protected override void HandlePoisonBuildUp() {
            if (poisonBuildUp <= 0) {
                poisonBuildUpBar.gameObject.SetActive(false); // 축적치가 하나도 없다면 화면에 보이지 않도록 한다.
            } else {
                poisonBuildUpBar.gameObject.SetActive(true); // 축적치가 조금이라도 있다면 화면에 표시
            }
            base.HandlePoisonBuildUp();
            poisonBuildUpBar.SetPoisonBuildUpAmount(poisonBuildUp); // 독 축적치를 나타낼 UI바에 현재 축적치를 반영한다.
        }

        protected override void HandlePoisonedEffect() {
            if (!isPoisoned) {
                poisonAmountBar.gameObject.SetActive(false); // 독 상태이상에 걸리지 않았다면 축적치 표시 X
            } else {
                poisonAmountBar.gameObject.SetActive(true); // 상태이상에 걸린 상태라면 축적치 표시
            }
            base.HandlePoisonedEffect();
            poisonAmountBar.SetPoisonAmount(poisonAmount); // 독 축적치를 나타낼 UI바에 현재 축적치를 반영
        }

    }
}
