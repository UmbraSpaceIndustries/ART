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

        [KSPField]
        public int crewCap = 6;

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
            if (!_isLatched) DumpCrew();
        }

        private void DumpCrew()
        {
            if (part.protoModuleCrew.Any())
            {
                try
                {
                    var c = part.protoModuleCrew.First();
                    FlightEVA.SpawnEVA(c.KerbalRef);
                }
                catch (Exception)
                {
                    print("[ART] problem removing crewmember");
                }
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
                    status = "Connected";
                    part.CrewCapacity = crewCap;
                    LatchAnimation[latchAnimationName].speed = 1;
                    LatchAnimation.Play(latchAnimationName);
                }
                else
                {
                    part.CrewCapacity = 0;
                    LatchAnimation[latchAnimationName].speed = -1;
                    LatchAnimation[latchAnimationName].time = LatchAnimation[latchAnimationName].length;
                    LatchAnimation.Play(latchAnimationName);
                }
            }
        }

    }
}
