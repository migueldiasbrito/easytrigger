using UnityEngine;

namespace Mdb.EasyTrigger.Config
{
    [CreateAssetMenu(fileName = "PlatformConfig", menuName = "ScriptableObjects/PlatformConfig")]
    public class PlatformConfig : ScriptableObject, IPlatformConfig
    {
        [field: SerializeField] public LayerMask GroundLayerMask { get; private set; }

        [field: SerializeField] public LayerMask PlatformLayerMask { get; private set; }
    }
}
