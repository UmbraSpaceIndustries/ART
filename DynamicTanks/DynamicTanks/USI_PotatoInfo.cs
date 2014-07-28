namespace DynamicTanks
{
    public class USI_PotatoInfo : PartModule
    {
        [KSPField]
        public float maxPercentHollow = .5f;

        [KSPField(guiActive = true, guiName = "Remaining Rock", isPersistant = true)]
        public double maxRock = 0f;

        [KSPField(guiActive = true, guiName = "Asteroid Size")]
        public string potatoSize = "N/A";

        [KSPField] 
        public bool Explored = false;
    }
}