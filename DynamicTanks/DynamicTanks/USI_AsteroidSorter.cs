using System.Collections.Generic;
using System.Linq;

namespace DynamicTanks
{
    public class USI_AsteroidSorter : PartModule
    {
        [KSPField]
        public float conversionRate = 0.25f;

        [KSPField]
        public float thresholdRate = 10f;

        private Part _potato;
        PartResource _rock;
        private List<USI_PotatoResource> _resources;

        private bool IsConnected()
        {
            FindPotato();
            return _potato != null;
        }

        public override void OnStart(PartModule.StartState state)
        {
            FindPotato();
        }

        public override void OnLoad(ConfigNode node)
        {
            FindPotato();
        }

        public override void OnAwake()
        {
            FindPotato();
        }

        public override void OnUpdate()
        {
            if (!IsConnected())
            {
                FindPotato();
            }
            CheckForRock();
            base.OnUpdate();
        }

        private void CheckForRock()
        {
            if (_rock != null && _resources != null)
            {
                foreach (var res in _resources)
                {
                    var thisRes = part.Resources[res.resourceName];
                    //If we have space
                    if (thisRes.maxAmount - thisRes.amount >= conversionRate)
                    {
                        //And there is enough rock
                        if (_rock.amount >= (res.resourceRate * thresholdRate))
                        {
                            //And we need some of this stuff
                            if (thisRes.amount < conversionRate)
                            {
                                //AutoConvert
                                _rock.amount -= (res.resourceRate*conversionRate);
                                thisRes.amount += conversionRate;
                            }
                        }
                    }
                }
            }
        }

        private void FindPotato()
        {
            if (vessel != null)
            {
                var potatoes = vessel.Parts.Where(p => p.Modules.Contains("ModuleAsteroid"));
                if (potatoes.Any())
                {
                    if (_potato == null)
                    {
                        _potato = potatoes.FirstOrDefault();
                    }
                    _rock = _potato.Resources["Rock"];
                    _resources = _potato.Modules.OfType<USI_PotatoResource>().Where(p => p.analysisComplete).ToList();
                    return;
                }
            }
            _potato = null;

        }
    }
}