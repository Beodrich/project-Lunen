using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElementCollection : MonoBehaviour
{
    public List<GameObject> UIObjects;
    [HideInInspector] public List<UITransition> UITransitions;
    public UITransition.State currentState;

    public void Awake()
    {
        UITransitions = new List<UITransition>();
        foreach (GameObject go in UIObjects) UITransitions.Add(go.GetComponent<UITransition>());
        foreach (UITransition uit in UITransitions) uit.SetState(UITransition.State.ImmediateDisable);
    }

    public void SetCollectionState(UITransition.State state)
    {
        currentState = state;
        foreach (UITransition t in UITransitions) t.SetState(state);
    }
}
