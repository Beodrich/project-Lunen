using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    

    [Header("Individual Stuff")]

    public string Nickname;
    public int Level;

    [VectorLabels("Current", "Required")]
    public Vector2Int Exp;

    public List<GameObject> ActionSet;

    

    [Header("Stats")]

    [VectorLabels("Base", " Mod", " Current")]
    public Vector3Int Health;
    [VectorLabels("Base", " Mod", " Current")]
    public Vector3Int Attack;
    [VectorLabels("Base", " Mod", " Current")]
    public Vector3Int Defense;
    [VectorLabels("Base", " Mod", " Current")]
    public Vector3Int Speed;

    

    [Header("Wild Monster Stuff")]

    public bool Enemy;

    [HideInInspector]
    public Lunen SourceLunen;
    [HideInInspector]
    public Director loopback;
    [HideInInspector]
    public float CurrCooldown;

    private void Start()
    {
        CurrCooldown = 1f;
    }

    private void Update()
    {
        if (Time.unscaledDeltaTime < 0.25f)
        {
            if (CurrCooldown > 0f)
            {
                CurrCooldown -= Time.unscaledDeltaTime;
            }
            else
            {
                CurrCooldown = 0f;
            }
        }
        if (Health.z <= 0)
        {
            if (loopback != null) loopback.ScanBothParties();
            if (Enemy) Destroy(gameObject);
        }
    }

    public void TemplateToMonster(Lunen template)
    {
        SourceLunen = template;

        Health.x = template.Health.x;
        Attack.x = template.Attack.x;
        Defense.x = template.Defense.x;
        Speed.x = template.Speed.x;

        Health.y = template.Health.y * Level;
        Attack.y = template.Attack.y * Level;
        Defense.y = template.Defense.y * Level;
        Speed.y = template.Speed.y * Level;
        Health.z = GetMaxHealth();
        CalculateStats();
        Nickname = template.Name;
        SetObjectName();
    }

    public void AssortPointsAI(int points)
    {
        for (int i = 0; i < points; i++)
        {
            int dividend = 4;
            int random = Random.Range(0, dividend);
            switch (random)
            {
                case 0: Health.y += 1; break;
                case 1: Attack.y += 1; break;
                case 2: Defense.y += 1; break;
                case 3: Speed.y += 1; break;
            }

        }
    }

    public void CalculateStats()
    {
        Attack.z = Attack.x + Attack.y;
        Defense.z = Defense.x + Defense.y;
        Speed.z = Speed.x + Speed.y;
    }

    public void SetObjectName()
    {
        transform.name = SourceLunen.Name + "_" + Nickname + "_Monster";
    }

    public int GetMaxHealth()
    {
        return Health.x + Health.y;
    }
}
