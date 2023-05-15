using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Pokemen> wildPokemens;

    public Pokemen GetRandomWildPokemen()
    {
        var wildPokemen = wildPokemens[Random.Range(0,wildPokemens.Count)];
        wildPokemen.init();
        return wildPokemen;
    }
}
