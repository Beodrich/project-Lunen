using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawHealthbar : MonoBehaviour
{
    public Monster targetMonster;

    public Image fill;

    public float PlayerHealth;
    public float LastPlayerHealth;
    public float HealthToShow;
    public float ChangeSpeed;

    public int healthSeparators;

    public Color maxHealthColor;
    public Color highHealthColor;
    public Color midHealthColor;
    public Color minHealthColor;

    public float lowHealthFlash;

    public float flashAdjustment;
    public float flashTimer;
    public float timeBetweenFlashes;
    public float timeBetweenFlashesCurrent;

    public bool flashChanged;

    public bool isShowingHealth;
    public bool isShowingEnergy;

    private float midHealth;

    //Actor playerStats;
    //EnemyMovement bossStats;
    Slider hb;
    //AudioLibrary audio;

    // Start is called before the first frame update
    void Start()
    {
        hb = GetComponent<Slider>();
        //audio = GameObject.Find("Audio Source").GetComponent<AudioLibrary>();
        midHealth = (100 - lowHealthFlash) / 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.unscaledDeltaTime < 0.25f)
        {
            if (targetMonster != null)
            {
                if (isShowingHealth)
                {
                    PlayerHealth = (((float)targetMonster.Health.Current) / ((float)targetMonster.GetMaxHealth())) * 100f;
                    LastPlayerHealth += (PlayerHealth - LastPlayerHealth) * ChangeSpeed * Time.unscaledDeltaTime;
                    HealthToShow = LastPlayerHealth;
                }
                else if (isShowingEnergy)
                {
                    PlayerHealth = (((float)targetMonster.CurrCooldown - (float)targetMonster.LastCooldown) / (float)targetMonster.LastCooldown) * -100f;
                    LastPlayerHealth += (PlayerHealth - LastPlayerHealth) * ChangeSpeed * Time.unscaledDeltaTime;
                    HealthToShow = LastPlayerHealth;
                }
            }
            if (isShowingHealth)
            {
                hb.value = HealthToShow;

                if (HealthToShow > lowHealthFlash)
                {
                    if (HealthToShow >= 99.8f)
                    {
                        fill.color = maxHealthColor;
                    }
                    else if (HealthToShow < midHealth)
                    {
                        fill.color = Color.Lerp(minHealthColor, midHealthColor, (HealthToShow - lowHealthFlash) / (midHealth - lowHealthFlash));

                    }
                    else
                    {
                        fill.color = Color.Lerp(midHealthColor, highHealthColor, (HealthToShow - (100 - midHealth)) / (midHealth - lowHealthFlash));
                    }
                }
                else
                {
                    flashTimer -= Time.unscaledDeltaTime;
                    timeBetweenFlashesCurrent -= Time.unscaledDeltaTime;
                    if (timeBetweenFlashesCurrent < 0)
                    {
                        timeBetweenFlashesCurrent += timeBetweenFlashes;
                        flashTimer = flashAdjustment;

                        //flashChanged = !flashChanged;
                    }
                    fill.color = Color.Lerp(minHealthColor, Color.white, flashTimer / flashAdjustment);
                }
            }
            if (isShowingEnergy)
            {

                hb.value = HealthToShow;

                flashTimer -= Time.unscaledDeltaTime;
                timeBetweenFlashesCurrent -= Time.unscaledDeltaTime;

                if (HealthToShow > 99.9f)
                {
                    if (timeBetweenFlashesCurrent < 0)
                    {
                        timeBetweenFlashesCurrent += timeBetweenFlashes;
                        flashTimer = flashAdjustment;
                        //audio.PlaySFX(6, 1f);
                        //flashChanged = !flashChanged;
                    }
                    fill.color = Color.Lerp(maxHealthColor, Color.white, flashTimer / flashAdjustment);
                }
                else
                {
                    timeBetweenFlashesCurrent = 0;
                    if (HealthToShow < 50)
                    {
                        fill.color = Color.Lerp(minHealthColor, midHealthColor, HealthToShow / 50);

                    }
                    else
                    {
                        fill.color = Color.Lerp(midHealthColor, highHealthColor, (HealthToShow % 50) / 50);
                    }
                }
            }
        }
    }

}
