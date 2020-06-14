using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class Cutscene : MonoBehaviour
{
    public enum PartType
    {
        Null,
        Dialogue,
        Movement,
        Animation,
        Trigger
    }
    public enum MoveType
    {
        Null,
        ToCollider,
        ToSpaces
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
        public bool startNextSimultaneous;
        public PartType type;

        //Type: Movement
        [ConditionalField(nameof(type), false, PartType.Movement)] public Move moveScript;
        [ConditionalField(nameof(type), false, PartType.Movement)] public bool moveInFacingDirection;
        [ConditionalField(nameof(moveInFacingDirection), true)] public MoveScripts.Direction movementDirection;
        [ConditionalField(nameof(type), false, PartType.Movement)] public MoveType moveType;
        [ConditionalField(nameof(moveType), false, MoveType.ToCollider)] public string colliderTag;
        [ConditionalField(nameof(moveType), false, MoveType.ToSpaces)] public int spacesToMove;

        //Type: Movement
        [ConditionalField(nameof(type), false, PartType.Trigger)] public TriggerType triggerType;
        [ConditionalField(nameof(triggerType), false, TriggerType.Battle)] public TrainerLogic trainerLogic;

        //Type: Movement
        [ConditionalField(nameof(type), false, PartType.Dialogue)] [TextArea(3,10)] public string text;

        [HideInInspector] public TrainerLogic trainerEncounter;
    }

    //[Reorderable(paginate = true, pageSize = 0)]
    [System.Serializable] public class ReorderableParts : Reorderable<Part> {}
    public ReorderableParts parts;
}
