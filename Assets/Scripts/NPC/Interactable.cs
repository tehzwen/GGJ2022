using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    interface IInteractable
    {
        void OnInteract();
        void OnEndInteract();
        void Prompt();
        void ClosePrompt();
    }

    public abstract class Interactable : MonoBehaviour, IInteractable
    {
        public abstract void OnInteract();

        public abstract void OnEndInteract();

        public abstract void Prompt();
        public abstract void ClosePrompt();
    }
}
