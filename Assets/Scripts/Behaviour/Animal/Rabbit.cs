using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class Rabbit : Animal
{

    public override void Init(AnimalTraits traits)
    {
        base.Init(traits);
    }

    protected override void Start()
    {
        runAnimationspeedFactor = 1.3f;
        base.Start();
        
    }

}

