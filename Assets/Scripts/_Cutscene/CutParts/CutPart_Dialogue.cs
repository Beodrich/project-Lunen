using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CutPart_Dialogue : CutPart
{
    //Standard Values
    [SerializeField] public string _name = "";
    public static CutPartType _type = CutPartType.Dialogue;
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

    public string text;
    public bool autoClose;

    //From Wait

    public float waitTime;
    public bool useStoryTriggerTime;
    public StoryTrigger trigger2;
    public string triggerPart2;

    //Temporary Values

    public StoryTrigger trigger;
    public string triggerPart;
    public bool showFoldout;

    //Functions

    public void PlayPart (SetupRouter sr)
    {
        bool next = true;
        if (sr.battleSetup.cutscenePart+1 == sr.battleSetup.lastCutscene.parts.Count)
        {
            next = false;
        }
        else if (sr.battleSetup.lastCutscene.parts[sr.battleSetup.cutscenePart+1].cutPartType != CutPartType.Dialogue && sr.battleSetup.lastCutscene.parts[sr.battleSetup.cutscenePart+1].cutPartType != CutPartType.Choice)
        {
            next = false;
        }
        sr.battleSetup.dialogueAutoClose = autoClose;
        sr.battleSetup.DialogueBoxPrepare(this, next);

        if (autoClose)
        {
            if (useStoryTriggerTime)
            {
                sr.battleSetup.waitTime = (float)trigger2.GetTriggerValue(triggerPart2);
            }
            else
            {
                sr.battleSetup.waitTime = waitTime;
            }
            
            sr.battleSetup.StartCoroutine(sr.battleSetup.cutsceneWait(sr.battleSetup.transform));
        }
    }

    public void Duplicate (CutPart cp)
    {
        CutPart_Dialogue _cp = (CutPart_Dialogue)cp;

        _title = _cp._title;
        _startNextSimultaneous = _cp.startNextSimultaneous;

        text = _cp.text;
        autoClose = _cp.autoClose;

        waitTime = _cp.waitTime;
        useStoryTriggerTime = _cp.useStoryTriggerTime;
        trigger = _cp.trigger2;
        triggerPart = _cp.triggerPart2;
    }

    public void GetTitle()
    {
        string start = "[" + _type + "]";
        string context = _title;
        _name = (start + " " + text);
    }

    #if UNITY_EDITOR
        public void DrawInspectorPart(Cutscene cutscene = null, CutsceneScript cutsceneScript = null)
        {
            text = EditorGUILayout.TextArea(text, GUILayout.MinHeight(100));
            
            EditorGUILayout.Space(10);
            showFoldout = EditorGUILayout.Foldout(showFoldout, "Insert Story Trigger Value");
            if (showFoldout)
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
                    if (GUILayout.Button("Insert"))
                    {
                        text += "##" + trigger.name + "/" + trigger.triggerParts[part].title + "##";
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
            
            startNextSimultaneous = EditorGUILayout.Toggle("Start Next Part Alongside: ", startNextSimultaneous);

            autoClose = EditorGUILayout.Toggle("Close Automatically: ", autoClose);

            if (autoClose)
            {
                useStoryTriggerTime = EditorGUILayout.Toggle("Use Trigger Time: ", useStoryTriggerTime);
                if (useStoryTriggerTime)
                {
                    EditorGUILayout.BeginHorizontal();
                    trigger2 = (StoryTrigger)EditorGUILayout.ObjectField(trigger2, typeof(StoryTrigger), false);
                    if (trigger2 != null)
                    {
                        int part = trigger2.GetTriggerPartIndex(triggerPart2);
                        if (part == -1) part = 0;
                        string lastTriggerPart = triggerPart2;
                        triggerPart2 = trigger2.triggerParts[EditorGUILayout.Popup(part, trigger2.GetTriggerPartList())].title;
                        if (lastTriggerPart != triggerPart2)
                        {
                            part = trigger2.GetTriggerPartIndex(triggerPart2);
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    waitTime = EditorGUILayout.FloatField("Wait Time (In Seconds): ", waitTime);
                }
            }

            
        }
    #endif
}
