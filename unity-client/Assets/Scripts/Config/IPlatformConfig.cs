using UnityEngine;

namespace Mdb.EasyTrigger.Config
{
    public interface IPlatformConfig
    {
        LayerMask GroundLayerMask { get; }
        LayerMask PlatformLayerMask { get; }
    }
}
