using System;
using System.Linq;
using DynamicTanks;

namespace DynamicTanks
{
    public class USI_AsteroidTank : PartModule
    {
        [KSPField(isPersistant = true)] 
        public double OriginalMass;


        [KSPField(guiActive = true, guiName = "Excavated", guiActiveEditor = true)]
        public string totSpace = "Unknown";


        private ModuleAsteroid _potato;
        private USI_DynamicTank _tank;

        public void FixedUpdate()
        {
            if(_potato != null 
                && _tank != null)
            {
                double LDiff = Math.Floor((OriginalMass - _potato.part.mass) * _potato.density * 1000);
                totSpace = String.Format("{0:0.000}t", OriginalMass - _potato.part.mass);
                int netDiff = Convert.ToInt32(Math.Floor(LDiff - _tank.maxCapacity));

                if (netDiff > 0)
                {
                    _tank.maxCapacity += netDiff;
                    _tank.availCapacity += netDiff;
                }
            }
            else
            {
                FindPotato();
            }
        }

        private void FindPotato()
        {
            _potato = part.FindModulesImplementing<ModuleAsteroid>().FirstOrDefault();
            _tank = part.FindModulesImplementing<USI_DynamicTank>().FirstOrDefault();
            if (OriginalMass < ResourceUtilities.FLOAT_TOLERANCE)
            {
                OriginalMass = _potato.part.mass;
            }
        }
    }
}
