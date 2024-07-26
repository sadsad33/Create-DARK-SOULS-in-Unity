using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class PoisonSurface : MonoBehaviour {
        public float poisonBuildUpAmount = 7; // 독 축적 수치
        public List<CharacterStatsManager> charactersInsidePoisonSurface; // 독늪 혹은 독 상태이상 발생지 내의 Character들을 담을 리스트

        private void OnTriggerEnter(Collider other) {
            CharacterStatsManager character = other.GetComponent<CharacterStatsManager>();
            if (character != null) {
                charactersInsidePoisonSurface.Add(character);
            }
        }

        private void OnTriggerExit(Collider other) {
            CharacterStatsManager character = other.GetComponent<CharacterStatsManager>();
            if (character != null) {
                charactersInsidePoisonSurface.Remove(character);
            }
        }

        private void OnTriggerStay(Collider other) {
            foreach (CharacterStatsManager character in charactersInsidePoisonSurface) { // 독 상태이상 발생지 내 Character 들에게 모두 축적 수치를 부여한다.
                if (character.isPoisoned) return;
                character.poisonBuildUp += poisonBuildUpAmount * Time.deltaTime;
            }
        }
    }
}
