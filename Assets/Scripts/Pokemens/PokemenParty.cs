using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemenParty : MonoBehaviour
{
    [SerializeField] List<Pokemen> pokemens;

    private void Start()
    {
        foreach (var p in pokemens)
        {
            p.init();
        }
    }

    public Pokemen GetHealthyPokemen()
    {
        return pokemens.Where(x => x.HP > 0).FirstOrDefault();
    }
}
