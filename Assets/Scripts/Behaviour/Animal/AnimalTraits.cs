﻿using System;

public class AnimalTraits
{

	public Species species;
	public RangedDouble maxSize;
	public RangedDouble dietFactor;
	public RangedInt nChildren;
	public RangedDouble infantFactor;
	public RangedDouble growthFactor;
	public RangedDouble speed;
	public RangedDouble heatTimer;
	public RangedDouble sightLength;
	public RangedDouble smellRadius;
	public FCMHandler fcmHandler;
	public string[] diet;
	public string[] foes;
	public string[] mates;

	public AnimalTraits(Species species, double maxSize, double dietFactor, int nChildren, double infantFactor, double growthFactor, double speed, double heatTimer, double sightLength, double smellRadius, FCMHandler fcmHandler, String[] diet, String[] foes, String[] mates)
	{
		this.species = species;
		this.maxSize = new RangedDouble(maxSize, 0);
		this.dietFactor = new RangedDouble(dietFactor, 0, 1);
		this.nChildren = new RangedInt(nChildren, 1);
		this.infantFactor = new RangedDouble(infantFactor, 0, 1);
		this.growthFactor = new RangedDouble(growthFactor, 0, 1);
		this.speed = new RangedDouble(speed, 0);
		this.heatTimer = new RangedDouble(heatTimer, 1);
		this.sightLength = new RangedDouble(sightLength, 0);
		this.smellRadius = new RangedDouble(smellRadius, 0);
		this.fcmHandler = fcmHandler;
		this.diet = diet;
		this.foes = foes;
		this.mates = mates;
	}

 
    public AnimalTraits(AnimalTraits traits)
{
		this.species = traits.species;
		this.maxSize = traits.maxSize;
		this.dietFactor = traits.dietFactor;
		this.nChildren = traits.nChildren;
		this.infantFactor = traits.infantFactor;
		this.growthFactor = traits.growthFactor;
		this.speed = traits.speed;
		this.heatTimer = traits.heatTimer;
		this.sightLength = traits.sightLength;
		this.smellRadius = traits.smellRadius;
		this.fcmHandler = traits.fcmHandler;
		this.diet = traits.diet;
		this.foes = traits.foes;
		this.mates = traits.mates;
	}

	public (double, string)[] GetNumericalTraits()
	{
		(double, string)[] traits = new (double, string)[7];
		traits[0] = (maxSize.GetValue(), "max size");
		traits[1] = (dietFactor.GetValue(), "diet factor");
		traits[2] = ((double)nChildren.GetValue(), "#children");
		traits[3] = (infantFactor.GetValue(), "infant factor");
		traits[4] = (growthFactor.GetValue(), "growth factor");
		traits[5] = (speed.GetValue(), "speed");
		traits[6] = (heatTimer.GetValue(), "heat Timer");

		return traits;
	}
	// but we probably wont need this method later if we  randomize different traits for
	// each animal
	public AnimalTraits Duplicate()
	{
		AnimalTraits copy = new AnimalTraits(species, maxSize.GetValue(), dietFactor.GetValue(), nChildren.GetValue(), infantFactor.GetValue(), growthFactor.GetValue(), speed.GetValue(), heatTimer.GetValue(), sightLength.GetValue(), smellRadius.GetValue(), FCMHandlerFactory.getFCMHandlerSpecies(FCMFactory.getSpeciesFCM(this.species), this.species), this.diet, this.foes, this.mates);
		return copy;
	}
}