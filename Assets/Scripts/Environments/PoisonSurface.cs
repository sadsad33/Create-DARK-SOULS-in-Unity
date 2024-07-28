using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class PoisonSurface : MonoBehaviour {
        public List<CharacterManager> charactersInsidePoisonSurface; // 독늪 혹은 독 상태이상 발생지 내의 Character들을 담을 리스트

        private void OnTriggerEnter(Collider other) {
            CharacterManager character = other.GetComponent<CharacterManager>();
            if (character != null) {
                charactersInsidePoisonSurface.Add(character);
            }
        }

        private void OnTriggerExit(Collider other) {
            CharacterManager character = other.GetComponent<CharacterManager>();
            if (character != null) {
                charactersInsidePoisonSurface.Remove(character);
            }
        }

        private void OnTriggerStay(Collider other) {
            foreach (CharacterManager character in charactersInsidePoisonSurface) { // 독 상태이상 발생지 내 Character 들에게 모두 축적 수치를 부여한다.
                if (character.characterStatsManager.isPoisoned) return;
                
                PoisonBuildUpEffect poisonBuildUp = Instantiate(WorldEffectsManager.instance.poisonBuildUpEffect);

                foreach (var effect in character.characterEffectsManager.timedEffects) {
                    if (effect.effectID == poisonBuildUp.effectID) return;
                }
                character.characterEffectsManager.timedEffects.Add(poisonBuildUp);
            }
        }
    }
}
