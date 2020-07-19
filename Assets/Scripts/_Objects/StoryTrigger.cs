using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "New Story Trigger", menuName = "GameElements/Story Trigger")]
public class StoryTrigger : ScriptableObject
{
    public List<TriggerPart> triggerParts;

    public string[] GetTriggerPartList()
    {
        List<string> partList = new List<string>();
        foreach(TriggerPart part in triggerParts) partList.Add(part.title);
        return partList.ToArray();
    }

    public int GetTriggerPartIndex(string _name)
    {
        for (int i = 0; i < triggerParts.Count; i++)
        {
            if (triggerParts[i].title == _name) return i;
        }
        return -1;
    }

    public object GetDefaultValue(string _name)
    {
        return (GetDefaultValue(GetTriggerPartIndex(_name)));
    }

    public object GetDefaultValue(int index)
    {
        TriggerPart part = triggerParts[index];
        switch (part.type)
        {
            case TriggerTypes.Bool: return part.defaultBool;
            case TriggerTypes.Int: return part.defaultInt;
            case TriggerTypes.Float: return part.defaultFloat;
            case TriggerTypes.Double: return part.defaultDouble;
            case TriggerTypes.String: return part.defaultString;
        }
        return null;
    }

    public object GetTriggerValue(string _name)
    {
        int index = GetTriggerPartIndex(_name);
        if (index != -1) return (GetTriggerValue(index));
        return null;
    }

    public object GetTriggerValue(int index)
    {
        TriggerPart part = triggerParts[index];
        switch (part.type)
        {
            case TriggerTypes.Bool: return part.triggerBool;
            case TriggerTypes.Int: return part.triggerInt;
            case TriggerTypes.Float: return part.triggerFloat;
            case TriggerTypes.Double: return part.triggerDouble;
            case TriggerTypes.String: return part.triggerString;
        }
        return null;
    }

    public bool SetTriggerValue(string _name, object newValue)
    {
        int index = GetTriggerPartIndex(_name);
        if (index != -1) return (SetTriggerValue(index, newValue));
        else return false;
    }

    public bool SetTriggerValue(int index, object newValue)
    {
        TriggerPart part = triggerParts[index];
        switch (part.type)
        {
            case TriggerTypes.Bool:
                part.triggerBool = (bool)newValue;
                return true;
            case TriggerTypes.Int:
                part.triggerInt = (int)newValue;
                return true;
            case TriggerTypes.Float:
                part.triggerFloat = (float)newValue;
                return true;
            case TriggerTypes.Double:
                part.triggerDouble = (double)newValue;
                return true;
            case TriggerTypes.String:
                part.triggerString = (string)newValue;
                return true;
        }
        return false;
    }

    public bool IsTriggerEqual(string _name, object otherValue)
    {
        return (IsTriggerEqual(GetTriggerPartIndex(_name), otherValue));
    }

    public bool IsTriggerEqual(int index, object otherValue)
    {
        TriggerPart part = triggerParts[index];
        switch (part.type)
        {
            case TriggerTypes.Bool:
                return (part.triggerBool == (bool) otherValue);
            case TriggerTypes.Int:
                return (part.triggerInt == (int) otherValue);
            case TriggerTypes.Float:
                return (part.triggerFloat == (float) otherValue);
            case TriggerTypes.Double:
                return (part.triggerDouble == (double) otherValue);
            case TriggerTypes.String:
                return (part.triggerString == (string) otherValue);
        }
        return false;
    }

    public bool IsTriggerLessThan(string _name, object otherValue, bool orEqualTo = false)
    {
        return (IsTriggerLessThan(GetTriggerPartIndex(_name), otherValue, orEqualTo));
    }

    public bool IsTriggerLessThan(int index, object otherValue, bool orEqualTo = false)
    {
        TriggerPart part = triggerParts[index];
        switch (part.type)
        {
            case TriggerTypes.Bool:
                return (orEqualTo ? (part.triggerBool == (bool) otherValue) : (part.triggerBool != (bool) otherValue));
            case TriggerTypes.Int:
                return (orEqualTo ? (part.triggerInt <= (int) otherValue) : (part.triggerInt < (int) otherValue));
            case TriggerTypes.Float:
                return (orEqualTo ? (part.triggerFloat <= (float) otherValue) : (part.triggerFloat < (float) otherValue));
            case TriggerTypes.Double:
                return (orEqualTo ? (part.triggerDouble <= (double) otherValue) : (part.triggerDouble < (double) otherValue));
            case TriggerTypes.String:
                return (orEqualTo ? (part.triggerString == (string) otherValue) : (part.triggerString != (string) otherValue));
        }
        return false;
    }

    public bool IsTriggerGreaterThan(string _name, object otherValue, bool orEqualTo = false)
    {
        return (IsTriggerGreaterThan(GetTriggerPartIndex(_name), otherValue, orEqualTo));
    }

    public bool IsTriggerGreaterThan(int index, object otherValue, bool orEqualTo = false)
    {
        TriggerPart part = triggerParts[index];
        switch (part.type)
        {
            case TriggerTypes.Bool:
                return (orEqualTo ? (part.triggerBool == (bool) otherValue) : (part.triggerBool != (bool) otherValue));
            case TriggerTypes.Int:
                return (orEqualTo ? (part.triggerInt >= (int) otherValue) : (part.triggerInt > (int) otherValue));
            case TriggerTypes.Float:
                return (orEqualTo ? (part.triggerFloat >= (float) otherValue) : (part.triggerFloat > (float) otherValue));
            case TriggerTypes.Double:
                return (orEqualTo ? (part.triggerDouble >= (double) otherValue) : (part.triggerDouble > (double) otherValue));
            case TriggerTypes.String:
                return (orEqualTo ? (part.triggerString == (string) otherValue) : (part.triggerString != (string) otherValue));
        }
        return false;
    }

    public void ResetToDefaults()
    {
        for (int i = 0; i < triggerParts.Count; i++)
        {
            SetTriggerValue(i, GetDefaultValue(i));
        }
    }
}
