using System.Collections.Generic;
using UnityEngine;
using Utils.Enums;

namespace Grid.Tiles.PathTile {
    public class Path : MonoBehaviour {
        public int Length {
            get {
                return _pathLinkMap.Count;
            }
        }
        
        private Dictionary<Vector3Int, LinkedListNode<PathLink>> _pathLinkMap;
        private LinkedList<PathLink> _pathLinks;

        public void Init() {
            _pathLinks = new LinkedList<PathLink>();
            _pathLinkMap = new Dictionary<Vector3Int, LinkedListNode<PathLink>>();
        }

        public void AddLastLink(Vector3Int pos) {
            if (_pathLinkMap.ContainsKey(pos)) {
                return;
            }
            var node = _pathLinks.AddLast(new PathLink(pos));
            _pathLinkMap.Add(pos, node);
        }

        public void RemoveLink(Vector3Int pos) {
            var node = _pathLinkMap[pos];
            _pathLinks.Remove(node);
            _pathLinkMap.Remove(pos);
            if (_pathLinkMap.Count == 0) {
                DestroyImmediate(gameObject);
            }
        }
        
        public PathLink GetLastLink() {
            return _pathLinks.Last?.Value;
        }

        public PathLink GetLink(Vector3Int pos) {
            return _pathLinkMap[pos]?.Value;
        }

        public PathLink GetPrevLink(Vector3Int pos) {
            return _pathLinkMap[pos].Previous?.Value;
        }

        public PathLink GetNextLink(Vector3Int pos) {
            return _pathLinkMap[pos].Next?.Value;
        }

        public bool ContainsTile(Vector3Int pos) {
            return _pathLinkMap.ContainsKey(pos);
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