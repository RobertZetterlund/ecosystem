using System;
using UnityEngine;

public class SearchAction
{
    private string targetGameTag;
    private Animal animal;

    public SearchAction(Animal animal)
    {
        this.animal = animal;
    }

    public void SetTarget(string gameTag)
    {
        targetGameTag = gameTag;
    }



}
