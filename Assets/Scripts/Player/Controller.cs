using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    enum Mode
    {
        HUMAN,
        BEAST
    }

    public class Controller : MonoBehaviour, Combat.IDamageable
    {
        public float MoveSpeed = 0.5f;
        public float CameraOffset = 2.0f;
        public Camera MainCamera;
        private float _health; // we will use a percentage here since changing value depending on mode
        private float _maxHealth;
        private Mode _mode;
        private Vector2 _currentMove;
        private Dictionary<int, GameObject> _currentInteractables;
        private GameObject _nearestInteractable;
        void Start()
        {
            _currentInteractables = new Dictionary<int, GameObject>();
            _health = 1.0f;
            _mode = Mode.HUMAN;
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
            
        }

        public bool TakeDamage(float damage)
        {
            if (this.Health() - damage <= 0.0f)
            {
                _health = 0.0f;
                return true;
            }
            else
            {
                _health = (this.Health() - damage) / this._getMaxHealth();
                return false;
            }
        }

        public void Heal(float amount)
        {
            if (this.Health() + amount < this._getMaxHealth())
            {
                _health = (this.Health() + amount) / this._getMaxHealth();
            }
            else
            {
                _health = this._getMaxHealth();
            }
        }

        public void OnDeath()
        {
            Debug.Log("I died!");
            //end game?
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
                Debug.Log(interactScript);
                interactScript.OnInteract();
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
    }
}
