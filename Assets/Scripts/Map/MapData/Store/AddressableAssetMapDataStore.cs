using System;
using System.Collections.Generic;
using System.Linq;
using Logging;
using UniRx.Async;
using UnityEditor;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Map.MapData.Store {
    public class AddressableAssetMapDataStore : IMapDataStore {
        private readonly List<AddressableAssetMapReference> _addressableAssetMapReferences;
        private readonly ILogger _logger;

        public List<IMapReference> MapReferences {
            get {
                return _addressableAssetMapReferences.Cast<IMapReference>().ToList();
            }
        }

        public AddressableAssetMapDataStore(List<AddressableAssetMapReference> mapReferences, ILogger logger) {
            _addressableAssetMapReferences = mapReferences;
            _logger = logger;
        }

        public async UniTask<IMutableMapData> LoadMap(MapStoreId mapStoreId) {
            var mapReference = _addressableAssetMapReferences.Find(x => x.MapStoreId == mapStoreId);
            if (mapReference == null) {
                var msg = $"Invalid MapId: {mapStoreId}";
                _logger.LogError(LoggedFeature.Assets, msg);
                throw new Exception(msg);
            }

            var handle = mapReference.AssetReference.LoadAssetAsync<SerializedMapData>();
            await UniTask.WaitUntil(() => handle.IsDone)
                         .Timeout(TimeSpan.FromSeconds(10));

            if (handle.Status != AsyncOperationStatus.Succeeded) {
                throw new Exception($"Failed to load map: {mapReference.MapName}");
            }

            return handle.Result;
        }

        public bool Commit(MapStoreId mapStoreId) {
            var mapReference = _addressableAssetMapReferences.Find(x => x.MapStoreId == mapStoreId);
            if (mapReference == null) {
                var msg = $"Invalid MapId: {mapStoreId}";
                _logger.LogError(LoggedFeature.Assets, msg);
                return false;
            }
            
            EditorUtility.SetDirty(mapReference.AssetReference.editorAsset);
            AssetDatabase.SaveAssets();
            return true;
        }
    }
}