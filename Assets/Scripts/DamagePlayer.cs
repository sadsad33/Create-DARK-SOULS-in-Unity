using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class DamagePlayer : MonoBehaviour {
        private float damage = 25;
        private void OnTriggerEnter(Collider other) {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null) {
                playerStats.TakeDamage(damage);
            }
        }
    }
}
