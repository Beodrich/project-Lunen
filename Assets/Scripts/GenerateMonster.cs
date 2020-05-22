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
    [SearchableEnum]
    public List<LunaDex.ActionEnum> LunenMoves;
    [Range(1, 50)]
    public int LunenLevel = 1;
    public TargetPlayer targetPlayer;

    private GameObject New1;
    private Monster NewMonster1;

    public bool Generate()
    {
        LunaDex ld = GetComponent<LunaDex>();
        New1 = Instantiate(ld.MonsterTemplate);
        //New1.transform.SetParent(this.transform);
        NewMonster1 = New1.GetComponent<Monster>();
        NewMonster1.Level = LunenLevel;
        for (int i = 0; i < LunenMoves.Count; i++)
        {
            NewMonster1.ActionSet.Add(Instantiate(ld.GetActionObject(LunenMoves[i])));
            NewMonster1.ActionSet[i].transform.SetParent(NewMonster1.transform);
        }
        
        NewMonster1.TemplateToMonster(ld.GetLunen(LunenBase));
        switch (targetPlayer)
        {
            case TargetPlayer.Player1:
                NewMonster1.transform.SetParent(transform);
                GetComponent<BattleSetup>().PlayerLunenTeam.Add(New1);
                break;
            case TargetPlayer.Player2:
                NewMonster1.transform.SetParent(transform);
                GetComponent<BattleSetup>().EnemyLunenTeam.Add(New1);
                NewMonster1.MonsterTeam = Director.Team.EnemyTeam;
                break;
        }
        return true;
    }
}
