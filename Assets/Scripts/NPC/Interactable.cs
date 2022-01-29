using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    interface IInteractable
    {
        void OnInteract(GameObject other);
        void OnEndInteract();
        void Prompt();
        void ClosePrompt();
    }

    public abstract class Interactable : MonoBehaviour, IInteractable
    {
        public abstract void OnInteract(GameObject other);

        public abstract void OnEndInteract();

        public abstract void Prompt();
        public abstract void ClosePrompt();
    }
}
