using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PlayerStats : MonoBehaviour {
        public int healthLevel = 10;
        public int maxHealth;
        public int currentHealth;

        public HealthBar healthBar;
        AnimatorHandler animatorHandler;
        private void Awake() {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
        }
        private void Start() {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
        }
        
        private int SetMaxHealthFromHealthLevel() {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        public void TakeDamage(int damage) {
            currentHealth -= damage;
            healthBar.SetCurrentHealth(currentHealth);
            animatorHandler.PlayTargetAnimation("Damage", true);

            if (currentHealth <= 0) {
                currentHealth = 0;
                animatorHandler.PlayTargetAnimation("Dead", true);
                // Dead �ִϸ��̼��� Transition���� Empty�� �����س��� �ʾҴ�.
                // Empty ���� isInteracting �׸��� �ʱ�ȭ �����ν� ���� �ִϸ��̼����� �Ѿ�� �ְԲ� ���ִµ� �÷��̾ �״´ٸ� �׷��ʿ� ����
            }
        }
    }
}