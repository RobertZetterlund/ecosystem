using System;
using System.Collections.Generic;
using UnityEngine;

public class Memory
{
    private GameObject Water;
    private GameObject Foe;
    private GameObject Mate;
    private GameObject Food;

    private List<int> rejectedByIDs;

    public Memory()
    {
        rejectedByIDs = new List<int>();
    }

    public void WriteSensedEventToMemory(SensedEvent sE)
    {
        // dont overwrite if null
        if (!(sE.GetWater() is null))
        {
            WriteWaterToMemory(sE.GetWater());
        }
        if (!(sE.GetFoe() is null))
        {
            WriteFoeToMemory(sE.GetFoe());
        }
        if (!(sE.GetMate() is null))
        {
            WriteMateToMemory(sE.GetMate());
        }
        if (!(sE.GetFood() is null))
        {
            WriteFoodToMemory(sE.GetFood());
        }
    }

    private void WriteWaterToMemory(GameObject Water)
    {
        if (!CheckIfRejected(Water.GetInstanceID()))
            this.Water = Water;
    }

    private void WriteFoeToMemory(GameObject Foe)
    {
        if (!CheckIfRejected(Foe.GetInstanceID()))
            this.Foe = Foe;
    }

    private void WriteMateToMemory(GameObject Mate)
    {
        if (!CheckIfRejected(Mate.GetInstanceID()))
            this.Mate = Mate;
    }

    private void WriteFoodToMemory(GameObject Food)
    {
        if (!CheckIfRejected(Food.GetInstanceID()))
            this.Food = Food;
    }

    public void AddRejection(int id)
    {
        if (rejectedByIDs.Count >= 5)
        {
            rejectedByIDs.RemoveAt(0);
        }
        rejectedByIDs.Add(id);
        if ( Water != null && Water.GetInstanceID() == id)
        {
            Water = null;
        }else if (Foe != null && Foe.GetInstanceID() == id)
        {
            Foe = null;
        }else if (Mate != null && Mate.GetInstanceID() == id)
        {
            Mate = null;
        }else if (Food != null && Food.GetInstanceID() == id)
        {
            Food = null;
        }
    }

    public Boolean CheckIfRejected(int id)
    {
        return rejectedByIDs.Contains(id);
    }

    public void forgetRejection(int id)
    {
        rejectedByIDs.Remove(id);
    }

    public GameObject ReadFoodFromMemory()
    {
        return Food;
    }
    public GameObject ReadFoeFromMemory()
    {
        return Foe;
    }
    public GameObject ReadWaterFromMemory()
    {
        return Water;
    }
    public GameObject ReadMateFromMemory()
    {
        return Mate;
    }
    public GameObject GetTargObj(EntityAction act)
    {
        if(act == EntityAction.GoingToFood)
        {
            return ReadFoodFromMemory();
        }else if(act == EntityAction.GoingToWater)
        {
            return ReadWaterFromMemory();
        }else if(act == EntityAction.SearchingForMate)
        {
            return ReadMateFromMemory();
        }
        else if (act == EntityAction.Escaping)
        {
            return ReadFoeFromMemory();
        }
        return null;
    }
}