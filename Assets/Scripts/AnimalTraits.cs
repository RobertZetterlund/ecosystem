using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
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
        public FCMHandler fcmHandler;

        public String[] diet;
        public String[] foe;
        public String[] mate;


        public AnimalTraits(Species species, double maxSize, double dietFactor, int nChildren, double infantFactor, double growthFactor, double speed, double heatTimer, FCMHandler fcmHandler, String[] diet, String[] foe, String[] mate)
        {
            this.species = species;
            this.maxSize = maxSize;
            this.dietFactor = dietFactor;
            this.nChildren = nChildren;
            this.infantFactor = infantFactor;
            this.growthFactor = growthFactor;
            this.speed = speed;
            this.heatTimer = heatTimer;
            this.fcmHandler = fcmHandler;
            this.diet = diet;
            this.foe = foe;
            this.mate = mate;
        }

        public (double, string)[] GetNumericalTraits()
        {
            (double, string)[] traits = new (double, string)[7];
            traits[0] = (maxSize, "max size");
            traits[1] = (dietFactor, "diet factor");
            traits[2] = ((double)nChildren, "#children");
            traits[3] = (infantFactor, "infant factor");
            traits[4] = (growthFactor, "growth factor");
            traits[5] = (speed, "speed");
            traits[6] = (heatTimer, "heat Timer");

            return traits;
        }
        // but we probably wont need this method later if we  randomize different traits for
        // each animal
        public AnimalTraits Duplicate()
        {
            AnimalTraits copy = new AnimalTraits(species, maxSize, dietFactor, nChildren, infantFactor, growthFactor, speed, heatTimer, FCMHandlerFactory.getFCMHandlerSpecies(FCMFactory.getSpeciesFCM(this.species), this.species), this.diet, this.foe, this.mate);
            return copy;
        }
    }



}
