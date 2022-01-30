using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Extensions
{
    public static class Tag
    {
        public const string INTERACTABLE = "Interactable";
    }
    public static class Tagging
    {
        public static bool IsInteractable(GameObject other)
        {
            return other.tag == "Interactable";
        }

        public static GameObject[] GetObjectsByTag(string tagValue)
        {
            return GameObject.FindGameObjectsWithTag(tagValue);
        }
    }
}
