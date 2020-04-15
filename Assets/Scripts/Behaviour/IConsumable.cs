using System;

public interface IConsumable
{

	double GetAmount();

	double GetSpeed();

	double Consume(double amount);
  
	ConsumptionType GetConsumptionType();
}
