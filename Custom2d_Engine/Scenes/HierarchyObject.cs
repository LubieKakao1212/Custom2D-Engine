﻿using Custom2d_Engine.Math;
using Custom2d_Engine.Ticking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Scenes
{
    public class HierarchyObject : IManagedTicker
    {
        public HierarchyObject Parent
        {
            get => parent;
            set
            {
                parent?.RemoveChild(value);

                if (IsRootInHierarcy && value != null)
                {
                    CurrentHierarchy.RemoveObject(this);
                }
                
                parent = value;
                if (parent != null)
                {
                    PrivateSetScene(parent.currentHierarchy);
                    transform.Parent = parent.Transform;
                    parent.AddChild(this);
                }
                else
                {
                    transform.Parent = null;
                }
            }
        }

        public bool IsRootInHierarcy => Parent == null && CurrentHierarchy != null;

        public Hierarchy CurrentHierarchy
        {
            get => currentHierarchy;
            internal set
            {
                PrivateSetScene(value);
            }
        }
        
        public Transform Transform
        {
            get => transform;
        }

        public IReadOnlyList<HierarchyObject> Children => children;

        /// <summary>
        /// Returns all children as well as their ChildrenDeep <br/>
        /// 
        /// Ordered as follows: <br/>
        /// Child1 -> ChildrenDeep Of Child1 -> Child2 -> ChildrenDeep Of Child2 -> ... -> ChildN -> ChildrenDeep Of ChildN
        /// </summary>
        // Potentialy a lot of allocations
        public IReadOnlyList<HierarchyObject> ChildrenDeep
        {
            get
            {
                var result = new List<HierarchyObject>();

                foreach (var child in children)
                {
                    result.Add(child);
                    result.AddRange(child.ChildrenDeep);
                }

                return result;
            }
        }

        public IReadOnlyList<HierarchyObject> ChildrenDeepAndSelf
        {
            get
            {
                var result = new List<HierarchyObject>
                {
                    this
                };

                foreach (var child in children)
                {
                    result.AddRange(child.ChildrenDeepAndSelf);
                }

                return result;
            }
        }

        TickManager IManagedTicker.TickManager => currentHierarchy.TickManager;

        //Not optimal for larege amount of children
        private readonly List<HierarchyObject> children = new();

        private Hierarchy currentHierarchy;
        private HierarchyObject parent = null;
        private readonly Transform transform = new();

        public HierarchyObject()
        {
            transform.Changed += OnTransformChanged;
        }

        private void OnTransformChanged()
        {
            if (Transform.Parent != Parent?.Transform)
            {
                throw new InvalidOperationException($"Cannot change transform's parent directly, use {nameof(HierarchyObject)}'s {nameof(Parent)} instead");
            }
        }

        private void PrivateSetScene(Hierarchy scene)
        {
            if (scene == currentHierarchy)
            {
                return;
            }

            if (scene != null)
            {
                currentHierarchy = scene;
                AddedToScene();
            }

            foreach (var child in children)
            {
                child.PrivateSetScene(currentHierarchy);
            }
            
            if (scene == null)
            {
                RemovedFromScene();
                currentHierarchy = null;
            }
        }

        private void AddChild(HierarchyObject child)
        {
            if (children.Contains(child))
            {
                throw new ArgumentException("Adding existing child, this should never happen");
            }
            children.Add(child);
        }
        
        private void RemoveChild(HierarchyObject child)
        {
            if (child.parent != this)
            {
                throw new InvalidOperationException("Invalid Parenting");
            }
            children.Remove(child);
        }

        protected virtual void AddedToScene() { }

        protected virtual void RemovedFromScene()
        {
            ((IManagedTicker)this).TickManager.RemoveAllTickers(this);
        }
    }
}
