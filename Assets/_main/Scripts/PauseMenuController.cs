using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteGame
{
    public class PauseMenuController : MonoBehaviour
    {
        public bool IsPaused = false;
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
            HandleGameSpeed();
        }

        private void ResumeGame()
        {
            IsPaused = false;
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
