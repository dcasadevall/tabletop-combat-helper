using System;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using Utils.Enums;

namespace Grid.Tiles.PathTile {
    [Serializable]
    public class Path : MonoBehaviour {
        
        [Serializable]
        private class LinksDictionary : SerializableDictionaryBase<Vector3Int, LinkedListNode<PathLink>> {}

        public int Length {
            get {
                return pathLinkMap.Count;
            }
        }

        [SerializeField]
        private LinksDictionary pathLinkMap;
        private LinkedList<PathLink> _pathLinks;

        public void Init() {
            _pathLinks = new LinkedList<PathLink>();
            pathLinkMap = new LinksDictionary();
        }

        public void AddLastLink(Vector3Int pos) {
            if (pathLinkMap.ContainsKey(pos)) {
                return;
            }

            var node = _pathLinks.AddLast(new PathLink(pos));
            pathLinkMap.Add(pos, node);
        }

        public void RemoveLink(Vector3Int pos) {
            var node = pathLinkMap[pos];
            _pathLinks.Remove(node);
            pathLinkMap.Remove(pos);
            if (pathLinkMap.Count == 0) {
                DestroyImmediate(gameObject);
            }
        }

        public PathLink GetLastLink() {
            return _pathLinks.Last?.Value;
        }

        public PathLink GetLink(Vector3Int pos) {
            return pathLinkMap[pos]?.Value;
        }

        public PathLink GetPrevLink(Vector3Int pos) {
            return pathLinkMap[pos].Previous?.Value;
        }

        public PathLink GetNextLink(Vector3Int pos) {
            return pathLinkMap[pos].Next?.Value;
        }

        public bool ContainsTile(Vector3Int pos) {
            return pathLinkMap.ContainsKey(pos);
        }
    }

    // TODO: Alberto: should be mutable?
    public class PathLink {
        public PathType PathType = PathType.Single;
        public Vector3Int Position;
        public Vector3Int Direction = Vector3Int.zero;
        public float RotationAngle = 0f;

        public PathLink(Vector3Int position) {
            Position = position;
        }
    }
}