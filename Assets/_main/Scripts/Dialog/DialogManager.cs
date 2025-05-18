using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;

namespace SpriteGame
{
    public class DialogManager : MonoBehaviour
    {
        public string CharacterName;
        public string CharacterDialogFilename;
        public string BaseImage;
        public DialogObject dialogObject;

        [SerializeField]
        private GameObject DialogCanvas;
        [SerializeField]
        private TextMeshProUGUI DialogBody;
        [SerializeField]
        private ButtonManager ButtonManager;
        [SerializeField]
        private List<DialogOption> currentDialogOptions = new();
        private DialogOption currentDialogOption = new();
        private int currentDialogPage = 0;

        private void Awake()
        {
            DialogCanvas = GameObject.Find("DialogCanvas");
            DialogBody = GameObject.Find("DialogBodyText").GetComponent<TextMeshProUGUI>();
            ButtonManager = DialogCanvas.GetComponentInChildren<ButtonManager>();
            DialogCanvas.SetActive(false);


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
        }

        public void StartDialog(float vibe)
        {
            List<OptionsByTier> tiers = dialogObject.general.optionsByTier.FindAll((n) =>  n.tier <= vibe);
            tiers.Sort((a, b) => a.tier.CompareTo(b.tier));
            Debug.Log(tiers[0].tier);
            currentDialogOptions = tiers[0].options;
            currentDialogOption = currentDialogOptions.Find((n) => n.key == "greeting");
            DialogCanvas.SetActive(true);
            PauseGameForDialog(true);
            UpdateDialogText(currentDialogOption.body);
        }

        public void UpdateDialogText(string bodyText)
        {
            DialogBody.text = bodyText;
        }

        public void NextPage()
        {
            Debug.Log("next page button clicked");
            if(currentDialogOption.pages.Count > 0)
            {
                currentDialogPage++;
                Debug.Log(currentDialogOption.pages.Count);
                if (currentDialogPage >= currentDialogOption.pages.Count)
                {
                    if(currentDialogOption.pages[currentDialogPage - 1].responses.Count != 0)
                    {
                        ButtonManager.SetDialogButtonOptions(currentDialogOption.pages[currentDialogPage - 1].responses);
                    }
                    currentDialogPage = 0;
                    return;
                } else
                {
                    UpdateDialogText(currentDialogOption.pages[currentDialogPage - 1].body);
                    return;
                }
            }
            else
            {
                Debug.Log("No more pages to display");
            }
            return;
        }

        public void HandleOption(int option)
        {
            Debug.Log("Option " + option + " clicked");
            // Handle the option click
            // You can use the option index to determine which option was clicked
            // For example, you can update the dialog text based on the selected option
            // UpdateDialogText(dialogObject.general.optionsByTier[0].options[option].body);
            // CloseDialog();
        }
    }
}

