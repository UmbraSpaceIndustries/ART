namespace DynamicTanks
{
    public class USI_ProbeData : PartModule
    {
        [KSPField]
        public string resourceName;

        [KSPField]
        public int presenceChance;

        [KSPField]
        public int lowRange;

        [KSPField]
        public int highRange;

    }
}