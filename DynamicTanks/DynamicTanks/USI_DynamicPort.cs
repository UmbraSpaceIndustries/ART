using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicTanks
{
    public class USI_DynamicPort : PartModule
    {
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
                    if(_state == StartState.Editor)
                    {
                        _resource.amount += _stepSize;
                    }
                    _resource.maxAmount += _stepSize;
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

        private int _stepSize;
        private USI_DynamicTank _tank;
        private PartResource _resource;

        [KSPField(guiActive = true, guiName = "Tank Status", guiActiveEditor = true)]
        public string status = "Unknown";

        public override void OnStart(StartState state)
        {
            _state = state; 
            base.OnStart(state);
        }

        public override void OnAwake()
        {
            if (_tank == null) FindTank();
            if (part.Resources.Count > 0)
            {
                _resource = part.Resources[0];
            }
            base.OnAwake();
        }


        public override void OnLoad(ConfigNode node)
        {
            if (_tank == null) FindTank();
            if (part.Resources.Count > 0)
            {
                _resource = part.Resources[0];
            }
            base.OnLoad(node);
        }


        public void FindTank()
        {
            if (vessel != null)
            {
                var tanks = vessel.parts.Where(p => p.Modules.Contains("DynamicTank"));
                if (!tanks.Any())
                {
                    status = "Not connected";
                    return;
                }

                foreach (var p in tanks)
                {
                    var t = p.Modules.OfType<USI_DynamicTank>().First();
                    if (t.availCapacity > 0)
                    {
                        _tank = t;
                        _stepSize = _tank.stepSize;
                        status = string.Format("{0} avail", _tank.availCapacity);
                    }
                }
            }
        }
        public override void OnUpdate()
        {
            if (_tank == null)
            {
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
                }
            }
        }


    }
}
