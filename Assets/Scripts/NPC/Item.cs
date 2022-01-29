using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    public class Item : Interactable
    {
        public GameObject prompt;
        void Start()
        {

        }

        void Update()
        {

        }

        public override void OnInteract()
        {
            Debug.Log("You interacted with me!");
        }

        public override void OnEndInteract()
        {
            Debug.Log("I'm no longer an item!");
        }

        public override void Prompt()
        {
            prompt.SetActive(true);
        }
        public override void ClosePrompt()
        {
            prompt.SetActive(false);
        }
    }
}
