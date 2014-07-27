using System;
using System.Linq;
using UnityEngine;

namespace DynamicTanks
{
    public class USI_AsteroidDrill : PartModule
    {
        [KSPField]
        public float maxPercentHollow = .5f;
        [KSPField]
        public string latchAnimationName = "Laser";
        [KSPField] 
        public string drillAnimationName = "ActivateLaser";
        [KSPField(guiActive = true, guiName = "Remaining Rock")]
        public double maxRock = 0f;
        [KSPField(guiActive = true, guiName = "Asteroid Size")]
        public string potatoSize = "N/A";

        private bool _isLatched;
        private bool _isDrilling;

        private Part _potato;
        private PartResource _rock;
        private USI_DynamicTank _tank;
        private ModuleGenerator _generator;

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
                _rock != null
                && _tank != null
                && _generator != null;
        }

        private bool IsConnected()
        {
            FindPotato();
            return _potato != null;
        }

        public override void OnStart(PartModule.StartState state)
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
            CheckForLatching();
            CheckForDrilling();
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
                if (potatoes.Any())
                {
                    if (_potato == null)
                    {
                        _potato = potatoes.FirstOrDefault();
                        double rock = _potato.mass*maxPercentHollow*200;
                        maxRock = Math.Round(rock*0.01, 0)*100;
                        potatoSize = _potato.mass + "t";
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
                if (part.Resources.Contains("Rock"))
                {
                    _rock = part.Resources["Rock"];
                }
                if(part.Modules.Contains("DynamicTank"))
                {
                    _tank = part.Modules.OfType<USI_DynamicTank>().FirstOrDefault();
                }
                if (part.Modules.Contains("ModuleGenerator"))
                {
                    _generator = part.Modules.OfType<ModuleGenerator>().FirstOrDefault();
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