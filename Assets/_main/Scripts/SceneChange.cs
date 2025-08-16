using System.Collections;
using System.Collections.Generic;
using SpriteGame;
using UnityEngine;

public class SceneChange : MonoBehaviour
{
    [SerializeField]
    private string sceneToLoad;

    [SerializeField]
    private Vector3 teleportLocation;

    private PlayerController playerController;

    public bool isSceneTrigger = true;

    private void Start()
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("Player not found in the scene. Please ensure the player has the 'Player' tag.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isSceneTrigger)
            {
                // Load the new scene here
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
            } else
            {

                //other.transform.position = teleportLocation;
                playerController.TeleportPlayer(teleportLocation);
            }
        }
    }
}