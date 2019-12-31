using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum GrowableType
{
    Branch = 0,
    Bud,
    Leaf
}

/*
 * Branch need energy to grow
 * Not enough energy: after N seconds, die (only if it's the last branch
 *  
 * Bud need energy to grow and spawn branch
 * Not enough energy: after N seconds, die
 * 
 * Leaf need energy to grow
 * Not enough energy: after N seconds, die
 * 
 * */

public abstract class AGrowable
{
    // TODO add depth from bud (from the creator)
    public Tree Owner { get; private set; }
    public AGrowable Parent { get; private set; }
    public GrowableType Type { get; private set; }
    public Vector3 Size { get; set; }
    public float RelativePercentPosition { get; } // The relative position between Start and End in the parent branch
    public Dictionary<GrowableType, List<AGrowable>> Children { get; private set; }
    public int Depth { get; set; } // This is depth from the root
    public GameObject GameObject { get; private set; }
    
    private float _timer = 0f;
    private bool _hasBeenKilled = false;

     // Params, they must be configurable
    private float _growthDuration = 2f;

    public AGrowable(Tree owner, GameObject gameObject, GrowableType type, Vector3 size, float relativePercentPosition)
    {
        Owner = owner;
        GameObject = gameObject;
        Type = type;
        Size = size;
        RelativePercentPosition = relativePercentPosition;
    
        // Initialiaze the children container
        Children = new Dictionary<GrowableType, List<AGrowable>>();
        foreach (GrowableType growableType in (GrowableType[]) Enum.GetValues(typeof(GrowableType)))
        {
            Children[growableType] = new List<AGrowable>();
        }
        Depth = 0;
    }

    public void Update(float deltaTime)
    {
        UpdateBehaviour(deltaTime);
        UpdatePosition(deltaTime);
        
        // Update children
        foreach (KeyValuePair<GrowableType, List<AGrowable>> children in Children)
        {
            for (int i = 0; i < children.Value.Count; i++)
            {
                children.Value[i].Update(deltaTime);
            }
        }
    }

    public void Prune()
    {
        if (ShouldDie())
        {
            Die();
        }
        else
        {
            foreach (KeyValuePair<GrowableType, List<AGrowable>> children in Children)
            {
                foreach (AGrowable child in children.Value)
                {
                    child.Prune();
                }
                children.Value.RemoveAll(child => child.ShouldDie());
            }
        }
    }

    void UpdatePosition(float deltaTime)
    {
        // Grow
        if (_timer < _growthDuration && GameObject != null)
        {
            _timer += deltaTime;
            
            float growthPercent = GetGrowthPercent();
            if (Parent != null && Parent.GameObject != null)
            {
                Vector3 offset = (Parent.GameObject.transform.GetChild(2).position - Parent.GameObject.transform.GetChild(1).position) * RelativePercentPosition;
                GameObject.transform.position = Parent.GameObject.transform.GetChild(1).position + offset; // TODO: cleanup GetChild
            }
            GameObject.transform.localScale = growthPercent * Size; // TODO: size x and z must be a param
        }
    }

    public abstract void UpdateBehaviour(float deltaTime);

    public bool ShouldDie()
    {
        return _hasBeenKilled;
    }

    public void Kill()
    {
        _hasBeenKilled = true;
    }

    public void Die()
    {
        foreach (KeyValuePair<GrowableType, List<AGrowable>> children in Children)
        {
            foreach (AGrowable child in children.Value)
            {
                child.Die();
            }
            children.Value.Clear();
        }
        GameObject.Destroy(GameObject);
    }

    public float GetGrowthPercent()
    {
        return Mathf.Clamp(_timer / _growthDuration, 0f, 1f);
    }

    public void AddChild(AGrowable child)
    {
        Assert.IsTrue(Children.ContainsKey(child.Type), "Keys are initialized in the constructor.");

        child.SetParent(this);
        Children[child.Type].Add(child);
    }

    public void RemoveChild(AGrowable child)
    {
        Assert.IsTrue(Children.ContainsKey(child.Type), "Keys are initialized in the constructor.");

        child.SetParent(null);
        Children[child.Type].Remove(child);
    }

    public int CountChildren(GrowableType type)
    {
        Assert.IsTrue(Children.ContainsKey(type), "Keys are initialized in the constructor.");

        return Children[type].Count;
    }

    public void SetParent(AGrowable parent)
    {
        Parent = parent;
        
        if (parent != null)
        {
            Depth = parent.Depth + 1;
        }

        // TODO: Add these angles in a param
        Vector3 rotation;
        rotation.x = UnityEngine.Random.Range(-45f, 45f);
        rotation.y = UnityEngine.Random.Range(-45f, 45f);
        rotation.z = UnityEngine.Random.Range(-45f, 45f);

        // We need to rotate only once for now
        GameObject.transform.Rotate(rotation);
    }
}