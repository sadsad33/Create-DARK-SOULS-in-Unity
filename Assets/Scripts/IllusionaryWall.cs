using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class IllusionaryWall : MonoBehaviour {
        public int illusionaryWallHealthPoint = 1;
        public Material illusionaryWallMaterial;
        public float alpha;
        public float fadeTimer = 2.5f; // ȯ������ ������µ� �ɸ��� �ð�
        public BoxCollider wallCollider;

        private void Awake() {
            illusionaryWallMaterial.color = new Color(1, 1, 1, 1);   
        }

        private void Update() {
            if (illusionaryWallHealthPoint <= 0)
                FadeIllusionaryWall();
        }

        private void FadeIllusionaryWall() {
            alpha = illusionaryWallMaterial.color.a; // ȯ������ ���İ�
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
