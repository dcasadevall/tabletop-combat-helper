using System;
using System.Collections.Generic;
using Logging;
using Map.MapData;
using Map.MapData.Store;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ILogger = Logging.ILogger;

namespace Map.MapSelection {
    public class MapSelectViewController : MonoBehaviour, IMapSelectViewController {
       public event Action<int> LoadMapClicked = delegate {};
       public event Action<int> EditMapClicked = delegate {};

#pragma warning disable 649
        [SerializeField]
        private Dropdown _dropdown;
        
        [SerializeField]
        private Button _loadMapButton;
        
        [SerializeField]
        private Button _editMapButton;
#pragma warning restore 649
        
        private int _selectedIndex = 0;
        
        private List<IMapReference> _mapReferences;
        private ILogger _logger;

        [Inject]
        public void Construct(List<IMapReference> mapReferences, ILogger logger) {
            _mapReferences = mapReferences;
            _logger = logger;
        }

        private void Start() {
            if (_dropdown == null) {
                _logger.LogError(LoggedFeature.Map, "Dropdown not assigned.");
                return;
            }
            
            if (_loadMapButton == null) {
                _logger.LogError(LoggedFeature.Map, "Spawn button not assigned.");
                return;
            }
            
            _loadMapButton.onClick.AddListener(HandleOnLoadMapClicked);
            _editMapButton.onClick.AddListener(HandleOnEditMapClicked);
            _dropdown.onValueChanged.AddListener(HandleOnValueChanged);
        }

        private void OnDestroy() {
            _loadMapButton.onClick.RemoveListener(HandleOnLoadMapClicked);
            _editMapButton.onClick.RemoveListener(HandleOnEditMapClicked);
            _dropdown.onValueChanged.RemoveListener(HandleOnValueChanged);
        }

        private void HandleOnLoadMapClicked() {
            if (_selectedIndex < 0 || _selectedIndex >= _mapReferences.Count) {
                _logger.LogError(LoggedFeature.Map, "Invalid selected index: {0}", _selectedIndex);
                return;
            }
            
            LoadMapClicked.Invoke(_selectedIndex);
        }
        
        private void HandleOnEditMapClicked() {
            if (_selectedIndex < 0 || _selectedIndex >= _mapReferences.Count) {
                _logger.LogError(LoggedFeature.Map, "Invalid selected index: {0}", _selectedIndex);
                return;
            }
            
            EditMapClicked.Invoke(_selectedIndex);
        }

        public void Show() {
            if (_dropdown == null) {
                _logger.LogError(LoggedFeature.Map, "Dropdown not assigned.");
                return;
            }
            
            if (_loadMapButton == null) {
                _logger.LogError(LoggedFeature.Map, "Spawn button not assigned.");
                return;
            }

            _dropdown.ClearOptions();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            foreach (var mapData in _mapReferences) {
                options.Add(new Dropdown.OptionData(mapData.MapName));
            }
            _dropdown.AddOptions(options);
            
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }

        private void HandleOnValueChanged(int selectedIndex) {
            _selectedIndex = selectedIndex;
        } 
    }
}