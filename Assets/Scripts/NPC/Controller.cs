using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    interface INPC
    {
        void MoveTo(Vector2 position);
        void Stop(float stopTime);
        void ReturnHome();
    }

    [System.Serializable]
    public class NPC
    {
        public Vector2 HomeLocation;
        public float Damage;
        public string Name;
        public float MoveSpeed;
        public Dialogue.Days dialogueData;
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
            this.dialogueData = Dialogue.JSONReader.ReadDialogueFile(this.Name);
        }

        public string GetGreeting(int day)
        {
            return this.dialogueData.days[day].GetGreeting();
        }

        public bool HasQuest(int index)
        {
            // hardcode the day to 0 for now since days not implemented
            foreach (Dialogue.Quest quest in this.dialogueData.days[0].quests)
            {
                // Debug.Log(string.Format("herre {0}", quest));
                if (!quest.complete && !quest.started)
                {
                    return true;
                }
            }

            return false;
        }

        public Dialogue.Quest GetQuest()
        {
            Dialogue.Quest temp = this.dialogueData.days[0].quests[0]; // won't matter since we check if any are available so return is just there for compiler

            // hardcode the day to 0 for now since days not implemented
            foreach (Dialogue.Quest quest in this.dialogueData.days[0].quests)
            {
                if (!quest.complete && !quest.started)
                {
                    quest.started = true;
                    return quest;
                }
            }
            return temp;
        }
        public float Health { get => _health; set => _health = value; }
        public float MaxHealth { get => _maxHealth; set => _maxHealth = value; }

        public bool IsAlive()
        {
            return this.Health > 0.0f;
        }
    }

    public class Controller : Interactable, INPC, Combat.IDamageable, Combat.INightEffected
    {
        public NPC npc;
        public GameObject player;
        public float sight;
        private UnityEngine.AI.NavMeshAgent _agent;
        private bool _onAlert = true;
        void Start()
        {
            this.npc = new NPC(npc.Name, npc.HomeLocation, npc.Damage, npc.MoveSpeed);
            _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            _agent.speed = npc.MoveSpeed;
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
        }

        void Update()
        {
            if (_onAlert)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

                if (distanceToPlayer <= sight)
                {
                    Vector3 difference = transform.position - player.transform.position;
                    difference *= 2.0f;
                    MoveTo(new Vector2(difference.x, difference.y));
                }
            }
        }

        public void OnDeath()
        {
            Debug.Log("I died!");
            StopAllCoroutines();
            Destroy(gameObject);
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

        public void Stop(float stopTime)
        {
            StartCoroutine(StopWait(stopTime));
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
        public override void OnInteract(GameObject other)
        {
            // Debug.Log(string.Format("You interacted with {0}!", this.npc.Name));
            // For now lets just set a random position for the npc to move to for testing purposes

            // we can make the npc do all sorts of things here when we interact
            // this.MoveTo(new Vector2(Random.value * 50, Random.value * 50));
            // this.Attack(player);

            // check if day and has a quest then we can give
            if (npc.HasQuest(0))
            {
                Player.Controller playerScript = other.GetComponent<Player.Controller>();

                if (playerScript != null)
                {
                    playerScript.GetQuest(gameObject, this.npc.GetQuest());
                }
            }
            else
            {
                Debug.Log(this.npc.GetGreeting(0));
            }
        }

        public override void OnEndInteract()
        {
            Debug.Log(string.Format("You are no longer interacting with {0}!", this.npc.Name));
        }

        public override void Prompt()
        {
            // can add a button prompt here ("Press e")
            if (this.npc.HasQuest(0))
            {
                // we can put a marker over them or something?
            }
        }
        public override void ClosePrompt()
        {
            // remove the prompt
        }

        // Night logic
        public void OnNightFall()
        {
            // implement running away/barricading from player
            _onAlert = true;
        }

        public void OnNightEnd()
        {
            // resume old behavior/fix up stuff


        }

        private IEnumerator StopWait(float stopTime)
        {
            float oldSpeed = _agent.speed;
            _agent.speed = 0.0f;
            yield return new WaitForSeconds(stopTime);
            _agent.speed = oldSpeed;
        }
    }
}
