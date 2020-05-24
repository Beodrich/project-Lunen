using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class GenerateMonster : MonoBehaviour
{
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
        LunaDex ld = GetComponent<LunaDex>();
        BattleSetup bs = GetComponent<BattleSetup>();
        for (int i = 0; i < bs.PlayerLunenTeam.Count; i++)
        {
            if (bs.PlayerLunenTeam[i] == null)
            {
                bs.PlayerLunenTeam.RemoveAt(i);
                i--;
            }
        }
        New1 = Instantiate(ld.MonsterTemplate);
        //New1.transform.SetParent(this.transform);
        NewMonster1 = New1.GetComponent<Monster>();
        NewMonster1.battleSetup = bs;
        NewMonster1.lunaDex = ld;
        NewMonster1.Level = LunenLevel;
        NewMonster1.TemplateToMonster(ld.GetLunen(LunenBase));
        for (int i = 1; i <= LunenLevel; i++)
        {
            NewMonster1.GetLevelUpMove(i);
        }
        
        
        switch (targetPlayer)
        {
            case TargetPlayer.Player1:
                NewMonster1.transform.SetParent(transform);
                bs.PlayerLunenTeam.Add(New1);
                break;
            case TargetPlayer.Player2:
                NewMonster1.transform.SetParent(transform);
                bs.EnemyLunenTeam.Add(New1);
                NewMonster1.MonsterTeam = Director.Team.EnemyTeam;
                break;
        }
        return true;
    }
}
