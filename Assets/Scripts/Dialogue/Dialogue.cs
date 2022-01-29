using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dialogue
{
    public enum TaskType
    {
        GATHER,
        TALK_TO,
        GO_TO,
        BUILD
    }

    [System.Serializable]
    public class Task
    {
        public string objectName;
        public TaskType type;
    }

    [System.Serializable]
    public class Stage
    {
        public string dialogue;
        public bool complete = false;
        public Task task;
    }

    [System.Serializable]
    public class Quest
    {
        public string name;
        public Stage[] stages;
        public string completionText;
        public int currentStage = 0;
        public bool complete = false;
        public bool started = false;

        public bool IncreaseStage()
        {
            currentStage++;
            Debug.Log(string.Format("Current Stage: {0}, Stages: {1}", this.currentStage, this.stages.Length));
            return currentStage >= stages.Length - 1;
        }

        public Stage GetCurrentStage()
        {
            // Debug.Log(string.Format("Current Stage: {0}, Stages: {1}", this.currentStage, this.stages.Length));
            return this.stages[this.currentStage];
        }
        public bool IsComplete()
        {
            foreach (Stage stage in stages)
            {
                if (!stage.complete)
                {
                    return false;
                }
            }
            return true;
        }
    }

    [System.Serializable]
    public class Day
    {
        public int dayNumber;
        public string[] greetings;
        public Quest[] quests;

        private int _lastGreeting = 0;

        public string GetGreeting()
        {
            int greetingIndex = Random.Range(0, greetings.Length);

            if (greetings.Length > 1 && greetingIndex == _lastGreeting) // make sure its unique!
            {
                return GetGreeting();
            }
            else
            {
                _lastGreeting = greetingIndex;
                return greetings[_lastGreeting];
            }
        }
    }

    [System.Serializable]
    public class Days
    {
        public Day[] days;
    }
    public class JSONReader
    {
        public static Days ReadDialogueFile(string filename)
        {
            TextAsset jsonData = Resources.Load("Dialogue/" + filename) as TextAsset;
            Days messagesInJson = JsonUtility.FromJson<Days>(jsonData.text);
            return messagesInJson;
        }
    }
}
