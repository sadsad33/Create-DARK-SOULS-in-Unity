using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class EnemyAnimatorManager : CharacterAnimatorManager {
        BossManager bossManager;
        //EnemyEffectsManager enemyEffectsManager;
        AICharacterManager enemyManager;
        protected override void Awake() {
            base.Awake();
            enemyManager = GetComponent<AICharacterManager>();
            enemyManager.animator = GetComponent<Animator>();
            //enemyEffectsManager = GetComponent<EnemyEffectsManager>();
            bossManager = GetComponent<BossManager>();
        }

        public void InstantiateBossParticleFX() {
            BossFXTransform bossFXTransform = GetComponentInChildren<BossFXTransform>();
            GameObject phaseFX = Instantiate(bossManager.particleFX, bossFXTransform.transform);
        }

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

        public override void OnAnimatorMove() {
            Vector3 velocity = character.animator.deltaPosition;
            character.characterController.Move(velocity);

            if (enemyManager.isRotatingWithRootMotion) {
                character.transform.rotation *= character.animator.deltaRotation;
            }
        }
    }
}
