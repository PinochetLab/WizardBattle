using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsItem : MonoBehaviour
{
    [SerializeField] Text nickNameText;
    [SerializeField] Text killsText;
    [SerializeField] Text damageText;

    public void SetUp(string name, int kills, float damage)
    {
        nickNameText.text = name;
        killsText.text = kills.ToString();
        damageText.text = damage.ToString();
    }
}
