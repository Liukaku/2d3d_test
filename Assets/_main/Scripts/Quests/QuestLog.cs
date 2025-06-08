using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteGame
{
    public class QuestLog : MonoBehaviour
    {
        [SerializeField]
        public List<Quest> quests;

        private void Awake()
        {
            quests = new List<Quest>();
            // Load quests from JSON files in the specified path
            string path = "Assets/_main/Scripts/DBs/Quests";
            string[] questFiles = System.IO.Directory.GetFiles(path, "*.json");
            foreach (string questFile in questFiles)
            {
                string jsonContent = System.IO.File.ReadAllText(questFile);
                Quest quest = JsonUtility.FromJson<Quest>(jsonContent);
                quests.Add(quest);
            }
        }

        public void ProgressQuest(Quest quest)
        {
            int qIndex = quests.FindIndex(q => q.uid == quest.uid);
            quests[qIndex].currentStep++;
            if (quests[qIndex].currentStep >= quests[qIndex].steps.Length)
            {
                quests[qIndex].state = QuestState.COMPLETE;
            }
            else
            {
                quests[qIndex].state = QuestState.IN_PROGRESS;
            }
        }
    }
}
