using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LunenActionPanel : MonoBehaviour
{
    public List<GameObject> ActionButtons;
    public List<LunenActionButton> ActionButtonScripts;
    public GameObject ActionListText;

    public void FindScripts()
    {
        for (int i = 0; i < ActionButtons.Count; i++)
        {
            ActionButtonScripts[i] = ActionButtons[i].GetComponent<LunenActionButton>();
        }
    }
}
