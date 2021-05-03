using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Charm/CharmItem")]
public class CharmItem : ScriptableObject
{
    public string name;
    public float loadTime;
    public Sprite icon;
}
