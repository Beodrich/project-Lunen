﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class UIPanelCollection : MonoBehaviour
{
    [System.Serializable]
    public class UIPanel
    {
        public string name;
        public GameObject gameObject;
        public UIElementCollection elementCollection;
    }

    public List<UIPanel> UIPanels;
    public List<string> StartingPanels;
    public List<string> CurrentlyOpenPanels;

    private void Awake() {
        
        GetElementCollections();
        DisableAllPanelsImmediately();
    }

    public void GetElementCollections()
    {
        foreach (UIPanel panel in UIPanels)
        {
            panel.elementCollection = panel.gameObject.GetComponent<UIElementCollection>();
        }
    }

    public bool SetPanelState(string panelName, UITransition.State state)
    {
        for (int i = 0; i < UIPanels.Count; i++)
        {
            if (UIPanels[i].name == panelName)
            {
                UIPanels[i].elementCollection.SetCollectionState(state);
                return true;
            }
        }
        Debug.Log("Panel " + panelName + " not found!");
        return false;
    }

    public UIElementCollection GetPanelWithString(string panelName)
    {
        for (int i = 0; i < UIPanels.Count; i++)
        {
            if (UIPanels[i].name == panelName)
            {
                return UIPanels[i].elementCollection;
            }
        }
        return null;
    }

    //THIS IS JUST FOR TESTING! DON'T USE THIS ONE IN THE FINAL GAME
    public void EnableAllPanels()
    {
        for (int i = 0; i < UIPanels.Count; i++)
        {
            UIPanels[i].elementCollection.SetCollectionState(UITransition.State.Enable);
        }
    }

    public void EnableStartingPanels()
    {
        for (int i = 0; i < UIPanels.Count; i++)
        {
            if (StartingPanels.Contains(UIPanels[i].name))
            {
                SetPanelState(UIPanels[i].name, UITransition.State.Enable);
            }
            else
            {
                SetPanelState(UIPanels[i].name, UITransition.State.ImmediateDisable);
            }
        }
    }

    public void DisableAllPanels()
    {
        for (int i = 0; i < UIPanels.Count; i++)
        {
            UIPanels[i].elementCollection.SetCollectionState(UITransition.State.Disable);
        }
    }

    public void DisableAllPanelsImmediately()
    {
        for (int i = 0; i < UIPanels.Count; i++)
        {
            UIPanels[i].elementCollection.SetCollectionState(UITransition.State.ImmediateDisable);
        }
    }
}
