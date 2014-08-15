using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace DynamicTanks
{
    public class USI_AsteroidProbe : PartModule
    {
        [KSPField]
        public string latchAnimationName = "Process";

        [KSPEvent(guiActive = true, guiName = "Perform Analysis", active = false, guiActiveUnfocused = false, guiActiveEditor = false, externalToEVAOnly = true)]
        public void AnalysisEvent()
        {
            if (_potato != null)
            {
                RunAnalysis();
            }
        }

        private bool _isLatched;

        private Part _potato;

        public Animation LatchAnimation 
        {
            get
            {
                return part.FindModelAnimators(latchAnimationName)[0];
            }
        }

        private bool IsConnected()
        {
            FindPotato();
            return _potato != null;
        }

        public override void OnStart(PartModule.StartState state)
        {
            FindPotato();
            LatchAnimation[latchAnimationName].layer = 3;
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
            CheckForLatching();
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
                    }
                    return;
                }
            }
            _potato = null;
        }

        private void CheckForLatching()
        {
            //If we're connected, then we should be latched.
            bool expectedLatch = _potato != null;
            if (expectedLatch != _isLatched)
            {
                setAnalysis();
            }
        }

        private void setAnalysis()
        {
            Events["AnalysisEvent"].active = _potato != null;
        }

        private void RunAnalysis()
        {
            var r = new Random();
            if (_potato != null)
            {
                var makeupInfo = vessel.Parts.Where(p => p.Modules.Contains("USI_PotatoResource") && p != part);
                if (makeupInfo.Any())
                {
                    var resList = _potato.Modules.OfType<USI_PotatoResource>().Where(p => p.analysisComplete == false);
                    foreach (var res in resList)
                    {
                        var pi =
                            part.Modules.OfType<USI_ProbeData>().FirstOrDefault(p => p.resourceName == res.resourceName);
                        var thisres =
                            part.Modules.OfType<USI_PotatoResource>().FirstOrDefault(x => x.resourceName == res.resourceName);
                        if (pi != null && thisres != null)
                        {
                            res.analysisComplete = true;
                            if (r.Next(100) <= pi.presenceChance)
                            {
                                var rate = r.Next(pi.lowRange, pi.highRange);
                                res.resourceRate = rate;
                                thisres.resourceRate = rate;
                            }
                        }
                    }
                }
                LatchAnimation[latchAnimationName].speed = 1;
                LatchAnimation.Play(latchAnimationName);
            }
            _potato = null;
        }

    }
}