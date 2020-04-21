using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.Enums;

namespace Grid.Tiles.PathTile {
    public class Path : MonoBehaviour, ISerializationCallbackReceiver {
        public int Length {
            get {
                return _pathLinkMap.Count;
            }
        }

        private Dictionary<Vector3Int, LinkedListNode<PathLink>> _pathLinkMap =
            new Dictionary<Vector3Int, LinkedListNode<PathLink>>();
        private LinkedList<PathLink> _pathLinks = new LinkedList<PathLink>();
        private List<PathLink> _serializedPathLinks = new List<PathLink>();

        public void OnBeforeSerialize() {
            _serializedPathLinks.Clear();
            _serializedPathLinks.AddRange(_pathLinks);
        }

        public void OnAfterDeserialize() {
            _pathLinks.Clear();
            _pathLinkMap.Clear();
            foreach (PathLink pathLink in _serializedPathLinks) {
                var node = _pathLinks.AddLast(pathLink);
                _pathLinkMap[pathLink.position] = node;
            }
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
    [Serializable]
    public class PathLink {
        [SerializeField] public PathType pathType = PathType.Single;
        [SerializeField] public Vector3Int position;
        [SerializeField] public Vector3Int direction = Vector3Int.zero;
        [SerializeField] public float rotationAngle;
        [SerializeField] public bool isDiagonal;

        public PathLink(Vector3Int position) {
            this.position = position;
        }
    }
}