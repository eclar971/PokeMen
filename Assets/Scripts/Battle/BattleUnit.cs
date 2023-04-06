using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] PokemenBase _base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    public Pokemen Pokemen { get; set; }
    public void Setup()
    {
        Pokemen = new Pokemen(_base, level);
        if (isPlayerUnit )
        {
            GetComponent<Image>().sprite = Pokemen.Base.BackSprite;
        }
        else
        {
            GetComponent<Image>().sprite = Pokemen.Base.FrontSprite;
        }
    }
}
