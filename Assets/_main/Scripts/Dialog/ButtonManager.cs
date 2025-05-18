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
        private GameObject ResponseButtons;

        public void Awake()
        {
            ResponseButtons = GameObject.Find("ResponseButtons");
            ResponseButtonOne = GameObject.Find("OptionOne").GetComponent<Button>();
            ResponseButtonOne.onClick.AddListener(() => OnButtonClick("OptionOne"));
            ResponseButtonTwo = GameObject.Find("OptionTwo").GetComponent<Button>();
            ResponseButtonTwo.onClick.AddListener(() => OnButtonClick("OptionTwo"));
            ResponseButtonThree = GameObject.Find("OptionThree").GetComponent<Button>();
            ResponseButtonThree.onClick.AddListener(() => OnButtonClick("OptionThree"));
            ResponseButtonFour = GameObject.Find("OptionFour").GetComponent<Button>();
            ResponseButtonFour.onClick.AddListener(() => OnButtonClick("OptionFour"));
            NextPageButton = GameObject.Find("NextPageButton").GetComponent<Button>();
            NextPageButton.onClick.AddListener(() => OnButtonClick("NextPageButton"));
            ResponseButtons.SetActive(false);
            // Find the ButtonManager in the scene and set it
            //ResponseButtons = GameObject.Find("ResponseButtons").GetComponentsInChildren<>;
        }

        public void OnButtonClick(string buttonLabel)
        {
            // Get the name of the button that was clicked
            string buttonName = gameObject.name;
            Debug.Log("Button clicked: " + buttonLabel);
            // buttons: "NextPageButton", "OptionOne", "OptionTwo", "OptionThree", "OptionFour"
            switch (buttonName)
            {
                case "NextPageButton":
                    // Handle the next page button click
                    Debug.Log("Next page button clicked");
                    dialogManager.NextPage();
                    break;
                case "OptionOne":
                    // Handle option one button click
                    Debug.Log("Option one button clicked");
                    dialogManager.HandleOption(0);
                    break;
                case "OptionTwo":
                    // Handle option two button click
                    Debug.Log("Option two button clicked");
                    dialogManager.HandleOption(1);
                    break;
                case "OptionThree":
                    // Handle option three button click
                    Debug.Log("Option three button clicked");
                    dialogManager.HandleOption(2);
                    break;
                case "OptionFour":
                    // Handle option four button click
                    Debug.Log("Option four button clicked");
                    dialogManager.HandleOption(3);
                    break;
            }
        }

        private void SetDialogManager(DialogManager dialogManager)
        {
            this.dialogManager = dialogManager;
        }

        public void SetDialogButtonOptions(List<DialogResponse> responses)
        {
            Debug.Log(responses);
        }
    }
}
