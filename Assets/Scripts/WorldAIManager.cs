using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SoulsLike {
    public class WorldAIManager : MonoBehaviour {
        public static WorldAIManager instance;

        [Header("A.I Spawn")]
        [SerializeField] List<AISpawn> area_00_Spawns;
        [SerializeField] List<AICharacterManager> spawnedInCharacters;

        private void Awake() {
            if (instance == null) {
                instance = this;
            } else {
                Destroy(gameObject);
            }
            spawnedInCharacters = new List<AICharacterManager>();
        }

        private void Start() {
            SpawnInAllCharacters();
        }

        private void SpawnInAllCharacters() {
            foreach (var character in area_00_Spawns) {
                character.AttemptToSpawnCharacter();
            }
        }

        public void AddSpawnedCharacterToActiveCharactersList(AICharacterManager aiCharacter) {
            spawnedInCharacters.Add(aiCharacter);

            for (int i = spawnedInCharacters.Count - 1; i > -1; i--) {
                if (spawnedInCharacters[i] == null) {
                    spawnedInCharacters.RemoveAt(i);
                }
            }
        }

        public AICharacterManager GetAICharacterByID(int ID) {
            return spawnedInCharacters.FirstOrDefault(spawnedInCharacter => spawnedInCharacter.aiCharacterID == ID);
        }
    }
}
