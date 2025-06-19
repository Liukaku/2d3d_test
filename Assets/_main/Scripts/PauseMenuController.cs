using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpriteGame
{
    public class PauseMenuController : MonoBehaviour
    {
        public bool IsPaused = false;

        [SerializeField] 
        private GameObject pauseMenuUI;

        [SerializeField]
        private GameObject homePauseMenuUI;

        [SerializeField]
        private GameObject questListUI;

        [SerializeField]
        private GameObject questObject;

        [SerializeField]
        private GameObject questStep;

        private Button ResumeButton, QuestLogButton, InventoryButton;

        private void Awake()
        {
            ResumeButton = GameObject.Find("ResumeButton").GetComponent<Button>();
            ResumeButton.onClick.AddListener(() => OnButtonClick("ResumeButton"));

            QuestLogButton = GameObject.Find("QuestLogButton").GetComponent<Button>();
            QuestLogButton.onClick.AddListener(() => OnButtonClick("QuestLogButton"));

            InventoryButton = GameObject.Find("InventoryButton").GetComponent<Button>();
            InventoryButton.onClick.AddListener(() => OnButtonClick("InventoryButton"));




        }

        private void OnButtonClick(string buttonLabel)
        {
            Debug.Log("Button clicked: " + buttonLabel);
            switch (buttonLabel)
            {
                case "ResumeButton":
                    HandlePause();
                    break;
                case "QuestLogButton":
                    ToggleQuestList();
                    break;
                case "InventoryButton":
                    //ToggleInventory();
                    break;
                default:
                    Debug.LogWarning("Unknown button clicked: " + buttonLabel);
                    break;
            }
        }

        private GameObject CreateQuestListEntry(Quest quest)
        {
            // create a new GameObject for the quest entry using the questObject prefab
            GameObject questEntry = Instantiate(questObject, questListUI.transform);
            // set the quest title and description in the UI
            questEntry.transform.Find("QuestTitle").GetComponent<TextMeshProUGUI>().text = quest.title;
            questEntry.transform.Find("QuestBody").GetComponent<TextMeshProUGUI>().text = quest.description;
            GameObject questStepsContainer = questEntry.transform.Find("QuestSteps").gameObject;
            // create a new GameObject for each quest step using the questStep prefab
            foreach (QuestStep step in quest.steps)
            {
                GameObject stepEntry = Instantiate(questStep, questStepsContainer.transform);
                // set the step description and state in the UI
                stepEntry.transform.GetComponent<TextMeshProUGUI>().text = step.description;
            }


            return questEntry;
        }

        public void PopulateQuestList(List<Quest> quests)
        {
            // QuestList > Viewport > Content
            GameObject contentTransform = questListUI.gameObject
                .transform.Find("QuestList")
                .transform.Find("Viewport")
                .Find("Content")
                .gameObject;
            // Clear existing quest entries
            //foreach (Transform child in contentTransform)
            //{
            //    Destroy(child.gameObject);
            //}
            // Populate the quest list with new entries
            foreach (Quest quest in quests)
            {
                GameObject questEntry = CreateQuestListEntry(quest);
                questEntry.gameObject.transform.SetParent(contentTransform.transform, false);
            }

            pauseMenuUI.SetActive(false);
        }

        private void ToggleQuestList()
        {
            if (questListUI.activeSelf)
            {
                questListUI.SetActive(false);
                homePauseMenuUI.SetActive(true);
            }
            else
            {
                homePauseMenuUI.SetActive(false);
                questListUI.SetActive(true);
            }
        }


        public void HandlePause()
        {
            if (IsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }


        private void PauseGame()
        {
            IsPaused = true;
            pauseMenuUI.SetActive(true);
            HandleGameSpeed();
        }

        private void ResumeGame()
        {
            IsPaused = false;
            pauseMenuUI.SetActive(false);
            HandleGameSpeed();
        }
        private void HandleGameSpeed()
        {
            if (IsPaused)
            {
                Time.timeScale = 0f; // Pause the game
                Debug.Log("Game Paused");
            }
            else
            {
                Time.timeScale = 1f; // Resume the game
                Debug.Log("Game Resumed");
            }
        }
    }
}
