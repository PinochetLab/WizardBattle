using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCharm : Charm
{
    public PlayerController player;

    public override void Impact(PlayerController player)
    {
        player.Damage(10f, player);
       // Debug.Log("Damage");
    }

    public override void UnImpact(PlayerController player)
    {
        
    }
}
