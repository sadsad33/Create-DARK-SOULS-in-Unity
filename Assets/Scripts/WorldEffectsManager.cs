using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class WorldEffectsManager : MonoBehaviour {
        public static WorldEffectsManager instance;

        [Header("Damage")]
        public TakeDamageEffect takeDamageEffect;
        public TakeBlockedDamageEffect takeBlockedDamageEffect;

        [Header("Poison")]
        public PoisonBuildUpEffect poisonBuildUpEffect;
        public PoisonedEffect poisonedEffect;
        public GameObject poisonFX;
        // 독에 중독된 상태에서의 사운드 이펙트
        // public AudioClip poisonSFX
        private void Awake() {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }

        private void Start() {
            DontDestroyOnLoad(this);
        }
    }
}
