using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicTanks
{
    public class USI_DynamicPort : PartModule
    {
        [KSPField]
        public string ConnectedModule = "";

        private StartState _state;

        [KSPEvent(guiActive = true, guiName = "Expand Tank", active = true)]
        public void AddSpace()
        {
            if (_tank != null && _resource != null)
            {
                if (_tank.maxCapacity == 0 || _tank.availCapacity == 0) FindTank();
                if (_tank.availCapacity >= _stepSize)
                {
                    _tank.availCapacity -= _stepSize;
                    if (_state == StartState.Editor)
                    {
                        _resource.amount += _stepSize;
                    }
                    _resource.maxAmount += _stepSize;
                    tankSize += _stepSize;
                }
            }
        }

        [KSPEvent(guiActive = true, guiName = "Compress", active = true)]
        public void RemoveSpace()
        {
            if (_tank != null && _resource != null)
            {
                var usedSpace = _tank.maxCapacity - _tank.availCapacity;
                if (usedSpace >= _stepSize && part.Resources[0].maxAmount >= _stepSize)
                {
                    _tank.availCapacity += _stepSize;
                    _resource.maxAmount -= _stepSize;
                    if (_state == StartState.Editor)
                    {
                        _resource.amount -= _stepSize;
                        tankSize -= _stepSize;
                    }
                }
            }
        }

        [KSPEvent(guiActive = true, guiName = "Dump", active = true)]
        public void DumpContents()
        {
            if (_tank != null && _resource != null)
            {
                var dumpAmount = _resource.amount % _stepSize;
                if (dumpAmount == 0) dumpAmount = _stepSize;

                if (_resource.amount >= dumpAmount)
                {
                    _resource.amount -= dumpAmount;
                }
                else
                {
                    _resource.amount = 0;
                }
            }
        }

        [KSPField(isPersistant = true)]
        public float tankSize;

        private int _stepSize;
        private USI_DynamicTank _tank;
        private PartResource _resource;
        private bool _curState;

        [KSPField(guiActive = true, guiName = "Tank Status", guiActiveEditor = true)]
        public string status = "Unknown";

        public override void OnStart(StartState state)
        {
            _state = state;
            _curState = true;
            base.OnStart(state);
        }

        public override void OnAwake()
        {
            try
            {
                if (vessel != null)
                {
                    if (_tank == null) FindTank();
                    if (part.Resources.Count > 0)
                    {
                        _resource = part.Resources[0];
                        if (tankSize > _resource.maxAmount)
                        {
                            _resource.maxAmount = tankSize;
                        }
                    }
                }
                base.OnAwake();
            }
            catch (Exception ex)
            {
                print("[HA] Error in USI_DynamicPort OnAwake - " + ex.Message);
            }
        }


        public override void OnLoad(ConfigNode node)
        {
            if (vessel != null)
            {
                if (_tank == null) FindTank();
                if (part.Resources.Count > 0)
                {
                    _resource = part.Resources[0];
                }
            }
            base.OnLoad(node);
        }


        public void FindTank()
        {
            if (vessel != null)
            {
                var tankParts = vessel.parts.Where(p => p.Modules.Contains("USI_DynamicTank"));
                if (!tankParts.Any())
                {
                    status = "Not connected";
                    return;
                }
                foreach (var tankPart in tankParts)
                {
                    var t = tankPart.Modules.OfType<USI_DynamicTank>().First();
                    if (t == null)
                    {
                        return;
                    }
                    if (_tank == null)
                    {
                        _tank = t;
                        _stepSize = _tank.stepSize;
                        status = string.Format("{0} avail", _tank.availCapacity);
                    }
                    //Always go to the largest tank
                    else if (t.availCapacity > _tank.availCapacity)
                    {
                        _tank = t;
                        _stepSize = _tank.stepSize;
                        status = string.Format("{0} avail", _tank.availCapacity);
                    }
                }
            }
        }

        private bool CheckForConnection()
        {
            var isConnected = false;
            if (ConnectedModule == "")
            {
                isConnected = true;
            }
            else if (vessel != null)
            {
                isConnected = vessel.Parts.Any(p => p.Modules.Contains(ConnectedModule));
            }
            else
            {
                isConnected = false;
            }
            return isConnected;
        }

        public override void OnUpdate()
        {
            try
            {
                var isActive = CheckForConnection();
                if (isActive != _curState)
                {
                    _curState = isActive;

                    Events["AddSpace"].active = isActive;
                    Events["RemoveSpace"].active = isActive;
                    Events["DumpContents"].active = isActive;
                    RefreshContextWindows();
                    if (!isActive)
                    {
                        //Dump contenta
                        if (part.Resources.Count > 0)
                        {
                            _resource = part.Resources[0];
                            _resource.maxAmount = 0;
                            _resource.amount = 0;
                        }
                        return;
                    }
                }
                if (_tank == null)
                {
                    status = "cannot find tank";
                    FindTank();
                }
                else
                {
                    status = string.Format("{0} avail", _tank.availCapacity);
                }
                if (_resource == null)
                {
                    if (part.Resources.Count > 0)
                    {
                        _resource = part.Resources[0];
                        if (_resource.maxAmount < tankSize)
                        {
                            _resource.maxAmount = tankSize;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                print("[HA] Error in USI_DynamicPort OnUpdate - " + ex.Message);
            }
        }

        public void RefreshContextWindows()
        {
            foreach (var o in FindObjectsOfType(typeof(UIPartActionWindow)))
            {
                var window = (UIPartActionWindow) o;
                if (window.part == part)
                {
                    window.displayDirty = true;
                }
            }
        }

    }
}
