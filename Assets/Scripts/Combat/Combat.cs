using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    interface IDamageable
    {
        bool TakeDamage(float damage);
        void Heal(float amount);
        void OnDeath();
        void Attack(GameObject other);
    }
}
