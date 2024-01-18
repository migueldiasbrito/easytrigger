using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Config
{
    [CreateAssetMenu(fileName = "PlatformConfig", menuName = "ScriptableObjects/PlatformConfig")]
    public class PlatformConfig : ScriptableObject, IPlatformConfig
    {
        [field: SerializeField] public LayerMask GroundLayerMask { get; private set; }
    }
}
