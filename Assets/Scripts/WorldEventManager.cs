using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    // 맵에 존재하는 이벤트 관리
    public class WorldEventManager : MonoBehaviour {

        public BossHealthBar bossHealthBar;
        public BossManager boss;

        public bool bossFightIsActive; // 보스전 시작 여부
        public bool bossHasBeenAwakened; // 보스 행동 시작 혹은 컷신 재생
        public bool bossHasBeenDefeated; // 보스 격파

        private void Awake() {
            bossHealthBar = FindObjectOfType<BossHealthBar>();
        }

        public void ActivateBossFight() {
            bossFightIsActive = true;
            bossHasBeenAwakened = true;
            bossHealthBar.SetUIHealthBarToActive();
            // 안개벽 생성
        }

        public void BossHasBeenDefeated() {
            bossHasBeenDefeated = true;
            bossFightIsActive = false;
            // 안개벽 소멸
        }
    }
}
