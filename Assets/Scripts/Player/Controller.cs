using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace Player
{
    enum Mode
    {
        HUMAN,
        BEAST
    }

    public class Controller : MonoBehaviour, Combat.IAttacker, Combat.INightEffected
    {
        public float MoveSpeed = 0.5f;
        public float CameraOffset = 2.0f;
        public float Damage = 25.0f;
        public float AttackRange = 2.0f;
        public float AttackCooldown = 1.0f;
        public float WolfSpeed;
        public Camera MainCamera;
        public Vector3 HomeLocation;
        public Image DarknessOverlay;
        public delegate void OverlayDelegate();
        private float _overlayFade = 0.05f;
        private float _health; // we will use a percentage here since changing value depending on mode
        private float _maxHealth;
        private bool _onCooldown = false;
        private Mode _mode;
        private Vector2 _currentMove;
        private Dictionary<int, GameObject> _currentInteractables;
        private Dictionary<string, QuestReception> _currentQuests;
        private Dictionary<string, Relationship> _npcRelationships;
        private List<string> _inventory;
        private GameObject _target;
        private float _originalCameraOffset;
        private GameObject _nearestInteractable;
        private UnityEngine.AI.NavMeshAgent _agent;

        void Awake()
        {
            _originalCameraOffset = CameraOffset;
            _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        }

        void Start()
        {
            _originalCameraOffset = CameraOffset;
            _npcRelationships = new Dictionary<string, Relationship>();
            _currentInteractables = new Dictionary<int, GameObject>();
            _currentQuests = new Dictionary<string, QuestReception>();
            _inventory = new List<string>();
            _health = 1.0f;
            _mode = Mode.HUMAN;
            _CreateNPCRelationships();
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
            _agent.enabled = false;
            _agent.speed = WolfSpeed;
        }

        void Update()
        {

            Bounds cameraBounds = Extensions.CameraExtensions.OrthographicBounds(MainCamera);

            // check if player is within the bounds first
            if ((transform.position.y + CameraOffset > cameraBounds.max.y || transform.position.y - CameraOffset < cameraBounds.min.y) ||
            (transform.position.x + CameraOffset > cameraBounds.max.x || transform.position.x - CameraOffset < cameraBounds.min.x))
            {
                MainCamera.transform.position = Vector3.Lerp(
                    MainCamera.transform.position,
                    new Vector3(transform.position.x, transform.position.y, MainCamera.transform.position.z),
                    (MoveSpeed + 0.2f) * Time.deltaTime);
            }



            if (_mode == Mode.HUMAN)
            {
                Vector2 moveVelocity = (MoveSpeed * Time.deltaTime) * _currentMove;
                transform.position += new Vector3(moveVelocity.x, moveVelocity.y, 0.0f);

                // We iterate over all interactable objects and open the prompt for the nearest one
                // which allows us to interact with the closest interactable object

                float distance = Mathf.Infinity;
                _nearestInteractable = null;

                foreach (KeyValuePair<int, GameObject> entry in _currentInteractables)
                {
                    float tempDistance = Vector3.Distance(transform.position, entry.Value.transform.position);
                    if (tempDistance < distance)
                    {
                        distance = tempDistance;
                        _nearestInteractable = entry.Value;
                    }
                    NPC.Interactable interactScript = entry.Value.GetComponent<NPC.Interactable>();
                    interactScript.ClosePrompt();
                }

                if (_nearestInteractable != null)
                {
                    NPC.Interactable interactScript = _nearestInteractable.GetComponent<NPC.Interactable>();
                    interactScript.Prompt();
                }

                QuestUpdate(); // checks for any quests that are complete
            }
            else
            {
                if (_target)
                {
                    // add logic for beast AI here
                    float targetDistance = Vector3.Distance(_target.transform.position, transform.position);

                    if (targetDistance <= AttackRange && !_onCooldown)
                    {
                        this.Attack(_target);
                    }
                    else
                    {
                        _agent.SetDestination(_target.transform.position);
                    }
                }
                else
                {
                    _FindNewTarget();
                }
            }
        }

        private float _getMaxHealth()
        {
            return _mode == Mode.BEAST ? 150.0f : 100.0f;
        }

        public void ToggleMode()
        {
            if (_mode == Mode.BEAST)
            {
                // change sprite/sound effects, give player back control
                _mode = Mode.HUMAN;
            }
            else
            {
                // change sprite/sound effects, remove player control
                _mode = Mode.BEAST;
            }
        }

        public float Health()
        {
            return _health * _getMaxHealth();
        }

        //Combat

        public void Attack(GameObject other)
        {
            if (!_onCooldown)
            {
                Combat.IDamageable damageAbleScript = other.GetComponent<Combat.IDamageable>();
                Debug.Log(string.Format("Attacked {0}", other));

                if (damageAbleScript != null)
                {
                    // check the type
                    if (damageAbleScript.GetAttackableType() == Combat.AttackableType.NPC)
                    {
                        NPC.Controller npcScript = other.GetComponent<NPC.Controller>();
                        if (!damageAbleScript.TakeDamage(Damage))
                        {
                            _npcRelationships.Remove(npcScript.npc.Name);
                            damageAbleScript.OnDeath();
                            _FindNewTarget();
                        }
                        else
                        {
                            npcScript.Stop(0.3f);
                        }
                    }
                    else if (damageAbleScript.GetAttackableType() == Combat.AttackableType.BARRICADE)
                    {
                        if (!damageAbleScript.TakeDamage(Damage))
                        {
                            _agent.speed = WolfSpeed;
                            damageAbleScript.OnDeath();
                        }
                    }
                }
                StartCoroutine(SetTimeout(AttackCooldown));

            }
        }

        // Quests & dialogue
        struct QuestReception
        {
            public GameObject giver;
            public Dialogue.Quest quest;

            public QuestReception(GameObject questGiver, Dialogue.Quest questValue)
            {
                giver = questGiver;
                quest = questValue;
            }
        }
        public void GetQuest(GameObject giver, Dialogue.Quest quest)
        {
            Debug.Log(quest.GetCurrentStage().dialogue);
            QuestReception q = new QuestReception(giver, quest);

            if (!_currentQuests.ContainsKey(quest.name))
            {
                Debug.Log(string.Format("Received quest {0}!", quest.name));
                _currentQuests.Add(quest.name, q);
            }
        }

        // helper method to check on status of our quests
        public void QuestUpdate()
        {
            foreach (KeyValuePair<string, QuestReception> q in _currentQuests)
            {
                if (!q.Value.quest.complete)
                {
                    Dialogue.Stage tempStage = q.Value.quest.GetCurrentStage();
                    if (tempStage.task.type == Dialogue.TaskType.GATHER &&
                        _inventory.Contains(tempStage.task.objectName))
                    {
                        tempStage.complete = true;

                        // We can mark this stage as done
                        if (q.Value.quest.IncreaseStage())
                        {
                            // can have a pop up showing the quest is complete
                            q.Value.quest.complete = true;
                        }
                    }
                    // need to add other conditions for other task types here
                    // ie maybe for go to we spawn a circle collider in world 
                    // talk to flags the npc for next time talked to
                    // build looks for the "built" items in the world?
                }
            }
        }

        public void AddItemToInventory(string itemName)
        {
            this._inventory.Add(itemName);
            Debug.Log(string.Format("Added {0} to inventory", itemName));
        }

        public void RemoveItemFromInventory(string itemName)
        {
            this._inventory.Remove(itemName);
        }

        public bool TryHandInQuest(GameObject giver)
        {
            foreach (KeyValuePair<string, QuestReception> q in _currentQuests)
            {
                if (q.Value.quest.complete && giver.GetInstanceID() == q.Value.giver.GetInstanceID())
                {
                    Debug.Log(q.Value.quest.completionText);
                    _currentQuests.Remove(q.Key);
                    return true;
                }
            }
            return false;
        }

        // Input callbacks
        public void Move(InputAction.CallbackContext context)
        {
            _currentMove = context.ReadValue<Vector2>();
        }

        public void Interact(InputAction.CallbackContext context)
        {
            if (_nearestInteractable && context.action.triggered && context.ReadValue<float>() > 0)
            {
                NPC.Interactable interactScript = _nearestInteractable.GetComponent<NPC.Interactable>();
                if (interactScript != null)
                {
                    if (!TryHandInQuest(_nearestInteractable))
                    {
                        interactScript.OnInteract(gameObject);
                    }
                }
            }
            else
            {
                // test case manually trigger night time activity
                // OnNightFall();
            }
        }

        // Collision trigger events
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (Extensions.Tagging.IsInteractable(other.gameObject))
            {
                if (!_currentInteractables.ContainsKey(other.gameObject.GetInstanceID()))
                {
                    _currentInteractables.Add(other.gameObject.GetInstanceID(), other.gameObject);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (Extensions.Tagging.IsInteractable(other.gameObject))
            {
                NPC.Interactable interactScript = other.gameObject.GetComponent<NPC.Interactable>();
                interactScript.ClosePrompt();
                _currentInteractables.Remove(other.gameObject.GetInstanceID());
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (_mode == Mode.BEAST)
            {
                NPC.Barricade barricadeScript = other.gameObject.GetComponent<NPC.Barricade>();
                Debug.Log(other.gameObject);

                if (barricadeScript != null)
                {
                    _agent.speed = 0.0f;
                    this.Attack(other.gameObject);
                }
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (_mode == Mode.BEAST)
            {
                NPC.Barricade barricadeScript = other.gameObject.GetComponent<NPC.Barricade>();
                Debug.Log(other.gameObject);

                if (barricadeScript != null)
                {
                    _agent.speed = WolfSpeed;
                }
            }
        }

        private void _CreateNPCRelationships()
        {
            GameObject[] interactObjects = Extensions.Tagging.GetObjectsByTag(Extensions.Tag.INTERACTABLE);

            foreach (GameObject o in interactObjects)
            {
                NPC.Controller npcScript = o.GetComponent<NPC.Controller>();

                if (npcScript != null && !_npcRelationships.ContainsKey(npcScript.npc.Name))
                {
                    _npcRelationships.Add(npcScript.npc.Name, new Relationship(o));
                }
            }
        }

        private void _EndNight()
        {
            CameraOffset = _originalCameraOffset;
            _target = null;
            _mode = Mode.HUMAN;
            _agent.speed = 0.0f;
            _agent.enabled = false;
        }

        public void OnNightEnd()
        {
            // this is still buggy so leaving it out
            // OverlayDelegate method = _EndNight;
            // // have the character get teleported to home position
            // StartCoroutine(FadeToBlack(method));
            _EndNight();
        }

        private void _StartNight()
        {
            // fire logic for picking npc targets
            CameraOffset = 8.0f;
            _FindNewTarget();
            Debug.Log(_target);
            _mode = Mode.BEAST;
            _agent.speed = WolfSpeed;
            _agent.enabled = true;
        }

        public void OnNightFall()
        {
            // this is still buggy so leaving it out
            // OverlayDelegate method = _StartNight;
            // // have the character get teleported to home position
            // StartCoroutine(FadeToBlack(method));
            _StartNight();
        }

        private void _FindNewTarget()
        {
            float value = Mathf.Infinity;
            string key = "";

            // lets choose our target based off of distance and friendliness
            foreach (KeyValuePair<string, Relationship> relation in _npcRelationships)
            {
                float tempDistance = Vector3.Distance(relation.Value.npc.transform.position, transform.position);
                float tempValue = tempDistance + relation.Value.Friendliness;

                // get npc script
                NPC.Controller tempController = relation.Value.npc.GetComponent<NPC.Controller>();
                if (tempController.npc.IsAlive() && tempValue < value)
                {
                    value = tempValue;
                    key = relation.Key;
                }
            }

            if (_npcRelationships.ContainsKey(key))
            {
                _target = _npcRelationships[key].npc;
            }
        }

        private IEnumerator SetTimeout(float time)
        {
            _onCooldown = true;
            yield return new WaitForSeconds(time);
            _onCooldown = false;
        }

        private IEnumerator FadeToBlack(OverlayDelegate method)
        {
            var tempColor = DarknessOverlay.color;

            while (tempColor.a <= 1.2f)
            {
                if (tempColor.a >= 1.0)
                {
                    transform.position = HomeLocation;
                }

                tempColor.a += _overlayFade;
                DarknessOverlay.color = tempColor;
                yield return new WaitForSeconds(_overlayFade);
            }
            StartCoroutine(WakeFromBlack(method));
        }

        private IEnumerator WakeFromBlack(OverlayDelegate method)
        {
            var tempColor = DarknessOverlay.color;

            while (tempColor.a >= -0.2f)
            {
                if (tempColor.a <= 0.8f)
                {
                    transform.position = HomeLocation;
                    method();
                }

                tempColor.a -= _overlayFade;
                DarknessOverlay.color = tempColor;
                yield return new WaitForSeconds(_overlayFade);
            }
        }
    }
}
