namespace DynamicTanks
{
    public class USI_PotatoResource : PartModule
    {
        [KSPField(isPersistant = true)] 
        public string resourceName = "";

        [KSPField(isPersistant = true)]
        public float resourceRate = 0;

        [KSPField(isPersistant = true)]
        public bool analysisComplete = false;
        
        [KSPField(guiActive = false, guiName = "", guiActiveEditor = false)]
        public string status = "Unknown";

        public override void OnUpdate()
        {
            if (resourceRate > 0 && Fields["status"].guiActive == false)
            {
                Fields["status"].guiActive = true;
                Fields["status"].guiName = resourceName;
                status = resourceRate + ":1";
            }
        }
    }
}