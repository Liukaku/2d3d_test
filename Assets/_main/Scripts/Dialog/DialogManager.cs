using System;
using System.Collections.Generic;
using System.IO;
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

        private void Awake()
        {
            // load in the dialog json file from Assets/_main/Scripts
            // and assign it to the dialogObject variable
            string path = "Assets/_main/Scripts/DBs/Dialog/" + CharacterDialogFilename + ".json";
            using StreamReader r = new(path);
            string jsonFile = r.ReadToEnd();
            DialogObject obj = new();
            if (jsonFile != null)
            {
                Debug.Log(jsonFile);
                var test = JsonUtility.FromJson<DialogObject>(jsonFile);
                Debug.Log(test.general.optionsByTier.tier);
                Debug.Log(test.general.optionsByTier.options);
                dialogObject = test;

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
    }
}

