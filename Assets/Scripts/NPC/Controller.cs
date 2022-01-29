using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    interface INPC
    {
        void MoveTo(Vector2 position);
        void ReturnHome();
    }

    [System.Serializable]
    public class NPC
    {
        public Vector2 HomeLocation;
        public float Damage;
        public string Name;
        public float MoveSpeed;

        private float _health;
        private float _maxHealth;
        public NPC(string name, Vector2 location, float damage, float moveSpeed)
        {
            this._maxHealth = 100.0f;
            this._health = 100.0f;
            this.Name = name;
            this.HomeLocation = location;
            this.Damage = damage;
            this.MoveSpeed = moveSpeed;
        }

        public float Health { get => _health; set => _health = value; }
        public float MaxHealth { get => _maxHealth; set => _maxHealth = value; }

        public bool IsAlive()
        {
            return this.Health <= 0.0f;
        }
    }

    public class Controller : Interactable, INPC, Combat.IDamageable
    {
        public NPC npc;
        public GameObject player;
        private UnityEngine.AI.NavMeshAgent _agent;
        void Start()
        {
            _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            _agent.speed = npc.MoveSpeed;
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
        }

        void Update()
        {

        }

        public void OnDeath()
        {
            Debug.Log("I died!");
        }

        public void Attack(GameObject other)
        {
            Debug.Log(string.Format("I attacked {0}", other));
            Combat.IDamageable damageScript = other.GetComponent<Combat.IDamageable>();

            if (damageScript != null)
            {
                Debug.Log("Attacked!");
                damageScript.TakeDamage(this.npc.Damage);
            }
            else
            {
                Debug.Log("Error!");
            }
        }

        public void MoveTo(Vector2 position)
        {
            Debug.Log(string.Format("Going to move to {0}", position));
            _agent.SetDestination(position);
        }

        public void ReturnHome()
        {
            // play dialogue about going home
            this.MoveTo(this.npc.HomeLocation);
        }

        public bool TakeDamage(float damage)
        {
            // can play dialogue for takign damage along with sound/blood sprite
            this.npc.Health -= damage;
            return this.npc.IsAlive();
        }

        public void Heal(float amount)
        {
            // play dialogue/animation for healing
            if (this.npc.Health + amount < this.npc.MaxHealth)
            {
                this.npc.Health += amount;
            }
            else
            {
                this.npc.Health = this.npc.MaxHealth;
            }
        }

        // Interactions
        public override void OnInteract()
        {
            Debug.Log(string.Format("You interacted with {0}!", this.npc.Name));
            // For now lets just set a random position for the npc to move to for testing purposes
            // this.MoveTo(new Vector2(Random.value * 50, Random.value * 50));
            this.Attack(player);
        }

        public override void OnEndInteract()
        {
            Debug.Log(string.Format("You are no longer interacting with {0}!", this.npc.Name));
        }

        public override void Prompt()
        {
            // can add a button prompt here ("Press e")
        }
        public override void ClosePrompt()
        {
            // remove the prompt
        }
    }
}
