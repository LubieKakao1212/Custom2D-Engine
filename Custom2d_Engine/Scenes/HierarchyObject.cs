using Custom2d_Engine.Math;
using Custom2d_Engine.Ticking;
using System;
using System.Collections.Generic;

namespace Custom2d_Engine.Scenes {
    public class HierarchyObject : IManagedTicker {
        public HierarchyObject? Parent {
            get => _parent;
            set {
                _parent?.RemoveChild(this);

                if (IsRootInHierarchy && value != null) {
                    CurrentHierarchy!.RemoveObject(this);
                }

                _parent = value;
                if (_parent != null) {
                    PrivateSetScene(_parent._currentHierarchy);
                    Transform.Parent = _parent.Transform;
                    _parent.AddChild(this);
                }
                else {
                    Transform.Parent = null;
                }
            }
        }

        public bool IsRootInHierarchy => Parent == null && CurrentHierarchy != null;

        public Hierarchy? CurrentHierarchy {
            get => _currentHierarchy;
            internal set => PrivateSetScene(value);
        }

        public Transform Transform { get; } = new();

        public IReadOnlyList<HierarchyObject> Children => _children;

        /// <summary>
        /// Returns all children as well as their ChildrenDeep <br/>
        /// 
        /// Ordered as follows: <br/>
        /// Child1 -> ChildrenDeep Of Child1 -> Child2 -> ChildrenDeep Of Child2 -> ... -> ChildN -> ChildrenDeep Of ChildN
        /// </summary>
        // Less allocations?
        public IReadOnlyList<HierarchyObject> ChildrenDeep {
            get {
                var result = new List<HierarchyObject>();

                foreach (var child in _children) {
                    child.ChildrenDeepAndSelfBuffered(result);
                }

                return result;
            }
        }

        public IReadOnlyList<HierarchyObject> ChildrenDeepAndSelf {
            get {
                var children = new List<HierarchyObject>(10);
                ChildrenDeepAndSelfBuffered(children);
                return children;
            }
        }

        public void ChildrenDeepAndSelfBuffered(List<HierarchyObject> buffer) {
            buffer.Add(this);
            foreach (var child in Children) {
                child.ChildrenDeepAndSelfBuffered(buffer);
            }
        }

        //TODO remove
        TickManager IManagedTicker.TickManager => _currentHierarchy!.TickManager;

        //Not optimal for larege amount of children
        private readonly List<HierarchyObject> _children = new();

        private Hierarchy? _currentHierarchy;
        private HierarchyObject? _parent;

        public HierarchyObject() {
            Transform.Changed += OnTransformChanged;
        }

        private void OnTransformChanged() {
            if (Transform.Parent != Parent?.Transform) {
                throw new InvalidOperationException(
                    $"Cannot change transform's parent directly, use {nameof(HierarchyObject)}'s {nameof(Parent)} instead");
            }
        }

        private void PrivateSetScene(Hierarchy? scene) {
            if (scene == _currentHierarchy) {
                return;
            }

            if (scene != null) {
                _currentHierarchy = scene;
                AddedToScene();
            }

            foreach (var child in _children) {
                child.PrivateSetScene(_currentHierarchy);
            }

            if (scene == null) {
                RemovedFromScene();
                _currentHierarchy = null;
            }
        }

        private void AddChild(HierarchyObject child) {
            if (_children.Contains(child)) {
                throw new ArgumentException("Adding existing child, this should never happen");
            }

            _children.Add(child);
        }

        private void RemoveChild(HierarchyObject child) {
            if (child._parent != this) {
                throw new InvalidOperationException("Invalid Parenting");
            }

            _children.Remove(child);
        }

        protected virtual void AddedToScene() {
        }

        protected virtual void RemovedFromScene() {
            ((IManagedTicker)this).TickManager.RemoveAllTickers(this);
        }
    }
}