using System;
using System.Linq;
using UnityEngine;
using USI;

namespace DynamicTanks
{
    public class USI_AsteroidDrill : PartModule
    {
        [KSPField]
        public string latchAnimationName = "Latch";
        
        [KSPField] 
        public string primaryDrillAnimationName = "Laser";

        [KSPField]
        public string secondaryDrillAnimationName = "ActivateLaser";

        private bool _isLatched;
        private bool _isDrilling;
        private Part _potato;
        private PartResource _moltenRock;
        private USI_Converter _generator;
        private USI_PotatoInfo _potatoInfo;
        private PartResource _rock;
        private USI_DynamicTank _tank;

        public Animation LatchAnimation 
        {
            get
            {
                return part.FindModelAnimators(latchAnimationName)[0];
            }
        }

        public Animation PrimaryDrillAnimation
        {
            get
            {
                return part.FindModelAnimators(primaryDrillAnimationName)[0];
            }
        }

        public Animation SecondaryDrillAnimation
        {
            get
            {
                return part.FindModelAnimators(secondaryDrillAnimationName)[0];
            }
        }

        private bool AllParts()
        {
            return
                _moltenRock != null
                && _generator != null;
        }

        private bool IsConnected()
        {
            FindPotato();
            return _potato != null;
        }

        public override void OnStart(StartState state)
        {
            FindParts();
            FindPotato();
            LatchAnimation[latchAnimationName].layer = 2;
            PrimaryDrillAnimation[primaryDrillAnimationName].layer = 3;
            SecondaryDrillAnimation[secondaryDrillAnimationName].layer = 4;
        }

        public override void OnLoad(ConfigNode node)
        {
            FindParts();
            FindPotato();
        }

        public override void OnUpdate()
        {
            if (AllParts())
            {
                if (!IsConnected())
                {
                    _moltenRock.amount = 0;
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
            CheckForLatching();
            CheckForDrilling();
            base.OnUpdate();
        }

        private void AddSpace()
        {
            if (_moltenRock.amount >= 1 && _potatoInfo.maxRock >= 1)
            {
                var rockAmount = (int)Math.Min(Math.Floor(_moltenRock.amount), Math.Floor(_potatoInfo.maxRock));
                _potatoInfo.maxRock -= rockAmount;
                _moltenRock.amount -= rockAmount;
                _tank.maxCapacity += rockAmount;
                _rock.amount += rockAmount;
                _rock.maxAmount += rockAmount;
                _potato.mass -= (0.00025f * rockAmount);
                _potatoInfo.potatoSize = _potato.mass + "t"; 
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
                        if (_potato.Modules.Contains("USI_DynamicTank"))
                        {
                            _tank = _potato.Modules.OfType<USI_DynamicTank>().FirstOrDefault();
                        }
                        else
                        {
                            _potato = null;
                            return;
                        }
                        if (_potato.Modules.Contains("USI_PotatoInfo"))
                        {
                            _potatoInfo = _potato.Modules.OfType<USI_PotatoInfo>().FirstOrDefault();
                            if (!_potatoInfo.Explored)
                            {
                                float rock = _potato.mass/0.00025f *_potatoInfo.maxPercentHollow;
                                _potatoInfo.maxRock = (float)Math.Round(rock*0.01, 0)*100;
                                _potatoInfo.Explored = true;
                            }
                            _potatoInfo.potatoSize = _potato.mass + "t";
                        }
                        else
                        {
                            _potato = null;
                            return;
                        }

                        if (_potato.Resources.Contains("Rock"))
                        {
                            _rock = _potato.Resources["Rock"];
                        }
                        else
                        {
                            _potato = null;
                            return;
                        }
                    }
                    return;
                }
            }
            _potato = null;
        }
        private void FindParts()
        {
            if (vessel != null)
            {
                if (part.Modules.Contains("USI_Converter"))
                {
                    _generator = part.Modules.OfType<USI_Converter>().FirstOrDefault();
                    _generator.DeactivateConverter();
                }
                if (part.Resources.Contains("MoltenRock"))
                {
                    _moltenRock = part.Resources["MoltenRock"];
                }
            }
        }

        private void CheckForLatching()
        {
            //If we're connected, then we should be latched.
            bool expectedLatch = _potato != null;
            if (expectedLatch != _isLatched)
            {
                _isLatched = expectedLatch;
                if (_isLatched)
                {
                    LatchAnimation[latchAnimationName].speed = 1;
                    LatchAnimation.Play(latchAnimationName);
                }
                else
                {
                    LatchAnimation[latchAnimationName].speed = -1;
                    LatchAnimation[latchAnimationName].time = LatchAnimation[latchAnimationName].length;
                    LatchAnimation.Play(latchAnimationName);
                }
            }
        }

        private void CheckForDrilling()
        {
            var expectedDrilling = true;
            if (_potato == null)
            {
                expectedDrilling = false;
            }
            
            if (_generator == null)
            {
                expectedDrilling = false;
            }
            else
            {
                if (!_generator.converterEnabled)
                {
                    expectedDrilling = false;
                }
            }            


            if (expectedDrilling != _isDrilling)
            {
                _isDrilling = expectedDrilling;
                if (_isDrilling)
                {
                    PrimaryDrillAnimation[primaryDrillAnimationName].speed = 1;
                    PrimaryDrillAnimation.Play(primaryDrillAnimationName);
                    SecondaryDrillAnimation[secondaryDrillAnimationName].speed = 1;
                    SecondaryDrillAnimation.Play(secondaryDrillAnimationName); var e = part.GetComponentsInChildren<KSPParticleEmitter>().FirstOrDefault();
                    if(e != null)
                    {
                        e.emit = true;
                        e.enabled = true;
                    }
                }
                else
                {
                    PrimaryDrillAnimation[primaryDrillAnimationName].speed = -1;
                    PrimaryDrillAnimation[primaryDrillAnimationName].time = PrimaryDrillAnimation[primaryDrillAnimationName].length;
                    PrimaryDrillAnimation.Play(primaryDrillAnimationName);
                    SecondaryDrillAnimation[secondaryDrillAnimationName].speed = -1;
                    SecondaryDrillAnimation[secondaryDrillAnimationName].time = SecondaryDrillAnimation[secondaryDrillAnimationName].length;
                    SecondaryDrillAnimation.Play(secondaryDrillAnimationName);
                    var e = part.GetComponentsInChildren<KSPParticleEmitter>().FirstOrDefault();
                    if(e != null)
                    {
                        e.emit = false;
                        e.enabled = false;
                    }
                }
            }
        }
    }
}