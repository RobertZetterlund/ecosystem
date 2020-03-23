using System;
using System.Collections.Generic;
using UnityEngine;

public class Memory
{
    private Vector3 WaterVector;
    private Vector3 FoeVector;
    private Vector3 MateVector;
    private Vector3 FoodVector;

    private List<int> rejectedByIDs;

    public Memory()
    {
        rejectedByIDs = new List<int>();
    }

    public void WriteWaterToMemory(Vector3 WaterVector) {
        this.WaterVector = WaterVector;
    }

    public void WriteFoeToMemory(Vector3 foeVector) {
        this.FoeVector = foeVector;
    }

    public void WriteMateToMemory(Vector3 MateVector) {
        this.MateVector = MateVector;
    }

    public void WriteFoodToMemory(Vector3 FoodVector) {
        this.FoodVector = FoodVector;
    }

    public void AddRejection(int id) {
        if(rejectedByIDs.Count >= 5) {
            rejectedByIDs.RemoveAt(0);
        }
        rejectedByIDs.Add(id);
    }

    public Boolean CheckIfRejected(int id) {
        return rejectedByIDs.Contains(id);
    }

    public void forgetRejection(int id) {
        rejctedByIds.Remove(id);
    }

    public Vector3 ReadFoodFromMemory() {
        return FoodVector;
    }
    public Vector3 ReadFoeFromMemory() {
        return FoeVector;
    }
    public Vector3 ReadWaterFromMemory() {
        return WaterVector;
    }
    public Vector3 ReadMateFromMemory() {
        return MateVector;
    }
}