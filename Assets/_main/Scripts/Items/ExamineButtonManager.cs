using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace SpriteGame
{
    public class ExamineButtonManager : MonoBehaviour
    {
        private ItemInteractManager dialogManager;
        private Button PickUpButton, ExamineButton, DropItemButton, EndExamineButton;
        private GameObject ResponseButtons;

        public void Awake()
        {
            ResponseButtons = GameObject.Find("ResponseButtons");

            PickUpButton = GameObject.Find("PickUpButton").GetComponent<Button>();
            PickUpButton.onClick.AddListener(() => OnButtonClick("PickUpButton"));

            ExamineButton = GameObject.Find("ExamineButton").GetComponent<Button>();
            ExamineButton.onClick.AddListener(() => OnButtonClick("ExamineButton"));

            DropItemButton = GameObject.Find("DropItemButton").GetComponent<Button>();
            DropItemButton.onClick.AddListener(() => OnButtonClick("DropItemButton"));

            EndExamineButton = GameObject.Find("EndExamineButton").GetComponent<Button>();
            EndExamineButton.onClick.AddListener(() => OnButtonClick("EndExamineButton"));
            ResponseButtons.SetActive(false);
            // Find the ButtonManager in the scene and set it
            //ResponseButtons = GameObject.Find("ResponseButtons").GetComponentsInChildren<>;
        }

        public void Initiate(ItemInteractManager n)
        {
            this.dialogManager = n;
            ResponseButtons.SetActive(true);
            ButtonEnabler(true);
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
                case "PickUpButton":
                    // Handle option one button click
                    Debug.Log("Option one button clicked");
                    dialogManager.HandleOption(0);
                    break;
                case "ExamineButton":
                    // Handle option two button click
                    Debug.Log("Option two button clicked");
                    dialogManager.HandleOption(1);
                    ButtonEnabler(false); // Disable buttons after examining
                    break;
                case "DropButton":
                    // Handle option three button click
                    Debug.Log("Option three button clicked");
                    dialogManager.HandleOption(2);
                    break;
                case "EndExamineButton":
                    // Handle option four button click
                    Debug.Log("Option four button clicked");
                    dialogManager.HandleOption(3);
                    break;
            }
        }

        private void ButtonEnabler(bool InitialState)
        {
            if(InitialState)
            {
                PickUpButton.gameObject.SetActive(true);
                ExamineButton.gameObject.SetActive(true);
                DropItemButton.gameObject.SetActive(true);
                EndExamineButton.gameObject.SetActive(false);
            }
            else
            {
                PickUpButton.gameObject.SetActive(false);
                ExamineButton.gameObject.SetActive(false);
                DropItemButton.gameObject.SetActive(false);
                EndExamineButton.gameObject.SetActive(true);
            }
        }
    }
}
