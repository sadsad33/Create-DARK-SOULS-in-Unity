using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class NPCAnimatorManager : CharacterAnimatorManager {
        NPCManager npcManager;
        protected override void Awake() {
            base.Awake();
            npcManager = GetComponent<NPCManager>();
            anim = GetComponent<Animator>();
        }

        private void OnAnimatorMove() {
            float delta = Time.deltaTime;
            npcManager.npcRigidbody.drag = 0;
            Vector3 deltaPosition = anim.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            npcManager.npcRigidbody.velocity = velocity;

            if (characterManager.isRotatingWithRootMotion) {
                characterManager.transform.rotation *= anim.deltaRotation;
            }
        }

        public void AwardSoulsOnDeath() {
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