using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class EnemyAnimatorManager : AnimatorManager {
        BossManager bossManager;
        //EnemyEffectsManager enemyEffectsManager;
        EnemyManager enemyManager;
        protected override void Awake() {
            base.Awake();
            enemyManager = GetComponent<EnemyManager>();
            anim = GetComponent<Animator>();
            //enemyEffectsManager = GetComponent<EnemyEffectsManager>();
            bossManager = GetComponent<BossManager>();
        }


        // RootMotion이 적용된 애니메이션이 재생중에 RootPosition이 이동되면 호출
        // OnAnimatorMove 함수를 구현하게 되면 Animator 컴포넌트의 ApplyRootMotion이 HandledByScript가 되어 OnAnimatorMove 함수내에서 이동 관련 로직을 구현해 주어야 함
        private void OnAnimatorMove() {
            float delta = Time.deltaTime;
            enemyManager.enemyRigidbody.drag = 0; // 미끄러짐 방지
            Vector3 deltaPosition = anim.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            enemyManager.enemyRigidbody.velocity = velocity;

            if (characterManager.isRotatingWithRootMotion) {
                characterManager.transform.rotation *= anim.deltaRotation;
            }
        }

        public void InstantiateBossParticleFX() {
            BossFXTransform bossFXTransform = GetComponentInChildren<BossFXTransform>();
            GameObject phaseFX = Instantiate(bossManager.particleFX, bossFXTransform.transform);
        }

        //public void PlayWeaponTrailFX() {
        //    enemyEffectsManager.PlayWeaponFX(false);
        //}

        public void AwardSoulsOnDeath() {
            // 씬 내의 모든 플레이어에게 소울을 줌
            PlayerStatsManager playerStats = FindObjectOfType<PlayerStatsManager>();
            SoulCountBar soulCountBar = FindObjectOfType<SoulCountBar>();

            if (playerStats != null) {
                playerStats.AddSouls(characterStatsManager.soulsAwardedOnDeath);
                if (soulCountBar != null) {
                    soulCountBar.SetSoulCountText(playerStats.soulCount);
                }
            }
        }
    }
}
