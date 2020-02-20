using System;

public interface IConsumable
{

    double GetAmount();

    void Consume(double amount);

    ConsumptionType GetConsumptionType();
}
