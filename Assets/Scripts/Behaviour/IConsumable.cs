using System;

public interface IConsumable
{

	double GetAmount();

	double Consume(double amount);

	ConsumptionType GetConsumptionType();
}
