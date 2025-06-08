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
        public bool progressQuest = false;
        public List<DialogPage> pages;
        public List<DialogResponse> responses;
    }

    [System.Serializable]
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
        public List<StoryQuests> quests;
    }

    [System.Serializable]
    public class StoryQuests
    {
        public string questUid;
        public int questStepIndex;
        public List<DialogOption> options;
    }
}
