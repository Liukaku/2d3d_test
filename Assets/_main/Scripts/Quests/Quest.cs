using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteGame
{
    public enum QuestState
    {
        NOT_STARTED,
        IN_PROGRESS,
        COMPLETE,
    }

    public enum QuestStepType
    {
        HUNT,
        TALK,
        COLLECT,
    }

    [System.Serializable]
    public class Quest
    {
        public string uid;
        public string title;
        public string description;
        public int currentStep;
        public QuestState state;
        public QuestStep[] steps;
    }

    [System.Serializable]
    public class QuestStep
    {
        public QuestState state;
        public int stepCount; // which step this is in the quest
        public string targetUid; // UID of the target object (NPC, item, etc.)
        public int targetCount; // how many of the target object is needed to complete this step
        public string description;

        public QuestStep(QuestStepType type, string targetUid, int targetCount, string description, int stepCount)
        {
            this.targetUid = targetUid;
            this.targetCount = targetCount;
            this.description = description;
            this.stepCount = stepCount;
        }
    }
}
