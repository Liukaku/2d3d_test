using System.Collections;
using System.Collections.Generic;
using SpriteGame;
using TMPro;
using UnityEngine;

public class ButtonInfo : MonoBehaviour
{
    private TextMeshProUGUI buttonText;
    public DialogResponse dialogResponse;

    private void Awake()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText == null)
        {
            Debug.LogError("ButtonInfo: TextMeshProUGUI component not found in children.");
        }
    }
    public void SetButtonContent(DialogResponse content)
    {     // Set the button text to the response body
        dialogResponse = content;
        if (buttonText != null)
        {
            buttonText.text = content.body;
        }

        // Set the button's next dialog key or other properties as needed
        // For example, you might want to store the next dialog key in a variable
        // or set it as a tag or name for later use.
    }
}
