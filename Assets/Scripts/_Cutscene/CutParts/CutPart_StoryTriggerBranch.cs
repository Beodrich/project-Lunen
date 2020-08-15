using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CutPart_StoryTriggerBranch : CutPart
{
    //Standard Values
    [SerializeField] public string _name = "";
    public static CutPartType _type = CutPartType.StoryTriggerBranch;
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
    public StoryTrigger trigger;
    public string triggerPart;
    public int equalIndex;
    public int targetIndex;

    public bool checkBool;
    public int checkInt;
    public float checkFloat;
    public double checkDouble;
    public string checkString;

    public Cutscene destinationCutscene;
    public CutsceneScript destinationCutsceneScript;
    public string destinationRoute;
    
    //Functions

    public void PlayPart (SetupRouter sr)
    {
        bool condition = false;
        switch (equalIndex)
        {
            
            case 0: //"Is Equal To"
                condition = trigger.IsTriggerEqual(triggerPart, GetCheckValue());
            break;
            case 1: //"Is Not Equal To"
                condition = !trigger.IsTriggerEqual(triggerPart, GetCheckValue());
            break;
            case 2: //"Is Less Than"
                condition = trigger.IsTriggerLessThan(triggerPart, GetCheckValue());
            break;
            case 3: //"Is Less Than Or Equal To"
                condition = trigger.IsTriggerLessThan(triggerPart, GetCheckValue(), true);
            break;
            case 4: //"Is Greater Than"
                condition = trigger.IsTriggerGreaterThan(triggerPart, GetCheckValue());
            break;
            case 5: //"Is Greater Than Or Equal To"
                condition = trigger.IsTriggerGreaterThan(triggerPart, GetCheckValue(), true);
            break;
        }

        if (condition)
        {
            if (destinationCutscene != null)
            {
                sr.battleSetup.CutsceneStartLite(new PackedCutscene(destinationCutscene), destinationRoute);
            }
            else
            {
                sr.battleSetup.CutsceneStartLite(sr.battleSetup.lastCutscene, destinationRoute);
            }
        }
        else
        {
            sr.battleSetup.AdvanceCutscene();
        }
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
        CutPart_StoryTriggerBranch _cp = (CutPart_StoryTriggerBranch)cp;

        _title = _cp._title;
        _startNextSimultaneous = _cp.startNextSimultaneous;

        trigger = _cp.trigger;
        triggerPart = _cp.triggerPart;
        equalIndex = _cp.equalIndex;
        targetIndex = _cp.targetIndex;
        checkBool = _cp.checkBool;
        checkInt = _cp.checkInt;
        checkFloat = _cp.checkFloat;
        checkDouble = _cp.checkDouble;
        checkString = _cp.checkString;
        destinationCutscene = _cp.destinationCutscene;
        destinationCutsceneScript = _cp.destinationCutsceneScript;
        destinationRoute = _cp.destinationRoute;
    }

    public void GetTitle()
    {
        string start = "[" + _type + "]";
        string context = _title;
        if (context == "")
        {
            string[] equalList = {"==", "!=", "<", "<=", ">", ">="};
            if (trigger != null)
            {
                start = "[STB]";
                context += "if (" + trigger.name;
                if (triggerPart != "")
                {
                    context += "/" + triggerPart + ") " + equalList[equalIndex] + " " + GetCheckValue().ToString();
                }
            }
            else
            {
                context += "No Story Trigger!";
            }
            
        }
        _name = (start + " " + context);
    }

    #if UNITY_EDITOR
        public void DrawInspectorPart(SerializedProperty serializedProperty, Cutscene cutscene = null, CutsceneScript cutsceneScript = null)
        {
            EditorGUILayout.LabelField("If...", EditorStyles.boldLabel);
            trigger = (StoryTrigger)EditorGUILayout.ObjectField("Story Trigger: ", trigger, typeof(StoryTrigger), false);
            if (trigger != null)
            {
                string[] equalList = {"Is Equal To", "Is Not Equal To", "Is Less Than", "Is Less Than Or Equal To", "Is Greater Than", "Is Greater Than Or Equal To"};
                string[] targetList = {"This Cutscene", "Other Cutscene", "Other CutsceneScript"};
                EditorGUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                int part = trigger.GetTriggerPartIndex(triggerPart);
                if (part == -1) part = 0;
                string lastTriggerPart = triggerPart;
                triggerPart = trigger.triggerParts[EditorGUILayout.Popup(part, trigger.GetTriggerPartList())].title;
                if (lastTriggerPart != triggerPart)
                {
                    part = trigger.GetTriggerPartIndex(triggerPart);
                }
                equalIndex = EditorGUILayout.Popup(equalIndex, equalList);
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
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField("Then Go To...", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                targetIndex = EditorGUILayout.Popup(targetIndex, targetList);
                switch (targetIndex)
                {
                    case 0: //This Cutscene
                        if (cutscene != null)
                        {
                            int destination = cutscene.GetRouteIndex(destinationRoute);
                            if (destination == -1) destination = 0;
                            destinationRoute = cutscene.GetRouteString(EditorGUILayout.Popup(destination, cutscene.GetListOfRoutes()));
                        }
                        if (cutsceneScript != null)
                        {
                            int destination = cutsceneScript.GetRouteIndex(destinationRoute);
                            if (destination == -1) destination = 0;
                            destinationRoute = cutsceneScript.GetRouteString(EditorGUILayout.Popup(destination, cutsceneScript.GetListOfRoutes()));
                        }
                    break;
                    case 1: //Cutscene
                        destinationCutscene = (Cutscene)EditorGUILayout.ObjectField(destinationCutscene, typeof(Cutscene), true);
                        if (destinationCutscene != null)
                        {
                            int destination = destinationCutscene.GetRouteIndex(destinationRoute);
                            if (destination == -1) destination = 0;
                            destinationRoute = destinationCutscene.GetRouteString(EditorGUILayout.Popup(destination, destinationCutscene.GetListOfRoutes()));
                        }
                    break;
                    case 2: //CutsceneScript
                        destinationCutsceneScript = (CutsceneScript)EditorGUILayout.ObjectField(destinationCutsceneScript, typeof(CutsceneScript), true);
                        if (destinationCutsceneScript != null)
                        {
                            int destination = destinationCutsceneScript.GetRouteIndex(destinationRoute);
                            if (destination == -1) destination = 0;
                            destinationRoute = destinationCutsceneScript.GetRouteString(EditorGUILayout.Popup(destination, destinationCutsceneScript.GetListOfRoutes()));
                        }
                    break;

                }
                
                
                EditorGUILayout.EndHorizontal();
            }
        }
    #endif
}
