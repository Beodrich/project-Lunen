using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class GenerateMonster : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;

    [System.Serializable]
    public class LunenSetup
    {
        public Lunen species;
        public int level;
    }

    public enum TargetPlayer
    {
        Player1,
        Player2,
        NoTarget
    }

    public enum SortMovesType
    {
        LowestFirst,
        HighestFirst,
        None
    }

    public Lunen LunenBase;
    [Range(1, 30)]
    public int LunenLevel = 1;
    public TargetPlayer targetPlayer;

    private GameObject New1;
    private Monster NewMonster1;

    public bool Generate()
    {
        New1 = GenerateLunen(LunenBase, LunenLevel);

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

    public GameObject GenerateLunen(Lunen species, int level, SortMovesType moveSort = SortMovesType.HighestFirst)
    {
        sr = GetComponent<SetupRouter>();
        New1 = Instantiate(sr.database.MonsterTemplate);
        NewMonster1 = New1.GetComponent<Monster>();
        NewMonster1.sr = sr;
        NewMonster1.Level = level;
        NewMonster1.SourceLunen = species;
        NewMonster1.TemplateToMonster(species);
        switch  (moveSort)
        {
            default: break;
            case SortMovesType.LowestFirst:
                NewMonster1.GetPreviousMoves();
                NewMonster1.SortMoves(false);
            break;
            case SortMovesType.HighestFirst:
                NewMonster1.GetPreviousMoves();
                NewMonster1.SortMoves(true);
            break;
        }
        
        return New1;
    }
}
