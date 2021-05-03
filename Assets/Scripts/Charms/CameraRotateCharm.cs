using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotateCharm : Charm
{
    public override void Impact(PlayerController player)
    {
        RotateCamera(player);
        print("Rotate Camera");
    }

    public override void UnImpact(PlayerController player)
    {
        player.SetCamAngle(0f);
    }

    void RotateCamera(PlayerController player)
    {
        float angle = Random.Range(-20f, 20f);
        player.SetCamAngle(angle);
    }
}
