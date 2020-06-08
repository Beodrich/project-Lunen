using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class GenerateMonster : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;

    public enum TargetPlayer
    {
        Player1,
        Player2,
        NoTarget
    }

    public LunaDex.LunenEnum LunenBase;
    [Range(1, 50)]
    public int LunenLevel = 1;
    public TargetPlayer targetPlayer;

    private GameObject New1;
    private Monster NewMonster1;

    public bool Generate()
    {
        sr = GetComponent<SetupRouter>();
        for (int i = 0; i < sr.battleSetup.PlayerLunenTeam.Count; i++)
        {
            if (sr.battleSetup.PlayerLunenTeam[i] == null)
            {
                sr.battleSetup.PlayerLunenTeam.RemoveAt(i);
                i--;
            }
        }
        New1 = Instantiate(sr.lunaDex.MonsterTemplate);
        //New1.transform.SetParent(this.transform);
        NewMonster1 = New1.GetComponent<Monster>();
        NewMonster1.loopback = sr;
        NewMonster1.Level = LunenLevel;
        NewMonster1.TemplateToMonster(sr.lunaDex.GetLunen(LunenBase));
        for (int i = 1; i <= LunenLevel; i++)
        {
            NewMonster1.GetLevelUpMove(i);
        }
        
        
        switch (targetPlayer)
        {
            case TargetPlayer.Player1:
                NewMonster1.transform.SetParent(transform);
                sr.battleSetup.PlayerLunenTeam.Add(New1);
                break;
            case TargetPlayer.Player2:
                NewMonster1.transform.SetParent(transform);
                sr.battleSetup.EnemyLunenTeam.Add(New1);
                NewMonster1.MonsterTeam = Director.Team.EnemyTeam;
                break;
        }
        return true;
    }
}
