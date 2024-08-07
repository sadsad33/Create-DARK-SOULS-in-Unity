using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace SoulsLike {
    [CreateAssetMenu(menuName = "AI/Spawner")]
    public class AISpawn : ScriptableObject {
        [Header("Character Info")]
        [SerializeField] GameObject character;
        [SerializeField] Vector3 spawnPosition;
        [SerializeField] Vector3 spawnRotation;

        [Header("Spawn Model")]
        private GameObject instantiatedModel;

        public void AttemptToSpawnCharacter() {
            if (character != null) {
                instantiatedModel = Instantiate(character);
                AICharacterManager aiCharacter = instantiatedModel.GetComponent<AICharacterManager>();
                instantiatedModel.GetComponent<NetworkObject>().Spawn();

                instantiatedModel.transform.position = spawnPosition;
                instantiatedModel.transform.rotation = Quaternion.Euler(spawnRotation.x, spawnRotation.y, spawnRotation.z);

                if (aiCharacter != null) {
                    aiCharacter.GetComponent<Animator>().applyRootMotion = true;
                    WorldAIManager.instance.AddSpawnedCharacterToActiveCharactersList(aiCharacter);
                }
            }
        }

        public void AttemptToDespawnCharacter() {
            if (instantiatedModel != null) {
                instantiatedModel.GetComponent<NetworkObject>().Despawn();
            }
        }
    }
}
