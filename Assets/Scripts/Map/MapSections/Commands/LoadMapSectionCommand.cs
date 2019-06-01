using System;
using System.Collections.Generic;
using CommandSystem;
using Grid.Serialized;
using Logging;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using ILogger = Logging.ILogger;

namespace Map.MapSections.Commands {
    public class LoadMapSectionCommand : ICommand {
        // TODO: Inject this
        private const string kMapSectionScene = "MapSectionScene";

        private readonly LoadMapSectionCommandData _data;
        private readonly List<IMapData> _mapDatas;
        private readonly ILogger _logger;
        private readonly MapSectionContext _mapSectionContext;
        private readonly ZenjectSceneLoader _sceneLoader;

        public bool IsInitialGameStateCommand {
            get {
                return false;
            }
        }

        private uint _previousSection;

        public LoadMapSectionCommand(LoadMapSectionCommandData data,
                                     List<IMapData> mapDatas,
                                     ILogger logger,
                                     MapSectionContext mapSectionContext,
                                     ZenjectSceneLoader sceneLoader) {
            _data = data;
            _mapDatas = mapDatas;
            _logger = logger;
            _mapSectionContext = mapSectionContext;
            _sceneLoader = sceneLoader;
        }

        public IObservable<Unit> Run() {
            if (_data.mapCommandData.mapIndex > _mapDatas.Count) {
                string errorMsg = string.Format("Invalid map index: {0}", _data.mapCommandData.mapIndex);
                _logger.LogError(LoggedFeature.Map, errorMsg);
                return Observable.Throw<Unit>(new Exception(errorMsg));
            }

            IMapData mapData = _mapDatas[(int) _data.mapCommandData.mapIndex];
            if (_data.sectionIndex >= mapData.Sections.Length) {
                return
                    Observable.Throw<Unit>(new
                                               ArgumentException($"Section Index: [{_data.sectionIndex}] is out of bounds."));
            }

            return LoadMapSection(_data.sectionIndex);
        }

        public void Undo() {
            LoadMapSection(_previousSection);
        }

        private static Dictionary<uint, SceneState> _loadedScenes = new Dictionary<uint, SceneState>();

        private IObservable<Unit> LoadMapSection(uint nextSection) {
            _previousSection = _mapSectionContext.CurrentSectionIndex;
            _mapSectionContext.CurrentSectionIndex = nextSection;
            
            if (_loadedScenes.ContainsKey(_previousSection)) {
                _loadedScenes[_previousSection].Deactivate();
            }

            if (_loadedScenes.ContainsKey(nextSection)) {
                _loadedScenes[nextSection].Reactivate();
                return Observable.ReturnUnit();
            }

            IMapData mapData = _mapDatas[(int) _data.mapCommandData.mapIndex];
            IMapSectionData mapSectionData = mapData.Sections[nextSection];

            return _sceneLoader.LoadSceneAsync(kMapSectionScene,
                                               LoadSceneMode.Additive,
                                               container => {
                                                   _loadedScenes[nextSection] =
                                                       new SceneState(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
                                                   
                                                   container.Bind<IGridData>().FromInstance(mapSectionData.GridData);
                                                   container.Bind<IMapSectionData>().FromInstance(mapSectionData);
                                               }).ToUniTask().ToObservable();
        }
    }
}