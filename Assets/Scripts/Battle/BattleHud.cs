using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;

    public void SetData(Pokemen pokemen)
    {
        nameText.text = pokemen.Base.Name;
        Debug.Log(pokemen.Base.Name);
        levelText.text = "Lvl " + pokemen.Level;
        hpBar.SetHP((float) pokemen.HP / pokemen.MaxHp);
    }
}
