using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class IllusionaryWall : MonoBehaviour {
        public int illusionaryWallHealthPoint = 1;
        public Material illusionaryWallMaterial;
        public float alpha;
        public float fadeTimer = 2.5f; // 환영벽이 사라지는데 걸리는 시간
        public BoxCollider wallCollider;

        private void Awake() {
            illusionaryWallMaterial.color = new Color(1, 1, 1, 1);   
        }

        private void Update() {
            if (illusionaryWallHealthPoint <= 0)
                FadeIllusionaryWall();
        }

        private void FadeIllusionaryWall() {
            alpha = illusionaryWallMaterial.color.a; // 환영벽의 알파값
            alpha -= Time.deltaTime / fadeTimer;
            Color fadedWallColor = new Color(1, 1, 1, alpha);
            illusionaryWallMaterial.color = fadedWallColor;

            if (wallCollider.enabled) {
                wallCollider.enabled = false;
            }

            if (alpha <= 0) {
                Destroy(this);
            }
        }
    }
}
