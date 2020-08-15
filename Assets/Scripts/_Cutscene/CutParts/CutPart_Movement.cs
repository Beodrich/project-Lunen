using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CutPart_Movement : CutPart
{
    //Standard Values
    [SerializeField] public string _name = "";
    public static CutPartType _type = CutPartType.Movement;
    public string _title = ("New " + _type.ToString());
    public bool _startNextSimultaneous;

    public bool startNextSimultaneous
    {
        get => _startNextSimultaneous;
        set => _startNextSimultaneous = value;
    }

    public string listDisplay
    {
        get => _name;
    }

    public string partTitle
    {
        get => _title;
        set => _title = value;
    }

    public CutPartType cutPartType
    {
        get => _type;
    }

    //Unique Values

    public Move moveScript;
    public bool chooseMoveDirection;
    public MoveScripts.Direction movementDirection;
    public MoveType moveType;
    public string colliderTag;
    public int spacesToMove;
    public bool movePlayer;

    //Functions

    public void PlayPart (SetupRouter sr)
    {
        if (movePlayer)
        {
            sr.playerLogic.move.StartCutsceneMove(this);
        }
        else
        {
            moveScript.StartCutsceneMove(this);
        }
    }

    public void Duplicate (CutPart cp)
    {
        CutPart_Movement _cp = (CutPart_Movement)cp;

        _title = _cp._title;
        _startNextSimultaneous = _cp.startNextSimultaneous;

        moveScript = _cp.moveScript;
        chooseMoveDirection = _cp.chooseMoveDirection;
        movementDirection = _cp.movementDirection;
        moveType = _cp.moveType;
        colliderTag = _cp.colliderTag;
        spacesToMove = _cp.spacesToMove;
        movePlayer = _cp.movePlayer;
    }

    public void GetTitle()
    {
        string start = "[" + _type + "]";
        string context = _title;
        _name = (start + " " + context);
    }

    #if UNITY_EDITOR
        public void DrawInspectorPart(SerializedProperty serializedProperty, Cutscene cutscene = null, CutsceneScript cutsceneScript = null)
        {
            movePlayer = EditorGUILayout.Toggle("Move Player? ", movePlayer);
            if (!movePlayer)
            {
                moveScript = (Move)EditorGUILayout.ObjectField("Move Script: ", moveScript, typeof(Move), true);
            }

            GUILayout.BeginHorizontal();
            chooseMoveDirection = EditorGUILayout.Toggle("Change Direction", chooseMoveDirection);
            if (chooseMoveDirection)
            {
                movementDirection = (MoveScripts.Direction)EditorGUILayout.EnumPopup(movementDirection);
            }
            GUILayout.EndHorizontal();

            
            EditorGUIUtility.fieldWidth = 120;
            GUILayout.BeginHorizontal();
            moveType = (MoveType)EditorGUILayout.EnumPopup("Movement Type: ", moveType);
            if (moveType == (MoveType.ToColliderTag))
            {
                colliderTag = EditorGUILayout.TextField(colliderTag);
            }
            else if (moveType == (MoveType.ToSpaces))
            {
                spacesToMove = EditorGUILayout.IntField(spacesToMove);
            }
            GUILayout.EndHorizontal();
            EditorGUIUtility.fieldWidth = 0;

            startNextSimultaneous = EditorGUILayout.Toggle("Start Next Part Alongside: ", startNextSimultaneous);
        }
    #endif
}
