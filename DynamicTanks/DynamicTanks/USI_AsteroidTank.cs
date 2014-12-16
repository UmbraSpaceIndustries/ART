using System;
using System.Linq;
using DynamicTanks;

namespace DynamicTanks
{

    public class USI_AsteroidTank : PartModule
    {   
        [KSPEvent(guiActive = true, guiName = "Vent Rock", active = true)]
        public void DumpContents()
        {
            if (_tank != null && _potato != null)
            {
                var dumpAmount = _rock.amount % 1000;
                if (dumpAmount == 0) dumpAmount = 1000;

                if (_rock.amount >= dumpAmount)
                {
                    _rock.amount -= dumpAmount;
                }
                else
                {
                    _rock.amount = 0;
                }
            }
        }

        [KSPEvent(guiActive = true, guiName = "Convert Space", active = true)]
        public void ConvertSpace()
        {
            if (_tank != null && _potato != null)
            {
                var spaceAvail = (int)Math.Floor(_rock.maxAmount - _rock.amount);
                print("space avail: " + spaceAvail);
                if (spaceAvail > 0)
                {
                    _rock.maxAmount -= spaceAvail;
                    _tank.availCapacity += spaceAvail;
                }
            }
        }


        private Part _potato;
        private PartResource _rock;
        private USI_DynamicTank _tank;

        private bool IsConnected()
        {
            FindPotato();
            return _potato != null;
        }

        public override void OnStart(StartState state)
        {
            try
            {
                FindPotato();
            }
            catch (Exception ex)
            {
                print("[HA] Error in USI_AsteroidTank OnStart: " + ex.Message);
            }

        }

        public override void OnLoad(ConfigNode node)
        {
            try
            {
                FindPotato();
            }
            catch (Exception ex)
            {
                print("[HA] Error in USI_AsteroidTank OnLoad: " + ex.Message);
            }
        }

        public override void OnAwake()
        {
            try
            {
                FindPotato();
            }
            catch (Exception ex)
            {
                print("[HA] Error in USI_AsteroidTank OnAwake: " + ex.Message);
            }
        }

        public override void OnUpdate()
        {
            if (!IsConnected())
            {
                FindPotato();
            }
            base.OnUpdate();
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
                        if (_potato.Modules.Contains("USI_DynamicTank"))
                        {
                            _tank = _potato.Modules.OfType<USI_DynamicTank>().FirstOrDefault();
                        }
                        if (_potato.Resources.Contains("Rock"))
                        {
                            _rock = _potato.Resources["Rock"];
                        }
                    }
                    return;
                }
            }
            _potato = null;
        }
    }
}
