using System;
using System.Linq;
using UnityEngine;

namespace DynamicTanks
{
    public class USI_AsteroidDrill : PartModule
    {
        [KSPField]
        public string latchAnimationName = "Laser";
        [KSPField] 
        public string drillAnimationName = "ActivateLaser";
        
        private bool _isLatched;
        private bool _isDrilling;
        private Part _potato;
        private PartResource _moltenRock;
        private ModuleGenerator _generator;
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

        public Animation DrillAnimation
        {
            get
            {
                return part.FindModelAnimators(drillAnimationName)[0];
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
            DrillAnimation[drillAnimationName].layer = 3;
        }

        public override void OnLoad(ConfigNode node)
        {
            FindParts();
            FindPotato();
        }

        public override void OnAwake()
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
                _potatoInfo.maxRock -= 1;
                _moltenRock.amount -= 1;
                _tank.maxCapacity += 1;
                _rock.amount += 1;
                _rock.maxAmount += 1;
                _potato.mass -= 0.005f;
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
                        if (_potato.Modules.Contains("USI_PotatoInfo"))
                        {
                            _potatoInfo = _potato.Modules.OfType<USI_PotatoInfo>().FirstOrDefault();
                            if (!_potatoInfo.Explored)
                            {
                                double rock = _potato.mass*_potatoInfo.maxPercentHollow*200;
                                _potatoInfo.maxRock = Math.Round(rock*0.01, 0)*100;
                                _potatoInfo.Explored = true;
                            }
                            _potatoInfo.potatoSize = _potato.mass + "t";
                        }
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
        private void FindParts()
        {
            if (vessel != null)
            {
                if (part.Modules.Contains("ModuleGenerator"))
                {
                    _generator = part.Modules.OfType<ModuleGenerator>().FirstOrDefault();
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
                if (!_generator.generatorIsActive)
                {
                    expectedDrilling = false;
                }
            }            


            if (expectedDrilling != _isDrilling)
            {
                _isDrilling = expectedDrilling;
                if (_isDrilling)
                {
                    DrillAnimation[drillAnimationName].speed = 1;
                    DrillAnimation.Play(drillAnimationName);
                    var e = part.GetComponentsInChildren<KSPParticleEmitter>().FirstOrDefault();
                    if(e != null)
                    {
                        e.emit = true;
                        e.enabled = true;
                    }
                }
                else
                {
                    DrillAnimation[drillAnimationName].speed = -1;
                    DrillAnimation[drillAnimationName].time = DrillAnimation[drillAnimationName].length;
                    DrillAnimation.Play(drillAnimationName);
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