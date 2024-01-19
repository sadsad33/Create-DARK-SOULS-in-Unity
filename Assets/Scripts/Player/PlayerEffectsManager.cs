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

        private void Awake() {
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerWeaponSlotManager = GetComponent<PlayerWeaponSlotManager>();
        }
        public void HealPlayerFromEffect() {
            playerStatsManager.HealPlayer(amountToBeHealed);
            GameObject healParticles = Instantiate(currentParticleFX, playerStatsManager.transform); // 회복 이펙트
            Destroy(healParticles, 2f);
        }

        public void UnLoadFlask() {
            Destroy(instantiatedFXModel.gameObject); // 에스트병 제거
        }
    }
}
