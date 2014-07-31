namespace DynamicTanks
{
    public class USI_PotatoInfo : PartModule
    {
        [KSPField]
        public float maxPercentHollow = .5f;

        [KSPField(isPersistant = true, guiActive = true, guiName = "Remaining Rock")]
        public float maxRock;

        [KSPField(guiActive = true, guiName = "Asteroid Size")]
        public string potatoSize = "N/A";

        [KSPField(isPersistant = true)] 
        public bool Explored;
    }
}