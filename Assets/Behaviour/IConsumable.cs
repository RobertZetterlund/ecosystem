using System;

public interface IConsumable
{

    double GetAmount();

    void DecreaseAmount(double amount);

    ConsumptionType GetConsumptionType();
}
