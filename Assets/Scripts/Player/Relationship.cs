using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    public class Relationship
    {

        public float Friendliness; // will range between 0 - 10, 0 = love 10 = hate
        public GameObject npc;

        public Relationship(GameObject npcValue)
        {
            Debug.Log("Created relationship!");
            Friendliness = 3.0f;
            npc = npcValue;
        }

        public void UpdateRelationship(float change)
        {
            if (change > 0)
            {
                if (Friendliness + change >= 10.0f)
                {
                    Friendliness = 10.0f;
                }
                else
                {
                    Friendliness += change;
                }
            }
            else
            {
                if (Friendliness - change <= 0.0f)
                {
                    Friendliness = 0.0f;
                }
                else
                {
                    Friendliness -= change;
                }
            }
        }
    }
}
