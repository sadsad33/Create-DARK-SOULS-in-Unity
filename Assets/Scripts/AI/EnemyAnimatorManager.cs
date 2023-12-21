using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class EnemyAnimatorManager : AnimatorManager {
        EnemyManager enemyManager;
        EnemyStats enemyStats;
        private void Awake() {
            anim = GetComponent<Animator>();
            enemyManager = GetComponentInParent<EnemyManager>();
            enemyStats = GetComponentInParent<EnemyStats>();
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

            if (enemyManager.isRotatingWithRootMotion) {
                enemyManager.transform.rotation *= anim.deltaRotation;
            }
        }

        public override void TakeCriticalDamageAnimationEvent() {
            enemyStats.TakeDamageNoAnimation(enemyManager.pendingCriticalDamage);
            enemyManager.pendingCriticalDamage = 0;
        }

        public void CanRotate() {
            anim.SetBool("canRotate", true);
        }
        public void StopRotation() {
            anim.SetBool("canRotate", false);
        }
        public void EnableCombo() {
            anim.SetBool("canDoCombo", true);
        }
        public void DisableCombo() {
            anim.SetBool("canDoCombo", false);
        }

        public void EnableIsInvulnerable() {
            anim.SetBool("isInvulnerable", true);
        }

        public void DisableIsInvulnerable() {
            anim.SetBool("isInvulnerable", false);
        }

        public void EnableIsParrying() {
            enemyManager.isParrying = true;
        }

        public void DisableIsParrying() {
            enemyManager.isParrying = false;
        }

        public void EnableCanBeRiposted() {
            enemyManager.canBeRiposted = true;
        }

        public void DisableCanBeRiposted() {
            enemyManager.canBeRiposted = false;
        }

        public void AwardSoulsOnDeath() {
            // 씬 내의 모든 플레이어에게 소울을 줌
            PlayerStats playerStats = FindObjectOfType<PlayerStats>();
            SoulCountBar soulCountBar = FindObjectOfType<SoulCountBar>();

            if (playerStats != null) {
                playerStats.AddSouls(enemyStats.soulsAwardedOnDeath);
                if (soulCountBar != null) {
                    soulCountBar.SetSoulCountText(playerStats.soulCount);
                }
            }
        }
    }
}
