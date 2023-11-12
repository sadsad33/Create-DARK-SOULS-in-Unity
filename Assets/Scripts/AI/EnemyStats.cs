using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class EnemyStats : CharacterStats {
        Animator animator;
        private void Awake() {
            animator = GetComponentInChildren<Animator>();
        }

        private void Start() {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
        }

        private float SetMaxHealthFromHealthLevel() {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        public void TakeDamageNoAnimation(float damage) {
            Debug.Log("���� ������ : " + damage);
            currentHealth -= damage;
            Debug.Log("���� ü�� : " + currentHealth);
            if (currentHealth <= 0) {
                currentHealth = 0;
                isDead = true;
            }
        }

        public void TakeDamage(float damage) {
            if (isDead) return;
            currentHealth -= damage;
            animator.Play("Damage");

            if (currentHealth <= 0) {
                currentHealth = 0;
                animator.Play("Dead");
                isDead = true;
            }
        }
    }
}
