using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenameLunen : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;
    public Monster monster;
    public GameData.PlayerLunen playerLunen;
    public InputField inputField;

    void Awake()
    {
        sr = GameObject.Find("BattleSetup").GetComponent<SetupRouter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (monster != null)
        {
            if (monster.Nickname != inputField.text)
            {
                monster.Nickname = inputField.text;
                sr.canvasCollection.UpdatePartyPanelLunen();
            }
            
        }
        if (playerLunen != null)
        {
            if (playerLunen.nickname != inputField.text)
            {
                playerLunen.nickname = inputField.text;
                sr.canvasCollection.RefreshStorageButtons();
            }
            
        }
    }

    public void SetMonster(Monster _monster = null, GameData.PlayerLunen _pl = null)
    {
        monster = _monster;
        playerLunen = _pl;
        if (monster != null)
        {
            inputField.text = monster.Nickname;
        }
        if (playerLunen != null)
        {
            inputField.text = playerLunen.nickname;
        }
    }
}
