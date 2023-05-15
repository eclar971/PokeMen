using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;
    [SerializeField] GameObject expBar;
    
    Pokemen _pokemen;

    public void SetData(Pokemen pokemen)
    {
        _pokemen = pokemen;

        nameText.text = pokemen.Base.Name;
        SetLvl();
        hpBar.SetHP((float) pokemen.HP / pokemen.MaxHp);
        SetExp();
    }
    public void SetLvl()
    {
        levelText.text = "Lvl " + _pokemen.Level;
    }
    public void SetExp()
    {
        if (expBar == null) return;
        
        float normalizedExp = GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }
    public IEnumerator SetExpSmooth(bool reset = false)
    {
        if (expBar == null) yield break;
        if (reset)
        {
            expBar.transform.localScale = new Vector3(0, 1, 1);
        }

        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp,1.5f).WaitForCompletion();
    }
    float GetNormalizedExp()
    {
        var levelNext = _pokemen.Level + 1;
        int currLevelExp = _pokemen.Level * _pokemen.Level * _pokemen.Level;
        int nextLevelExp = levelNext * levelNext * levelNext;

        float normalizedExp = (float)(_pokemen.Exp - currLevelExp) / (nextLevelExp - currLevelExp);
        Debug.Log(normalizedExp);
        return Mathf.Clamp01(normalizedExp);
    }
    public IEnumerator UpdateHP()
    {

        yield return hpBar.SetHPSmooth((float)_pokemen.HP / _pokemen.MaxHp);
    }
}
