using System;

[Serializable]
public class FoxSettings : AnimalSettings
{
    public FoxSettings()
    {
        dietFactor = 1;
        speed = 8;
        overallCostFactor = 0;
    }
}

