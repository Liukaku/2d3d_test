using System.Collections;
using System.Collections.Generic;
using SpriteGame;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemInteractManager : MonoBehaviour
{
    public Interactable InteractableItem;
    [SerializeField]
    private GameObject DialogCanvas;
    [SerializeField]
    private TextMeshProUGUI DialogName;
    [SerializeField]                   
    private TextMeshProUGUI DialogBody;
    [SerializeField]
    private ExamineButtonManager ButtonManager;

    private Image spriteRenderer;
    [SerializeField]
    private Dictionary<string, Sprite> sprites;
    // Start is called before the first frame update
    void Awake()
    {
        InteractableItem = transform.GetComponent<Interactable>();
        DialogCanvas = GameObject.Find("ItemExamineCanvas");
        DialogName = GameObject.Find("ItemExamineName").GetComponent<TextMeshProUGUI>();
        DialogBody = GameObject.Find("ItemExamineBody").GetComponent<TextMeshProUGUI>();
        ButtonManager = DialogCanvas.GetComponentInChildren<ExamineButtonManager>();

        DialogCanvas.SetActive(false); // Hide the dialog canvas initially
    }

    public void InitiateInteract()
    {
        if (InteractableItem == null)
        {
            Debug.LogError("InteractableItem is not set. Please assign an Interactable component to this GameObject.");
            return;
        } else
        {
            // enable the dialog canvas and set the item name in the dialog body
            DialogName.text = InteractableItem.Name;
            DialogBody.text = "";
            DialogCanvas.SetActive(true);
            ButtonManager.Initiate(this);
            InteractableItem.IsInteracting = true;
            PauseGameForDialog(true);
        }
    }
    public void PickUp()
    {
        InteractableItem.PickUp();
        EndInteract();
    }

    public void Examine()
    {
        ButtonManager.DisableButtons();
        DialogBody.text = InteractableItem.ExamineText;

    }

    public void DropItem()
    {
        EndInteract();
    }

    public void EndExamine()
    {
        DialogBody.text = "";
        ButtonManager.Initiate(this);
    }

    private void EndInteract()
    {
        DialogCanvas.SetActive(false); // Hide the dialog canvas after picking up the item
        PauseGameForDialog(false); // Resume the game after picking up the item
        InteractableItem.IsInteracting = false; // Reset interaction state
    }

    public void HandleOption(int button)
    {
        switch (button)
        {
            case 0: // Pick Up
                PickUp();
                break;
            case 1: // Examine
                Examine();
                break;
            case 2: // Drop Item
                DropItem();
                break;
            case 3: // End Examine
                EndExamine();
                break;
        }
    }

    private void PauseGameForDialog(bool pausing)
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

}
