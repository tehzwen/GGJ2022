using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public enum AttackableType
    {
        BARRICADE,
        NPC
    }
    interface IDamageable
    {
        bool TakeDamage(float damage);
        void Heal(float amount);
        void OnDeath();
        AttackableType GetAttackableType();
    }

    interface IAttacker
    {
        void Attack(GameObject other);
    }

    interface INightEffected
    {
        void OnNightFall();
        void OnNightEnd();
    }
}
