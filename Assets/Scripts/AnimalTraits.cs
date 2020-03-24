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

        public AnimalTraits(Species species, double maxSize, double dietFactor, int nChildren, double infantFactor, double growthFactor, double speed, double heatTimer, FCMHandler fcmHandler)
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

        public AnimalTraits copyMe(){
            return new AnimalTraits(this.species, this.maxSize, this.dietFactor, this.nChildren, this.infantFactor, this.growthFactor, this.speed, this.heatTimer, FCMHandlerFactory.getFCMHandlerSpecies(FCMFactory.getSpeciesFCM(this.species), this.species));        
        }
    }


}
