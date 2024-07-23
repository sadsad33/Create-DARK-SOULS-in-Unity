using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class PoisonSurface : MonoBehaviour {
        public float poisonBuildUpAmount = 7; // �� ���� ��ġ
        public List<CharacterEffectsManager> charactersInsidePoisonSurface; // ���� Ȥ�� �� �����̻� �߻��� ���� Character���� ���� ����Ʈ

        private void OnTriggerEnter(Collider other) {
            CharacterEffectsManager character = other.GetComponent<CharacterEffectsManager>();
            if (character != null) {
                charactersInsidePoisonSurface.Add(character);
            }
        }

        private void OnTriggerExit(Collider other) {
            CharacterEffectsManager character = other.GetComponent<CharacterEffectsManager>();
            if (character != null) {
                charactersInsidePoisonSurface.Remove(character);
            }
        }

        private void OnTriggerStay(Collider other) {
            foreach (CharacterEffectsManager character in charactersInsidePoisonSurface) { // �� �����̻� �߻��� �� Character �鿡�� ��� ���� ��ġ�� �ο��Ѵ�.
                if (character.isPoisoned) return;
                character.poisonBuildUp += poisonBuildUpAmount * Time.deltaTime;
            }
        }
    }
}
