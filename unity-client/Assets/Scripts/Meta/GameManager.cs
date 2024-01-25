using Mdb.EasyTrigger.Character;
using Mdb.EasyTrigger.Config;
using Mdb.EasyTrigger.Input;
using UnityEngine;

namespace Mdb.EasyTrigger.Level.Meta
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private PlatformConfig _platformConfig;
        [SerializeField] private CharacterInputListener _playerPrefab;
        [SerializeField] private InputController _inputControllerPrefab;
        [SerializeField] private Campaign _singlePlayerCampaign;

        private void Start()
        {
            _playerPrefab.Setup(_inputControllerPrefab, _platformConfig, _singlePlayerCampaign);
            _singlePlayerCampaign.Setup(_platformConfig);
            _singlePlayerCampaign.AddPlayers(new CharacterView[] { _playerPrefab.View });

            _singlePlayerCampaign.StartCampaign();
        }
    }
}
