using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteGame
{
    [System.Serializable]
    public class DialogObject
    {
        public General general;
        public Story story;
    }

    [System.Serializable]
    public class General
    {
        public List<OptionsByTier> optionsByTier;
    }

    [System.Serializable]
    public class OptionsByTier
    {
        public int tier;
        public List<DialogOption> options;
    }

    [System.Serializable]
    public class DialogOption
    {
        public string key;
        public string image;
        public string body;
        public List<DialogPage> pages;
        public List<DialogResponse> responses;
    }

    public class DialogPage
    {
        public string key;
        public string image;
        public string body;
        public List<DialogResponse> responses;
    }

    [System.Serializable]
    public class DialogResponse
    {
        public string body;
        public string next;
        public int vibe;
    }

    [System.Serializable]
    public class Story
    {
        // Add fields as needed for story-specific dialog
    }
}
