using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using Malee.List;

public class Cutscene : MonoBehaviour
{
    public enum PartType
    {
        Null,
        Dialogue,
        Movement,
        Animation,
        Battle,
        Choice,
        Wait,
        HealParty,
        SetSpawn
    }
    public enum MoveType
    {
        Null,
        ToCollider,
        ToSpaces,
        ToAnyCollider
    }
    public enum TriggerType
    {
        Null,
        Battle
    }
    
    [System.Serializable]
    public class Part
    {
        public string name;
        
        public PartType type;

        //Simultaneous Start
        [ConditionalField(nameof(type), false, PartType.Movement)] public bool startNextSimultaneous;

        //Type: Movement
        [ConditionalField(nameof(type), false, PartType.Movement)] public Move moveScript;
        [ConditionalField(nameof(type), false, PartType.Movement)] public bool chooseMoveDirection;
        [ConditionalField(nameof(chooseMoveDirection))] public MoveScripts.Direction movementDirection;
        [ConditionalField(nameof(type), false, PartType.Movement)] public MoveType moveType;
        [ConditionalField(nameof(moveType), false, MoveType.ToCollider)] public string colliderTag;
        [ConditionalField(nameof(moveType), false, MoveType.ToSpaces)] public int spacesToMove;

        //Type: Battle
        [ConditionalField(nameof(type), false, PartType.Battle)] public TrainerLogic trainerLogic;

        //Type: Dialogue
        [ConditionalField(nameof(type), false, PartType.Dialogue, PartType.Choice)] [TextArea(5,10)] public string text;

        //Type: Choice
        [ConditionalField(nameof(type), false, PartType.Choice)] public bool useChoice1;
        [ConditionalField(nameof(useChoice1))] public string choice1Text = "Choice 1";
        [ConditionalField(nameof(useChoice1))] public int choice1Route;

        [ConditionalField(nameof(type), false, PartType.Choice)] public bool useChoice2;
        [ConditionalField(nameof(useChoice2))] public string choice2Text = "Choice 2";
        [ConditionalField(nameof(useChoice2))] public int choice2Route;

        [ConditionalField(nameof(type), false, PartType.Choice)] public bool useChoice3;
        [ConditionalField(nameof(useChoice3))] public string choice3Text = "Choice 3";
        [ConditionalField(nameof(useChoice3))] public int choice3Route;

        //Type: Wait
        [ConditionalField(nameof(type), false, PartType.Wait)] public float waitTime;

        [HideInInspector] public TrainerLogic trainerEncounter;
    }

    [System.Serializable]
    public class Route
    {
        [Reorderable(paginate = true, pageSize = 0, elementNameProperty = "myString")]
        public MyList parts;

        [System.Serializable]
        public class MyList : ReorderableArray<Part> {
        }
    }

    public string cutsceneName;
    public List<Route> routes;
}
