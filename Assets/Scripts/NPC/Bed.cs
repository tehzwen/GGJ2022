using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    public class Bed : Interactable
    {
        void Start()
        {

        }

        void Update()
        {

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
    }
}
