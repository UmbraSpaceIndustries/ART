using System;
using System.Linq;
using UnityEngine;

namespace DynamicTanks
{
    public class USI_AsteroidTank : USI_DynamicPort
    {
        [KSPField]
        public string latchAnimationName = "Clamp";
        private bool _isLatched;

        private Part _potato;

        public Animation LatchAnimation
        {
            get
            {
                return part.FindModelAnimators(latchAnimationName)[0];
            }
        }

        public override void OnAwake()
        {
            FindPotato();
            base.OnAwake();
        }

        public override void OnLoad(ConfigNode node)
        {
            FindPotato();
            base.OnLoad(node);
        }

        public override void OnStart(StartState state)
        {
            FindPotato();
            LatchAnimation[latchAnimationName].layer = 2;
            base.OnStart(state);
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
                    return;
                }
            }
            _potato = null;
        }

        public override void OnUpdate()
        {
            try
            {
                CheckForLatching();
                base.OnUpdate();
            }
            catch (Exception ex)
            {
                print("[HA] Error in USI_AsteroidTank OnUpdate: " + ex.Message);
            }
        }

        private void CheckForLatching()
        {
            FindPotato();
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

    }
}