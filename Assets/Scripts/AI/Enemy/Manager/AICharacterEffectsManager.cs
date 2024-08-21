using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class AICharacterEffectsManager : CharacterEffectsManager {
        AICharacterManager aiCharacterManager;

        protected override void Awake() {
            base.Awake();
            aiCharacterManager = GetComponent<AICharacterManager>();
        }

        public override void ProcessEffectInstantly(CharacterEffect effect) {
            base.ProcessEffectInstantly(effect);
            if (effect is TakeDamageEffect) {
                aiCharacterManager.aiStatsManager.enemyHealthBar.UpdateHealth(aiCharacterManager.aiStatsManager.currentHealth);
                //Debug.Log(aiCharacterManager.aiStatsManager.enemyHealthBar);
            }
        }
    }
}
