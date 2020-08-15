using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CutPart_StoryTriggerSet : CutPart
{
    //Standard Values
    [SerializeField] public string _name = "";
    public static CutPartType _type = CutPartType.StoryTriggerSet;
    public string _title = ("");
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
    public StoryTrigger trigger;
    public string triggerPart;

    public bool checkBool;
    public int checkInt;
    public float checkFloat;
    public double checkDouble;
    public string checkString;
    
    //Functions

    public void PlayPart (SetupRouter sr)
    {
        trigger.SetTriggerValue(triggerPart, GetCheckValue());
        sr.battleSetup.AdvanceCutscene();
    }

    public object GetCheckValue()
    {
        int part = trigger.GetTriggerPartIndex(triggerPart);
        switch(trigger.triggerParts[part].type)
        {
            case TriggerTypes.Bool: return checkBool;
            case TriggerTypes.Int: return checkInt;
            case TriggerTypes.Float: return checkFloat;
            case TriggerTypes.Double: return checkDouble;
            case TriggerTypes.String: return checkString;
        }
        return null;
    }

    public void Duplicate (CutPart cp)
    {
        CutPart_StoryTriggerSet _cp = (CutPart_StoryTriggerSet)cp;

        _title = _cp._title;
        _startNextSimultaneous = _cp.startNextSimultaneous;

        trigger = _cp.trigger;
        triggerPart = _cp.triggerPart;

        checkBool = _cp.checkBool;
        checkInt = _cp.checkInt;
        checkFloat = _cp.checkFloat;
        checkDouble = _cp.checkDouble;
        checkString = _cp.checkString;
    }

    public void GetTitle()
    {
        string start = "[" + _type + "] ";
        string context = _title;
        if (_title == "")
        {
            if (trigger != null)
            {
                start = "[STS] ";
                context += "Set (" + trigger.name;
                if (triggerPart != "")
                {
                    context += "/" + triggerPart + ") To " + GetCheckValue().ToString();
                }
            }
            else
            {
                context += "No Story Trigger!";
            }
        }
        _name = (start + context);
    }

    #if UNITY_EDITOR
        public void DrawInspectorPart(SerializedProperty serializedProperty, Cutscene cutscene = null, CutsceneScript cutsceneScript = null)
        {
            trigger = (StoryTrigger)EditorGUILayout.ObjectField("Story Trigger: ", trigger, typeof(StoryTrigger), false);
            if (trigger != null)
            {
                int part = trigger.GetTriggerPartIndex(triggerPart);
                if (part == -1) part = 0;
                string lastTriggerPart = triggerPart;
                triggerPart = trigger.triggerParts[EditorGUILayout.Popup(part, trigger.GetTriggerPartList())].title;
                if (lastTriggerPart != triggerPart)
                {
                    part = trigger.GetTriggerPartIndex(triggerPart);
                }
                switch(trigger.triggerParts[part].type)
                {
                    case TriggerTypes.Bool:
                        checkBool = EditorGUILayout.Toggle(checkBool.ToString(), (bool)checkBool);
                        break;
                    case TriggerTypes.Int:
                        checkInt = EditorGUILayout.IntField((int)checkInt);
                        break;
                    case TriggerTypes.Float:
                        checkFloat = EditorGUILayout.FloatField((float)checkFloat);
                        break;
                    case TriggerTypes.Double:
                        checkDouble = EditorGUILayout.DoubleField((double)checkDouble);
                        break;
                    case TriggerTypes.String:
                        checkString = EditorGUILayout.TextField(checkString);
                        break;
                }
                
            }
        }
    #endif
}
