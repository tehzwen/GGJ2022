using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Extensions
{
    public static class Tagging
    {
        public static bool IsInteractable(GameObject other)
        {
            return other.tag == "Interactable";
        }
    }
}
