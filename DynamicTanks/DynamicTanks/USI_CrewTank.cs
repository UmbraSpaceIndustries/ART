using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DynamicTanks
{
    public class USI_CrewTank : PartModule
    {
        [KSPField]
        public string latchAnimationName = "Clamp";
        private bool _isLatched;


        public Animation LatchAnimation
        {
            get
            {
                return part.FindModelAnimators(latchAnimationName)[0];
            }
        }

        private StartState _state;
        private Part _potato;

        [KSPField(guiActive = true, guiName = "Status", guiActiveEditor = true)]
        public string status = "Unknown";

        public override void OnAwake()
        {
            FindPotato();
            base.OnAwake();
        }

        public override void OnStart(StartState state)
        {
            _state = state;
            FindPotato();
            LatchAnimation[latchAnimationName].layer = 2;
            base.OnStart(state);
        }

        public override void OnLoad(ConfigNode node)
        {
            FindPotato();
            base.OnLoad(node);
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
            CheckForLatching();
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
                    status = "Connected";
                    part.CrewCapacity = 4;
                    LatchAnimation[latchAnimationName].speed = 1;
                    LatchAnimation.Play(latchAnimationName);
                }
                else
                {
                    status = "Not Connected";
                    foreach (var c in part.protoModuleCrew)
                    {
                        FlightEVA.SpawnEVA(c.KerbalRef);
                    }
                    part.CrewCapacity = part.protoModuleCrew.Count();
                    LatchAnimation[latchAnimationName].speed = -1;
                    LatchAnimation[latchAnimationName].time = LatchAnimation[latchAnimationName].length;
                    LatchAnimation.Play(latchAnimationName);
                }
            }
        }

    }
}
