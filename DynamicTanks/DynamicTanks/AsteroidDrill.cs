using System;
using System.Linq;

namespace DynamicTanks
{
    public class AsteroidDrill : PartModule
    {
        [KSPField]
        public float maxPercentHollow = .5f;

        //[KSPField]
        //public float powerEfficiency = 10f;

        //[KSPField]
        //public float drillRate = 1f;


        [KSPField(guiActive = true, guiName = "Remaining Rock")]
        public double maxRock = 0f;

        [KSPField(guiActive = true, guiName = "Asteroid Size")]
        public string potatoSize = "N/A";

        private Part _potato;
        private PartResource _rock;
        private DynamicTank _tank;
        private ModuleGenerator _generator; 

        private bool AllParts()
        {
            return 
                _rock != null
                && _tank != null
                && _generator != null;
        }

        private bool IsConnected()
        {
            return _potato != null;
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            FindParts();
            FindPotato();
        }

        public override void OnAwake()
        {
            base.OnAwake();
            FindParts();
            FindPotato();
        }

        public override void OnUpdate()
        {
            if (AllParts())
            {
                if (!IsConnected())
                {
                    _rock.amount = 0;
                    potatoSize = "N/A";
                    maxRock = 0f;
                    FindPotato();
                }
                else
                {
                    AddSpace();
                }
            }
            else
            {
                FindParts();
            }
            base.OnUpdate();
        }

        private void AddSpace()
        {
            potatoSize = _potato.mass + "t"; 
            if (_rock.amount >= 1)
            {
                maxRock -= 1;
                _tank.maxCapacity += 1;
                _tank.availCapacity += 1;
                _rock.amount -= 1;
                _potato.mass -= 0.005f;
            }
        }

        private void FindPotato()
        {
            if (vessel != null)
            {
                var potatoes = vessel.Parts.Where(p => p.Modules.Contains("ModuleAsteroid"));
                ////TODO:  Multiple asteroid drilling support
                if (potatoes != null && potatoes.Any())
                {
                    _potato = potatoes.FirstOrDefault();
                    double rock = _potato.mass * maxPercentHollow * 200;
                    maxRock = Math.Round(rock * 0.01, 0) * 100;
                    potatoSize = _potato.mass + "t";
                }
            }
        }
        private void FindParts()
        {
            if (vessel != null)
            {
                if (part.Resources.Contains("Rock"))
                {
                    _rock = part.Resources["Rock"];
                }
                if(part.Modules.Contains("DynamicTank"))
                {
                    _tank = part.Modules.OfType<DynamicTank>().FirstOrDefault();
                }
                if (part.Modules.Contains("ModuleGenerator"))
                {
                    _generator = part.Modules.OfType<ModuleGenerator>().FirstOrDefault();
                } 
            }
        }
    }
}