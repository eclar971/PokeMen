using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Pokemen> wildPokemens;

    public Pokemen GetRandomWildPokemen(Pokemen Playerpokemen)
    {
        var wildPokemen = wildPokemens[Random.Range(0,wildPokemens.Count)];
        wildPokemen.Level = Playerpokemen.Level + Random.Range(-4,4);
        wildPokemen.init();
        return wildPokemen;
    }
}
