using System;
using System.Collections.Generic;
using Custom2d_Engine.Ticking;
using Custom2d_Engine.Util;
using Microsoft.Xna.Framework;

namespace Custom2d_Engine.Scenes;

//TODO Optimise callbacks
public class Hierarchy : IManagedTicker {
    public IReadOnlyCollection<DrawableObject> Drawables =>
        CustomOrderedInstancesOf<DrawableObject>(obj => obj.EnableDraw, (obj) => obj.DrawOrder);

    public TickManager TickManager { get; private set; }

    private readonly HashSet<HierarchyObject> _rootsSet = new();
    private readonly List<HierarchyObject> _roots = new();

    private readonly List<HierarchyObject> _objectsToAdd = new();
    private readonly List<HierarchyObject> _objectsToRemove = new();

    private bool _isUpdating;

    public Hierarchy(TickManager tickManager) {
        TickManager = tickManager;
    }

    public void AddObject(HierarchyObject obj) {
        if (_isUpdating) {
            _objectsToAdd.Add(obj);
            if (_objectsToAdd.Contains(obj)) {
                return;
            }

            if (_objectsToRemove.Contains(obj)) {
                _objectsToRemove.Remove(obj);
            }

            _objectsToAdd.Add(obj);
            return;
        }

        InsertObject(obj, _roots.Count);
    }

    public void InsertObject(HierarchyObject obj, int order) {
        if (_isUpdating) {
            throw new ApplicationException("Not usable during updates");
        }

        if (obj.Parent != null) {
            throw new InvalidOperationException("Cannot change scene of non-root object");
        }

        if (_rootsSet.Contains(obj)) {
            //TODO Inplement logger
            Console.Out.WriteLine("Assigning object to scene it is alredy assigned, this may be a mistake");
            return;
        }

        obj.CurrentHierarchy?.RemoveObjectInternal(obj);

        obj.CurrentHierarchy = this;
        _rootsSet.Add(obj);
        _roots.Insert(order, obj);
    }

    public void RemoveObject(HierarchyObject obj) {
        if (_isUpdating) {
            if (_objectsToRemove.Contains(obj)) {
                return;
            }

            if (_objectsToAdd.Contains(obj)) {
                _objectsToAdd.Remove(obj);
            }
            else {
                _objectsToRemove.Add(obj);
            }

            return;
        }

        if (!_rootsSet.Contains(obj)) {
            throw new InvalidOperationException("Invalid object removal");
        }

        RemoveObjectInternal(obj);

        obj.CurrentHierarchy = null;
    }

    public IReadOnlyCollection<T> AllInstancesOf<T>() {
        var listOut = new List<T>();
        var buffer = new List<HierarchyObject>();

        foreach (var root in _roots) {
            buffer.Clear();
            root.ChildrenDeepAndSelfBuffered(buffer);
            foreach (var obj in root.ChildrenDeepAndSelf) {
                if (obj is T instance) {
                    listOut.Add(instance);
                }
            }
        }

        return listOut;
    }

    public IReadOnlyCollection<T> CustomOrderedInstancesOf<T>(Func<T, bool> filter, Func<T, float> orderer) {
        IDictionary<float, List<T>> orderedDrawables = new SortedDictionary<float, List<T>>();

        int count = 0;
        var buffer = new List<HierarchyObject>();

        foreach (var root in _roots) {
            buffer.Clear();
            root.ChildrenDeepAndSelfBuffered(buffer);
            foreach (var obj in buffer) {
                if (obj is T instance && filter(instance)) {
                    var list = orderedDrawables.GetOrSetToDefaultLazy(orderer(instance), key => new List<T>());
                    list.Add(instance);
                    count++;
                }
            }
        }
            
        var listOut = new List<T>(count);

        foreach (var order in orderedDrawables) {
            listOut.AddRange(order.Value);
        }

        return listOut;
    }

    public void Update(GameTime gameTime) {
        var updatables = CustomOrderedInstancesOf<HierarchyObject>(
            obj => obj.EnableUpdates, 
            obj => obj.UpdateOrder);
            
        BeginUpdate();
        foreach (var updatable in updatables) {
            updatable.Update(gameTime);
        }
        EndUpdate();
    }
        
    public void BeginUpdate() {
        if (_isUpdating) {
            throw new InvalidOperationException($"Cannot call {nameof(BeginUpdate)} when already updating");
        }

        _isUpdating = true;
    }

    public void EndUpdate() {
        _isUpdating = false;

        foreach (var addition in _objectsToAdd) {
            AddObject(addition);
        }

        _objectsToAdd.Clear();

        foreach (var removal in _objectsToRemove) {
            RemoveObject(removal);
        }

        _objectsToRemove.Clear();
    }

    private void RemoveObjectInternal(HierarchyObject obj) {
        if (!_rootsSet.Contains(obj)) {
            throw new InvalidOperationException("Invalid object removal");
        }

        _rootsSet.Remove(obj);
        _roots.Remove(obj);
    }
}