using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUG_BattleBuilder : MonoBehaviour
{
    public bool Player2IsAI;

    public GameObject Player1;
    public GameObject Player2;

    public GameObject Player1Lunen;
    public GameObject Player2Lunen;

    public GameObject[] Player1Moves;
    public GameObject[] Player2Moves;

    public int Player1Level;
    public int Player2Level;

    public GameObject dex;
    public GameObject TextPlayer1;
    public GameObject TextPlayer2;
    public GameObject Template;
    public GameObject Button;

    private GameObject New1;
    private GameObject New2;

    private Monster NewMonster1;
    private Monster NewMonster2;

    // Start is called before the first frame update
    void Start()
    {
        New1 = Instantiate(Template);
        New2 = Instantiate(Template);

        NewMonster1 = New1.GetComponent<Monster>();
        NewMonster2 = New2.GetComponent<Monster>();

        NewMonster1.Level = Player1Level;
        NewMonster2.Level = Player2Level;

        NewMonster1.ActionSet.AddRange(Player1Moves);
        NewMonster2.ActionSet.AddRange(Player2Moves);

        NewMonster1.TemplateToMonster(Player1Lunen.GetComponent<Lunen>());
        NewMonster2.TemplateToMonster(Player2Lunen.GetComponent<Lunen>());

        Player1.GetComponent<Player>().Team.Add(NewMonster1);
        Player1.GetComponent<Player>().Team.Add(NewMonster2);

        NewMonster1.DEBUG_TEXT_OUTPUT = TextPlayer1;
        NewMonster2.DEBUG_TEXT_OUTPUT = TextPlayer2;
       
        NewMonster1.DEBUG_DISPLAY_TEXT();
        NewMonster2.DEBUG_DISPLAY_TEXT();

    }
}
