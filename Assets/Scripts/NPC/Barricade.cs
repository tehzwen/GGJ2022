using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    public class Barricade : Interactable, Combat.IDamageable, Combat.INightEffected
    {
        private float _health = 150.0f;
        void Start()
        {

        }

        void Update()
        {

        }

        private bool _IsAlive()
        {
            return this._health > 0.0f;
        }

        public bool TakeDamage(float damage)
        {
            this._health -= damage;
            return this._IsAlive();
        }

        public Combat.AttackableType GetAttackableType()
        {
            return Combat.AttackableType.BARRICADE;
        }
        public void Heal(float amount)
        {

        }
        public void OnDeath()
        {
            Destroy(gameObject);
        }

        public override void OnInteract(GameObject other)
        {
            Debug.Log("You interacted with me!");
        }

        public override void OnEndInteract()
        {
            Debug.Log("I'm no longer an item!");
        }

        public override void Prompt()
        {
            Debug.Log("Sleepy time!");
        }
        public override void ClosePrompt()
        {
            Debug.Log("Not Sleepy time!");
        }

        public void OnNightFall()
        {
            Collider2D coll = GetComponent<Collider2D>();
            coll.enabled = true;
        }

        public void OnNightEnd()
        {
            Collider2D coll = GetComponent<Collider2D>();
            coll.enabled = false;
            Debug.Log("Here!");
        }
    }
}
