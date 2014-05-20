using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DynamicTanks
{
    public class DynamicTank : PartModule
    {
        [KSPField(isPersistant = true)]
        public int maxCapacity = 0;

        [KSPField(isPersistant = true)]
        public int availCapacity = 0;

        [KSPField(isPersistant = true)]
        public int stepSize = 100;

        [KSPField(guiActive = true, guiName = "Empty/Total Space", guiActiveEditor = true)]
        public string status = "Unknown";

        public override void OnStart(PartModule.StartState state)
        {
            status = String.Format("({0}/{1})", availCapacity, maxCapacity);
            base.OnStart(state);
        }
        public override void OnUpdate()
        {
            status = String.Format("({0}/{1})", availCapacity,maxCapacity);            
            base.OnUpdate();
        }
    }
}
