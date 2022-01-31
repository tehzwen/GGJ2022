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
    public interface IDamageable
    {
        bool TakeDamage(float damage);
        void Heal(float amount);
        void OnDeath();
        AttackableType GetAttackableType();
    }

    public interface IAttacker
    {
        void Attack(GameObject other);
    }

    public interface INightEffected
    {
        void OnNightFall();
        void OnNightEnd();
    }
}
