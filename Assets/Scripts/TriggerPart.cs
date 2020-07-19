using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum TriggerTypes
{
    Bool,
    Int,
    Float,
    Double,
    String,
}

[System.Serializable]
public class TriggerPart
{
    
    public string title;
    public TriggerTypes type;

    public bool defaultBool;
    public int defaultInt;
    public float defaultFloat;
    public double defaultDouble;
    public string defaultString;

    public bool triggerBool;
    public int triggerInt;
    public float triggerFloat;
    public double triggerDouble;
    public string triggerString;

    [TextArea(1,5)]
    public string notes;

    
}