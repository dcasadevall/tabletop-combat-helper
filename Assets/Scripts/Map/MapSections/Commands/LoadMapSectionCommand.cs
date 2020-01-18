using System;
using System.Collections.Generic;
using CommandSystem;
using Grid.Serialized;
using UniRx;
using UniRx.Async;
using UnityEngine.SceneManagement;
using Zenject;

namespace Map.MapSections.Commands {
    public class LoadMapSectionCommand : ICommand {
        private readonly SignalBus _signalBus;
        private readonly LoadMapSectionCommandData _data;
        private readonly IMapData _mapData;
        private readonly IPausableCommandQueue _pausableCommandQueue;
        private readonly MapSectionContext _mapSectionContext;
        private readonly ZenjectSceneLoader _sceneLoader;

        public bool IsInitialGameStateCommand {
            get {
                return false;
            }
        }

        private uint _previousSection;

        public LoadMapSectionCommand(SignalBus signalBus,
                                     LoadMapSectionCommandData data,
                                     IMapData mapData,
                                     IPausableCommandQueue pausableCommandQueue,
                                     MapSectionContext mapSectionContext,
                                     ZenjectSceneLoader sceneLoader) {
            _signalBus = signalBus;
            _data = data;
            _mapData = mapData;
            _pausableCommandQueue = pausableCommandQueue;
            _mapSectionContext = mapSectionContext;
            _sceneLoader = sceneLoader;
        }

        public IObservable<Unit> Run() {
            if (_data.sectionIndex >= _mapData.Sections.Length) {
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
            IMapSectionData mapSectionData = _mapData.Sections[nextSection];
            _signalBus.Fire(new MapSectionWillLoadSignal(mapSectionData));
            
            // While we change scenes, pause any commands from being processed.
            // This avoids race conditions with commands being instantiated in the wrong context.
            _pausableCommandQueue.Pause();

            _previousSection = _mapSectionContext.CurrentSectionIndex;
            _mapSectionContext.CurrentSectionIndex = nextSection;

            if (_loadedScenes.ContainsKey(_previousSection)) {
                _loadedScenes[_previousSection].Deactivate();
            }

            if (_loadedScenes.ContainsKey(nextSection)) {
                _pausableCommandQueue.Resume();
                _loadedScenes[nextSection].Reactivate();
                return Observable.ReturnUnit();
            }

            return _sceneLoader.LoadSceneAsync(_data.mapCommandData.sectionSceneName,
                                               LoadSceneMode.Additive,
                                               container => {
                                                   _loadedScenes[nextSection] =
                                                       new SceneState(SceneManager.GetSceneAt(SceneManager.sceneCount -
                                                                                              1));

                                                   container.Bind<IGridData>().FromInstance(mapSectionData.GridData);
                                                   container.Bind<IMapSectionData>().FromInstance(mapSectionData);
                                                   _pausableCommandQueue.Resume();
                                               }).ToUniTask().ToObservable();
        }
    }
}