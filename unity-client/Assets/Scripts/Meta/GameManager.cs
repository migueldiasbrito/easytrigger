using Mdb.EasyTrigger.Character;
using Mdb.EasyTrigger.Config;
using Mdb.EasyTrigger.Input;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mdb.EasyTrigger.Level.Meta
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private PlatformConfig _platformConfig;
        [SerializeField] private CharacterInputListener _playerPrefab;
        [SerializeField] private InputController _inputControllerPrefab;
        [SerializeField] private Campaign _singlePlayerCampaign;
        [SerializeField] private Campaign _multiPlayerCampaign;
        [SerializeField] private Canvas _waitForPlayersCanvas;
        [SerializeField] private TMP_Text _waitForPlayersText;
        [SerializeField] private PlayerInputManager _playerInputManager;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private Canvas _mainMenu;
        [SerializeField] private TMP_Text _text;

        private List<CharacterInputListener> _players = new List<CharacterInputListener>();
        private List<InputController> _inputControllers = new List<InputController>();

        private float _cameraInitialPosition;

        public void OnSingleCampaignStart()
        {
            ClearCampaigns();

            _cameraTransform.position = new Vector3(_cameraInitialPosition, _cameraTransform.position.y,
                _cameraTransform.position.z);
            _mainMenu.gameObject.SetActive(false);

            StartSinglePlayerCampaign();
        }

        private void StartSinglePlayerCampaign()
        {
            CharacterInputListener player = Instantiate(_playerPrefab, _singlePlayerCampaign.StartPoints[0],
                Quaternion.identity);
            InputController inputController = Instantiate(_inputControllerPrefab);
            _players.Add(player);
            _inputControllers.Add(inputController);

            player.Setup(inputController, _platformConfig, _singlePlayerCampaign);

            _singlePlayerCampaign.AddPlayers(new CharacterView[] { player.View });
            _singlePlayerCampaign.StartCampaign();
        }

        public void OnMultiCampaignStart()
        {
            ClearCampaigns();

            _cameraTransform.position = new Vector3(_cameraInitialPosition, _cameraTransform.position.y,
                _cameraTransform.position.z);
            _mainMenu.gameObject.SetActive(false);

            StartMultiPlayerCampaign();
        }

        private void StartMultiPlayerCampaign()
        {
            _waitForPlayersText.text = "Waiting for players... (0 / 2)\nPress any key to join";
            _waitForPlayersCanvas.gameObject.SetActive(true);
            _playerInputManager.EnableJoining();

            _multiPlayerCampaign.StartCampaign();
        }

        private void ClearCampaigns()
        {
            _singlePlayerCampaign.Clear();
            _multiPlayerCampaign.Clear();

            //_playerInputManager.

            _players.ForEach(player => Destroy(player.gameObject));
            _players.Clear();

            _inputControllers.ForEach(input => Destroy(input.gameObject));
            _inputControllers.Clear();
        }

        public void OnExit()
        {
            Application.Quit();
        }

        public void OnCancelMultiPlayerCampaign()
        {
            _playerInputManager.DisableJoining();
            _waitForPlayersCanvas.gameObject.SetActive(false);
            _mainMenu.gameObject.SetActive(true);
        }

        public void OnPlayerJoined(PlayerInput newPlayer)
        {
            _inputControllers.Add(newPlayer.GetComponent<InputController>());
            _waitForPlayersText.text = $"Waiting for players... ({_inputControllers.Count} / 2)\nPress any key to join";

            if (_inputControllers.Count >= 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    CharacterInputListener player = Instantiate(_playerPrefab, _multiPlayerCampaign.StartPoints[i],
                        Quaternion.identity);
                    _players.Add(player);
                    player.Setup(_inputControllers[i], _platformConfig, _multiPlayerCampaign);
                }

                _multiPlayerCampaign.AddPlayers(_players.Select(player => player.View).ToArray());
                _waitForPlayersCanvas.gameObject.SetActive(false);
                _playerInputManager.DisableJoining();
            }
        }

        private void Start()
        {
            _cameraInitialPosition = _cameraTransform.position.x;
            _mainMenu.gameObject.SetActive(true);

            _singlePlayerCampaign.Setup(_platformConfig, _cameraTransform, OnWin, OnGameOver);
            _multiPlayerCampaign.Setup(_platformConfig, _cameraTransform, OnWin, OnGameOver);
            
            int charactersLayer = _playerPrefab.View.gameObject.layer;
            Physics2D.IgnoreLayerCollision(charactersLayer, charactersLayer);
        }

        private void OnGameOver()
        {
            DisplayMenu("You Died...");
        }

        private void OnWin()
        {
            DisplayMenu("YOU WON!");
        }

        private void DisplayMenu(string message)
        {
            _text.text = message;
            _mainMenu.gameObject.SetActive(true);
        }
    }
}
