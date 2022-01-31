using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
    public static class Finder
    {
        public class NightAffectedObject
        {
            public GameObject ObjectValue;
            public Combat.INightEffected NightScript;

            public NightAffectedObject(GameObject o, Combat.INightEffected script)
            {
                this.ObjectValue = o;
                this.NightScript = script;
            }
        }
        public static NightAffectedObject[] GetNightAffectedObjects()
        {
            List<NightAffectedObject> nightObjects = new List<NightAffectedObject>();
            GameObject[] objects = Tagging.GetObjectsByTag(Tag.INTERACTABLE);

            for (int i = 0; i < objects.Length; i++)
            {
                NPC.Barricade barricadeScript = objects[i].GetComponent<NPC.Barricade>();
                NPC.Controller npcScript = objects[i].GetComponent<NPC.Controller>();

                if (barricadeScript != null)
                {
                    nightObjects.Add(new NightAffectedObject(objects[i], barricadeScript));
                }
                else if (npcScript != null)
                {
                    nightObjects.Add(new NightAffectedObject(objects[i], npcScript));
                }
            }

            return nightObjects.ToArray();
        }
    }
}
