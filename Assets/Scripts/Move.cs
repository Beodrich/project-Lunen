﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    //public Animator animator;
    public float speed = 5f;

    public bool inGrass = false;

    public GameObject grassObject;

    public float grassEncounterCheckCurrent;
    public float grassEncounterCheckEvery;

    private Rigidbody2D rb2D;

    [HideInInspector]
    public BattleSetup battle;

    public void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private Vector2 movement = Vector3.zero;
    private void Update()
    {
        movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        /*  if (movement.x != 0 || movement.y != 0)
          {
              if (inGrass)
              {
                  grassEncounterCheckCurrent -= Time.deltaTime;
                  if (grassEncounterCheckCurrent <= 0)
                  {
                      TryWildEncounter(grassObject.GetComponent<GrassEncounter>());
                      grassEncounterCheckCurrent += grassEncounterCheckEvery;
                  }
              }
          }

          if (battle == null)
          {
              battle = GameObject.Find("BattleSetup").GetComponent<BattleSetup>();
          }*/

        //animator.SetFloat("Horizontal", movement.x);
        //animator.SetFloat("Vertical", movement.y);
        //animator.SetFloat("Magnitude", movement.magnitude);
    }

    public bool TryWildEncounter(GrassEncounter encounter)
    {
        float chance = Random.Range(0f, 100f);
        if (chance < encounter.chanceModifier)
        {
            PrepareWildEncounter(encounter);
            return true;
        }
        else return false;
    }

    public void PrepareWildEncounter(GrassEncounter encounter)
    {
        float randomChoice = Random.Range(0f, 100f);
        float searcher = 0f;
        int index = -1;

        while (searcher < randomChoice && index < encounter.possibleEncounters.Count-1)
        {
            index++;
            searcher += encounter.possibleEncounters[index].chanceWeight;
        }

        battle.GenerateWildEncounter(encounter.possibleEncounters[index].lunen, Random.Range(encounter.possibleEncounters[index].LevelRange.Min, encounter.possibleEncounters[index].LevelRange.Max + 1));

        battle.MoveToBattle(0,0);
    }

    private void FixedUpdate()
    {
        rb2D.MovePosition(rb2D.position + movement * speed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Grass":
                inGrass = true;
                grassObject = other.gameObject;
                break;
            case "TrainerSight":
                TrainerEncounter encounter = other.gameObject.GetComponent<TrainerEncounter>();
                if (encounter != null)
                {
                    battle.GenerateTrainerBattle(encounter);
                    battle.MoveToBattle(0, 0);
                }
                break;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Grass":
                inGrass = false;
                grassObject = null;
                break;
        }
    }
}
