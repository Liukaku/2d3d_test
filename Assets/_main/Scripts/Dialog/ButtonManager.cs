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
        private ButtonInfo buttonInfoOne, buttonInfoTwo, buttonInfoThree, buttonInfoFour;
        private GameObject ResponseButtons;
        private string[] buttonOptions = { "OptionOne", "OptionTwo", "OptionThree", "OptionFour" };

        public void Awake()
        {
            ResponseButtons = GameObject.Find("ResponseButtons");

            ResponseButtonOne = GameObject.Find("OptionOne").GetComponent<Button>();
            ResponseButtonOne.onClick.AddListener(() => OnButtonClick("OptionOne"));
            buttonInfoOne = ResponseButtonOne.GetComponent<ButtonInfo>();

            ResponseButtonTwo = GameObject.Find("OptionTwo").GetComponent<Button>();
            ResponseButtonTwo.onClick.AddListener(() => OnButtonClick("OptionTwo"));
            buttonInfoTwo = ResponseButtonTwo.GetComponent<ButtonInfo>();

            ResponseButtonThree = GameObject.Find("OptionThree").GetComponent<Button>();
            ResponseButtonThree.onClick.AddListener(() => OnButtonClick("OptionThree"));
            buttonInfoThree = ResponseButtonThree.GetComponent<ButtonInfo>();

            ResponseButtonFour = GameObject.Find("OptionFour").GetComponent<Button>();
            ResponseButtonFour.onClick.AddListener(() => OnButtonClick("OptionFour"));
            buttonInfoFour = ResponseButtonFour.GetComponent<ButtonInfo>();

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
                    dialogManager.HandleOption(0, buttonInfoOne.dialogResponse);
                    break;
                case "OptionTwo":
                    // Handle option two button click
                    Debug.Log("Option two button clicked");
                    dialogManager.HandleOption(1, buttonInfoTwo.dialogResponse);
                    break;
                case "OptionThree":
                    // Handle option three button click
                    Debug.Log("Option three button clicked");
                    dialogManager.HandleOption(2, buttonInfoThree.dialogResponse);
                    break;
                case "OptionFour":
                    // Handle option four button click
                    Debug.Log("Option four button clicked");
                    dialogManager.HandleOption(3, buttonInfoFour.dialogResponse);
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
                    ResponseButtonOne.gameObject.SetActive(true);
                    buttonInfoOne.SetButtonContent(dia);
                    break;
                case 1:
                    ResponseButtonTwo.gameObject.SetActive(true);
                    buttonInfoTwo.SetButtonContent(dia);
                    break;
                case 2:
                    ResponseButtonThree.gameObject.SetActive(true);
                    buttonInfoThree.SetButtonContent(dia);
                    break;
                case 3:
                    ResponseButtonFour.gameObject.SetActive(true);
                    buttonInfoFour.SetButtonContent(dia);
                    break;
            }
        }

        private void DisableAllButtons()
        {
            ResponseButtonOne.gameObject.SetActive(false);
            ResponseButtonTwo.gameObject.SetActive(false);
            ResponseButtonThree.gameObject.SetActive(false);
            ResponseButtonFour.gameObject.SetActive(false);
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
