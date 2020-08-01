using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHealthText : MonoBehaviour
{
    public Text text;
    public Monster referenceMonster;

    private void Update() 
    {
        if (referenceMonster != null)
        {
            if (referenceMonster.MonsterTeam == Director.Team.PlayerTeam)
            {
                text.text = "HP " + (int)referenceMonster.Health.z + "/" + (int)referenceMonster.GetMaxHealth();
            }
            else
            {
                text.text = " ";
            }
            
        }
        else
        {
            text.text = " ";
        }
        
    }
}
