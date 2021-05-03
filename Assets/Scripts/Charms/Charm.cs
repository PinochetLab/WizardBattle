using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Charm : MonoBehaviour
{
    public abstract void Impact(PlayerController player);
    public abstract void UnImpact(PlayerController player);
}
