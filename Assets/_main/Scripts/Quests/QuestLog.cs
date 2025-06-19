using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteGame
{
    public class QuestLog : MonoBehaviour
    {
        [SerializeField]
        public List<Quest> quests;
        [SerializeField]
        private PauseMenuController pauseMenuController;

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

            pauseMenuController = GameObject.Find("PauseMenu").GetComponent<PauseMenuController>();
            if (pauseMenuController != null)
            {
                pauseMenuController.PopulateQuestList(quests);
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

        public void ItemPickup(string itemUid)
        {
            // separate functions in case i need to add more logic in later
            CheckUidAgainstStepRequirements(itemUid);
        }

        public void DefeatNpc(string npcUid)
        {
            // separate functions in case i need to add more logic in later
            CheckUidAgainstStepRequirements(npcUid);
        }

        private void CheckUidAgainstStepRequirements(string uid)
        {
            List<Quest> activeQuests = quests.FindAll(q => q.state == QuestState.IN_PROGRESS);
            foreach (Quest quest in activeQuests)
            {
                QuestStep currStep = quest.steps[quest.currentStep];
                if (currStep.targetUid == uid && currStep.targetCount > 0)
                {
                    currStep.targetCount--;
                }

                if (currStep.targetCount <= 0)
                {
                    currStep.state = QuestState.COMPLETE;
                    ProgressQuest(quest);
                    Debug.Log($"Quest '{quest.title}' step {currStep.stepCount} completed.");
                }
            }
        }
    }
}
