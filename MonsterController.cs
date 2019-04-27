using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : CharactersController
{
    protected override void Start()
    {
        base.Start();

        isMonster = true;
    }
}
