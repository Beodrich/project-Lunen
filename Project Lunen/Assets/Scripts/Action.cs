using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour
{
    public string Name;
    public Monster User;
    public Monster Target;
    public Types.Element Type;
    public float Cooldown;
    public GameObject Animation;
    public int Power;

    public void Attack()
    {
        Target.Health.Current -= 1;
    }
}
