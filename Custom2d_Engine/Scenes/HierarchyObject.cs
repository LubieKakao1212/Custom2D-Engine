using System;
using System.Collections.Generic;
using Custom2d_Engine.Math;
using Custom2d_Engine.Ticking;
using Microsoft.Xna.Framework;

namespace Custom2d_Engine.Scenes {
    public class HierarchyObject : IManagedTicker {
        public HierarchyObject? Parent {
            get => _parent;
            set {
                _parent?.RemoveChild(this);

                var newHierarchy = value?._currentHierarchy;
                
                if (CurrentHierarchy != newHierarchy) {
                    PrivateSetScene(newHierarchy);
                }
                
                _parent = value;
                if (_parent != null) {
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

        public bool EnableUpdates { get; set; } = false;
        public float UpdateOrder { get; set; } = 0;
        public bool UseTickManager { get; set; } = true;
        
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
        
        //TODO remove
        TickManager IManagedTicker.TickManager => _localTickManager;
        
        //Not optimal for larege amount of children
        private readonly List<HierarchyObject> _children = new();

        private readonly TickManager _localTickManager = new TickManager();
        private Hierarchy? _currentHierarchy;
        private HierarchyObject? _parent;

        public HierarchyObject() {
            Transform.Changed += OnTransformChanged;
        }

        public void Update(GameTime gameTime) {
            CustomUpdate(gameTime);
            if (UseTickManager) {
                _localTickManager.Forward(gameTime.ElapsedGameTime);
            }
        }

        /// <summary>
        /// Used to perform custom update logic even while not using local <see cref="TickManager"/> <br/>
        /// Called before local TickManager
        /// </summary>
        protected virtual void CustomUpdate(GameTime time) { }
        
        public void ChildrenDeepAndSelfBuffered(List<HierarchyObject> buffer) {
            buffer.Add(this);
            foreach (var child in Children) {
                child.ChildrenDeepAndSelfBuffered(buffer);
            }
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
                if (IsRootInHierarchy) {
                    CurrentHierarchy!.RemoveObject(this);
                }
                
                _currentHierarchy = scene;
                AddedToScene();
            }

            foreach (var child in _children) {
                child.PrivateSetScene(_currentHierarchy);
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

        public virtual void RemovedFromScene() {
            _currentHierarchy = null;
            ((IManagedTicker)this).TickManager.RemoveAllTickers(this);
        }
    }
}