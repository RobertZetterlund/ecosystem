using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class AnimalTraits
    {
        public Species species;
        public double maxSize;
        public double dietFactor;
        public int nChildren;
        public double infantFactor;
        public double growthFactor;
        public double speed;
        public double heatTimer;
        public double sightLength;
        public double smellRadius;
        public FCMHandler fcmHandler;

        public AnimalTraits(Species species, double maxSize, double dietFactor, int nChildren, double infantFactor, double growthFactor, double speed, double heatTimer, double sightLength, double smellRadius, FCMHandler fcmHandler)
        {
            this.species = species;
            this.maxSize = maxSize;
            this.dietFactor = dietFactor;
            this.nChildren = nChildren;
            this.infantFactor = infantFactor;
            this.growthFactor = growthFactor;
            this.speed = speed;
            this.heatTimer = heatTimer;
            this.sightLength = sightLength;
            this.smellRadius = smellRadius;
            this.fcmHandler = fcmHandler;

        }

        public (double, string)[] GetNumericalTraits()
        {
            (double, string)[] traits = new (double, string)[9];
            traits[0] = (maxSize, "max size");
            traits[1] = (dietFactor, "diet factor");
            traits[2] = ((double)nChildren, "#children");
            traits[3] = (infantFactor, "infant factor");
            traits[4] = (growthFactor, "growth factor");
            traits[5] = (speed, "speed");
            traits[6] = (heatTimer, "heat Timer");
            traits[7] = (sightLength, "sight length");
            traits[8] = (smellRadius, "smell radius");
            return traits;
        }
        // but we probably wont need this method later if we  randomize different traits for
        // each animal
        public AnimalTraits Duplicate()
        {
            AnimalTraits copy = new AnimalTraits(species, maxSize, dietFactor, nChildren, infantFactor, growthFactor, speed, heatTimer, sightLength, smellRadius, FCMHandlerFactory.getFCMHandlerSpecies(FCMFactory.getSpeciesFCM(this.species), this.species));
            return copy;
        }
    }


