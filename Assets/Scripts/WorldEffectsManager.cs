using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class WorldEffectsManager : MonoBehaviour {
        public static WorldEffectsManager instance;

        public PoisonBuildUpEffect poisonBuildUpEffect;
        public PoisonEffect poisonedEffect;
        private void Awake() {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }
    }
}
