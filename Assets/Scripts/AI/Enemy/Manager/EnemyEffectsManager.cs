using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class EnemyEffectsManager : CharacterEffectsManager {
        AICharacterManager aiCharacterManager;

        protected override void Awake() {
            base.Awake();
            aiCharacterManager = GetComponent<AICharacterManager>();
        }

        public override void ProcessEffectInstantly(CharacterEffect effect) {
            base.ProcessEffectInstantly(effect);
            if (effect is TakeDamageEffect) {
                aiCharacterManager.enemyStatsManager.enemyHealthBar.UpdateHealth(aiCharacterManager.enemyStatsManager.currentHealth);
            }
        }
    }
}
