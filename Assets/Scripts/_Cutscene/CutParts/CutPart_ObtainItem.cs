using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CutPart_ObtainItem : CutPart
{
    //Standard Values
    [SerializeField] public string _name = "";
    public static CutPartType _type = CutPartType.ObtainItem;
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

    public Item itemObtained;
    public int itemAmount;

    //Functions

    public void PlayPart (SetupRouter sr)
    {
        sr.database.SetTriggerValue("ItemObtain/Name", itemObtained.name);
        sr.database.SetTriggerValue("ItemObtain/Amount", itemAmount);
        sr.inventory.AddItem(itemObtained, itemAmount);
        sr.battleSetup.AdvanceCutscene();
    }

    public void Duplicate (CutPart cp)
    {
        CutPart_ObtainItem _cp = (CutPart_ObtainItem)cp;

        _title = _cp._title;
        _startNextSimultaneous = _cp.startNextSimultaneous;

        itemObtained = _cp.itemObtained;
        itemAmount = _cp.itemAmount;
    }

    public void GetTitle()
    {
        string start = "[" + _type + "]";
        string context = _title;
        if (context == "")
        {
            context += itemAmount + "x " + itemObtained.name;
        }
        _name = (start + " " + context);
    }

    #if UNITY_EDITOR
        public void DrawInspectorPart(SerializedProperty serializedProperty, Cutscene cutscene = null, CutsceneScript cutsceneScript = null)
        {
            itemObtained = (Item)EditorGUILayout.ObjectField("Item: ", itemObtained, typeof(Item), true);
            itemAmount = EditorGUILayout.IntField("Amount: ", itemAmount);
        }
    #endif
}
