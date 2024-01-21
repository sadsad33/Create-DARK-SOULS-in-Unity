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


        // RootMotion�� ����� �ִϸ��̼��� ����߿� RootPosition�� �̵��Ǹ� ȣ��
        // OnAnimatorMove �Լ��� �����ϰ� �Ǹ� Animator ������Ʈ�� ApplyRootMotion�� HandledByScript�� �Ǿ� OnAnimatorMove �Լ������� �̵� ���� ������ ������ �־�� ��
        private void OnAnimatorMove() {
            float delta = Time.deltaTime;
            enemyManager.enemyRigidbody.drag = 0; // �̲����� ����
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
            // �� ���� ��� �÷��̾�� �ҿ��� ��
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
