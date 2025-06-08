using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace SpriteGame
{
    public class DialogManager : MonoBehaviour
    {
        public string CharacterName;
        public string CharacterDialogFilename;
        public string BaseImage;
        public DialogObject dialogObject;
        public bool inConversation = false;
        public string DialogUid;
        public string CurrentQuestUid;

        private QuestLog questLog;

        [SerializeField]
        private GameObject DialogCanvas;
        [SerializeField]
        private TextMeshProUGUI DialogBody;
        [SerializeField]
        private ButtonManager ButtonManager;
        [SerializeField]
        private List<DialogOption> currentDialogOptions = new();
        [SerializeField]
        private DialogOption currentDialogOption = new();
        private int currentDialogPage = 0;
        private Image spriteRenderer;
        [SerializeField]
        private Dictionary<string, Sprite> sprites;



        private void Awake()
        {
            DialogCanvas = GameObject.Find("DialogCanvas");
            DialogBody = GameObject.Find("DialogBodyText").GetComponent<TextMeshProUGUI>();
            ButtonManager = DialogCanvas.GetComponentInChildren<ButtonManager>();
            GameObject conversationImage = GameObject.Find("ConversationImage");
            spriteRenderer = conversationImage.GetComponent<Image>();
            sprites = new Dictionary<string, Sprite>();
            // Load the base image
            Sprite[] loadedSprites = Resources.LoadAll<Sprite>("Images/Characters/" + CharacterDialogFilename);
            Debug.Log(loadedSprites.Length + " sprites loaded from Resources/Images/Characters/" + CharacterDialogFilename);
            foreach (Sprite sprite in loadedSprites)
            {
                sprites.Add(sprite.name, sprite);
            }


            // load in the dialog json file from Assets/_main/Scripts
            // and assign it to the dialogObject variable
            string path = "Assets/_main/Scripts/DBs/Dialog/" + CharacterDialogFilename + ".json";
            using StreamReader r = new(path);
            string jsonFile = r.ReadToEnd();
            DialogObject obj = new();
            if (jsonFile != null)
            {
                var test = JsonUtility.FromJson<DialogObject>(jsonFile);
                dialogObject = test;

                UpdateDialogText(test.general.optionsByTier[0].options[0].body);

            }
            else
            {
                Debug.LogError("Could not find dialog file: " + CharacterDialogFilename);
                Debug.LogError("Path: " + path);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            DialogCanvas.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void PauseGameForDialog(bool pausing)
        {
            if (pausing)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }

        public void CloseDialog()
        {
            PauseGameForDialog(false);
            DialogCanvas.SetActive(false);
            inConversation = false;
        }

        public void ClearDialogBody()
        {
            DialogBody.text = "";
        }

        public void StartDialog(float vibe, QuestLog log)
        {
            inConversation = true;
            ButtonManager.SetDialogManager(this);

            // This loops through the quests, checks if the current step's targetUid matches the DialogUid,
            questLog = log;
            List<Quest> questsForNpc = questLog.quests.Where(q => q.steps[q.currentStep].targetUid == DialogUid).ToList();

            if (questsForNpc.Count > 0)
            {
                Debug.Log("Found " + questsForNpc.Count + " quests for NPC: " + DialogUid);
                var dialogOptions = dialogObject.story.quests.Where(d => d.questUid == questsForNpc[0].uid).ToList();
                // filter out the dialog options based on the current quest step
                var index = dialogOptions.FindIndex(d => d.questStepIndex == questsForNpc[0].currentStep);
                currentDialogOptions = dialogOptions[index].options;
                CurrentQuestUid = dialogOptions[index].questUid;

            } else
            {
                List<OptionsByTier> tiers = dialogObject.general.optionsByTier.FindAll((n) => n.tier <= vibe);
                tiers.Sort((a, b) => a.tier.CompareTo(b.tier));
                //Debug.Log(tiers[0].tier);
                currentDialogOptions = tiers[0].options;
                //Debug.Log(currentDialogOption.pages.Count);
            }

            currentDialogOption = currentDialogOptions.Find((n) => n.key == "greeting");
            DialogCanvas.SetActive(true);
            PauseGameForDialog(true);
            UpdateDialogText(currentDialogOption.body);
            SetConversationImage(currentDialogOption.image);
            ButtonManager.DisableAllButtons();
        }

        // set the ConversationImage object sprite to the character image
        public void SetConversationImage(string imageName)
        {
            if (sprites.ContainsKey(imageName))
            {
                spriteRenderer.sprite = sprites[imageName];
            }
            else
            {
                Debug.LogError("Sprite not found: " + imageName);
                spriteRenderer.sprite = sprites["default"];
            }
        }

        public void UpdateDialogText(string bodyText)
        {
            DialogBody.text = bodyText;
        }

        private void HandleDisplayingResponses(List<DialogResponse> responses)
        {
            if (responses.Count != 0)
            {
                ButtonManager.SetDialogButtonOptions(responses);
            }
            else
            {
                currentDialogPage = 0;
                CloseDialog();
            }
        }

        public void NextPage()
        {
            int numberOfPages = currentDialogOption.pages.Count;

            if (currentDialogOption.progressQuest == true)
            {
                var qIndex = questLog.quests.FindIndex(q => q.uid == CurrentQuestUid);
                questLog.ProgressQuest(questLog.quests[qIndex]);
            }

            if (numberOfPages > 0)
            {
                currentDialogPage++;
                Debug.Log("current page count: " + numberOfPages);
                // If we are the last page, we need to check if there are any responses
                Debug.Log("first check: " + (numberOfPages != 0));
                Debug.Log("second check: " + ((currentDialogPage - 1) >= numberOfPages));
                if (currentDialogOption.pages.Count != 0 && (currentDialogPage - 1) >= numberOfPages)
                {
                    if(currentDialogOption.pages[numberOfPages - 1].responses.Count != 0)
                    {
                        HandleDisplayingResponses(currentDialogOption.pages[numberOfPages - 1].responses);
                    }
                    else
                    {
                        currentDialogPage = 0;
                        CloseDialog();
                    }

                } else
                {
                    UpdateDialogText(currentDialogOption.pages[currentDialogPage - 1].body);
                    return;
                }
            }
            else
            {
                Debug.Log("No more pages to display");
                HandleDisplayingResponses(currentDialogOption.responses);
            }
            return;
        }

        public void HandleOption(int option, DialogResponse response)
        {
            try
            {
                //Debug.Log("Option " + option + " clicked");
                //Debug.Log("Response: " + response.body);
                currentDialogOption = currentDialogOptions.Find((n) => n.key == response.next);
                UpdateDialogText(currentDialogOption.body);
                ButtonManager.DisableAllButtons();
                ButtonManager.NextPageButton.gameObject.SetActive(true);
                // Handle the option click
                // You can use the option index to determine which option was clicked
                // For example, you can update the dialog text based on the selected option
                // UpdateDialogText(dialogObject.general.optionsByTier[0].options[option].body);
                // CloseDialog();
            }
            catch (Exception e)
            {
                Debug.LogError("Error handling option: " + e.Message);
                CloseDialog();
            }

        }
    }
}

