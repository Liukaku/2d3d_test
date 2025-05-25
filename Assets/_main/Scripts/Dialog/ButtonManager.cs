using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace SpriteGame
{
    public class ButtonManager : MonoBehaviour
    {
        private DialogManager dialogManager;
        private Button ResponseButtonOne, ResponseButtonTwo, ResponseButtonThree, ResponseButtonFour, NextPageButton;
        private Button[] buttons = new Button[4];
        private ButtonInfo buttonInfoOne, buttonInfoTwo, buttonInfoThree, buttonInfoFour;
        private ButtonInfo[] buttonInfos = new ButtonInfo[4];
        private GameObject ResponseButtons;
        private string[] buttonOptions = { "OptionOne", "OptionTwo", "OptionThree", "OptionFour" };

        public void Awake()
        {
            ResponseButtons = GameObject.Find("ResponseButtons");
            for(int i = 0; i < buttonOptions.Length; i++)
            {
                try
                {
                    buttons[i] = GameObject.Find(buttonOptions[i]).GetComponent<Button>();
                    buttons[i].onClick.AddListener(() => OnButtonClick(buttonOptions[i]));
                    buttonInfos[i] = buttons[i].GetComponent<ButtonInfo>();
                }
                catch (System.Exception e)
                {
                    Debug.LogError("ButtonManager: Error finding button " + buttonOptions[i] + ": " + e.Message);
                }

            }

            NextPageButton = GameObject.Find("NextPageButton").GetComponent<Button>();
            NextPageButton.onClick.AddListener(() => OnButtonClick("NextPageButton"));
            ResponseButtons.SetActive(false);
            // Find the ButtonManager in the scene and set it
            //ResponseButtons = GameObject.Find("ResponseButtons").GetComponentsInChildren<>;
        }

        public void DisableButtons()
        {
            ResponseButtons.SetActive(false);
        }

        public void OnButtonClick(string buttonLabel)
        {
            Debug.Log("Button clicked: " + buttonLabel);
            // buttons: "NextPageButton", "OptionOne", "OptionTwo", "OptionThree", "OptionFour"
            switch (buttonLabel)
            {
                case "NextPageButton":
                    // Handle the next page button click
                    Debug.Log("Next page button clicked");
                    dialogManager.NextPage();
                    break;
                case "OptionOne":
                    // Handle option one button click
                    Debug.Log("Option one button clicked");
                    dialogManager.HandleOption(0, buttonInfos[0].dialogResponse);
                    break;
                case "OptionTwo":
                    // Handle option two button click
                    Debug.Log("Option two button clicked");
                    dialogManager.HandleOption(1, buttonInfos[1].dialogResponse);
                    break;
                case "OptionThree":
                    // Handle option three button click
                    Debug.Log("Option three button clicked");
                    dialogManager.HandleOption(2, buttonInfos[2].dialogResponse);
                    break;
                case "OptionFour":
                    // Handle option four button click
                    Debug.Log("Option four button clicked");
                    dialogManager.HandleOption(3, buttonInfos[3].dialogResponse);
                    break;
            }
        }

        public void SetDialogManager(DialogManager dialogManager)
        {
            this.dialogManager = dialogManager;
        }

        private void ButtonEnabler(int optionIndex, DialogResponse dia)
        {
            Debug.Log(dia.body);
            switch (optionIndex)
            {
                case 0:
                    buttons[0].gameObject.SetActive(true);
                    buttonInfos[0].SetButtonContent(dia);
                    break;
                case 1:
                    buttons[1].gameObject.SetActive(true);
                    buttonInfos[1].SetButtonContent(dia);
                    break;
                case 2:
                    buttons[2].gameObject.SetActive(true);
                    buttonInfos[2].SetButtonContent(dia);
                    break;
                case 3:
                    buttons[3].gameObject.SetActive(true);
                    buttonInfos[3].SetButtonContent(dia);
                    break;
            }
        }

        private void DisableAllButtons()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].gameObject.SetActive(false);
            }
        }

        public void SetDialogButtonOptions(List<DialogResponse> responses)
        {
            Debug.Log(responses);
            DisableAllButtons();
            dialogManager.ClearDialogBody();
            for (int i = 0; i < responses.Count; i++)
            {
                Debug.Log("Response " + i + ": " + responses[i].body);
                ButtonEnabler(i, responses[i]);
            }
            ResponseButtons.SetActive(true);

        }
    }
}
