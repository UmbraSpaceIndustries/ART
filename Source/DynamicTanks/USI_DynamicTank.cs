using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DynamicTanks
{
    public class USI_DynamicTank : PartModule
    {
        [KSPField(isPersistant = true)]
        public int maxCapacity = 0;

        [KSPField(isPersistant = true)]
        public int availCapacity = 0;

        [KSPField(isPersistant = true)]
        public int stepSize = 1000;

        [KSPField(guiActive = true, guiName = "Total Space", guiActiveEditor = true)]
        public string totSpace = "Unknown";

        [KSPField(guiActive = true, guiName = "Available Space", guiActiveEditor = true)]
        public string avSpace = "Unknown";

        public void FixedUpdate()
        {
            totSpace = String.Format("({0}m3)", (float)maxCapacity / 1000f);
            avSpace = String.Format("({0}m3)", (float)availCapacity / 1000f);
        }
    }
}
