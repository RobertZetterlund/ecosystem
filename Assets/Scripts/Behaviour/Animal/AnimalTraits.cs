﻿using System;

public class AnimalTraits
{

	public Species species;
	public RangedDouble maxSize;
	public RangedDouble dietFactor;
	public RangedDouble nChildren;
	public RangedDouble speed;
	public RangedDouble heatTimer;
	public RangedDouble sightLength;
	public RangedDouble smellRadius;
	public FCMHandler fcmHandler;
	public string[] diet;
	public string[] foes;
	public string[] mates;


	public AnimalTraits(Species species, double maxSize, double dietFactor, double nChildren, double speed, double heatTimer, double sightLength, double smellRadius, FCMHandler fcmHandler, String[] diet, String[] foes, String[] mates)
	{
		this.species = species;
		this.maxSize = new RangedDouble(maxSize, 0);
		this.dietFactor = new RangedDouble(dietFactor, 0, 1);
		this.nChildren = new RangedDouble(nChildren, 0);
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
		traits[3] = (speed.GetValue(), "speed");
		traits[4] = (heatTimer.GetValue(), "heat Timer");
		traits[5] = (sightLength.GetValue(), "sight length");
		traits[6] = (smellRadius.GetValue(), "smell radius");

		return traits;
	}
	// but we probably wont need this method later if we  randomize different traits for
	// each animal
	public AnimalTraits Duplicate()
	{
		AnimalTraits copy = new AnimalTraits(species, maxSize.GetValue(), dietFactor.GetValue(), nChildren.GetValue(), speed.GetValue(), heatTimer.GetValue(), sightLength.GetValue(), smellRadius.GetValue(), FCMHandlerFactory.getFCMHandlerSpecies(FCMFactory.getSpeciesFCM(this.species), this.species), this.diet, this.foes, this.mates);
		return copy;
	}
}