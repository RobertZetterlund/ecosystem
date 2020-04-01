
public enum EntityField
{
    //The order that they appear in is the order that they will appear in when visualised
    //in the Python script, so it's more neat to keep them grouped like this

    //Concepts
    FoodClose,
    FoodFar,
    FoodPresenceHigh,
    FoodPresenceLow,
    WaterClose,
    WaterFar,
    WaterPresenceHigh,
    WaterPresenceLow,
    MateClose,
    MateFar,
    MatePresenceHigh,
    MatePresenceLow,
    FoeClose,
    FoeFar,
    Fertile,
    NotFertile,
    Hungry,
    NotHungry,
    FoePresenceHigh,
    FoePresenceLow,
    Thirsty,
    NotThirsty,


    //Actions
    Idle,
    Resting,
    GoingToFood,
    GoingToWater,
    Eating,
    Drinking,
    SearchingForMate,
    Reproducing,
    Escaping
}