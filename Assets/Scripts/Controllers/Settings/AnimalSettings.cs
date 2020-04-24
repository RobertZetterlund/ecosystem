using System;

[Serializable]
public class AnimalSettings
{
	public bool randomiseFCM = true;
	public double timeToDeathByThirst = 70;
	public double BiteFactor = 0.25; // use to calculate how much you eat in one bite
	public double AdultSizeFactor = 0.3; // how big you have to be to mate
	public double lifespan = 80;

	public double maxSize = 3;
	public double dietFactor;
	public double nChildren = 2;
	public double speed;
	public double heatTimer = 13;
	public double sightLength = 30;
	public double smellRadius = 20;
}

