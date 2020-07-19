using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CutPart_Wait : CutPart
{
    //Standard Values
    [SerializeField] public string _name = "";
    public static CutPartType _type = CutPartType.Wait;
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

    public float waitTime;
    public bool useStoryTriggerTime;
    public StoryTrigger trigger;
    public string triggerPart;

    //Functions

    public void PlayPart (SetupRouter sr)
    {
        if (useStoryTriggerTime)
        {
            sr.battleSetup.waitTime = (float)trigger.GetTriggerValue(triggerPart);
        }
        else
        {
            sr.battleSetup.waitTime = waitTime;
        }
        
        sr.battleSetup.StartCoroutine(sr.battleSetup.cutsceneWait(sr.battleSetup.transform));
    }

    public void Duplicate (CutPart cp)
    {
        CutPart_Wait _cp = (CutPart_Wait)cp;

        _title = _cp._title;
        _startNextSimultaneous = _cp.startNextSimultaneous;

        waitTime = _cp.waitTime;
        useStoryTriggerTime = _cp.useStoryTriggerTime;
        trigger = _cp.trigger;
        triggerPart = _cp.triggerPart;
    }

    public void GetTitle()
    {
        string start = "[" + _type + "]";
        string context = _title;
        if (_title == "")
        {
            context = "Wait ";
            if (useStoryTriggerTime)
            {
                int partIndex = trigger.GetTriggerPartIndex(triggerPart);
                if (trigger.triggerParts[partIndex].type != TriggerTypes.Int && trigger.triggerParts[partIndex].type != TriggerTypes.Float) context += "ERROR! VALUE NOT INT OR FLOAT!";
                else context += trigger.GetTriggerValue(triggerPart).ToString() + "s (" + trigger.triggerParts[partIndex].title + ")";
            }
            else
            {
                context += waitTime + "s";
            }
        }
        _name = (start + " " + context);
    }

    #if UNITY_EDITOR
        public void DrawInspectorPart(Cutscene cutscene = null, CutsceneScript cutsceneScript = null)
        {
            useStoryTriggerTime = EditorGUILayout.Toggle("Use Trigger Time: ", useStoryTriggerTime);
            if (useStoryTriggerTime)
            {
                EditorGUILayout.BeginHorizontal();
                trigger = (StoryTrigger)EditorGUILayout.ObjectField(trigger, typeof(StoryTrigger), false);
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
                }

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                waitTime = EditorGUILayout.FloatField("Wait Time (In Seconds): ", waitTime);
            }
            
        }
    #endif
}
