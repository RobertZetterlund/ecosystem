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
    }


}
