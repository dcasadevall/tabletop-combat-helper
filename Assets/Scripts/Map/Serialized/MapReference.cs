using System;
using UniRx.Async;
using UnityEditor;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Map.Serialized {
    [Serializable]
    public class MapReference : ILoadadableMapReference, IMapReference {
        public string name;
        public string MapName {
            get {
                return name;
            }
        }


        public AssetReference mapData;
        public async UniTask<IMutableMapData> LoadMap() {
            var handle = mapData.LoadAssetAsync<MapData>();
            await UniTask.WaitUntil(() => handle.IsDone)
                         .Timeout(TimeSpan.FromSeconds(10));
            
            if (handle.Status != AsyncOperationStatus.Succeeded) {
                throw new Exception($"Failed to load map: {name}");
            }

            return handle.Result;
        }
    }
}