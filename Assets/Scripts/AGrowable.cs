using System.Collections.Generic;
using UnityEngine;

public abstract class AGrowable
{
    // TODO add depth from bud (from the creator)
    public int Depth { get; set; } // This is depth from the root
    public Vector3 Size { get; set; }
    public float RelativePercentPosition { get; } // The relative position between Start and End in the parent branch
    public AGrowable Parent { get; private set; }
    public Tree Owner { get; private set; }
    public List<AGrowable> Children { get; private set; }
    
    private GameObject GameObject { get; set; }
    private float _timer = 0f;
    private bool _hasBeenKilled = false;

     // Params, they must be configurable
    private float _growthDuration = 2f;

    public AGrowable(Tree owner, GameObject gameObject, Vector3 size, float relativePercentPosition)
    {
        Depth = 0;
        Size = size;
    
        Owner = owner;
        GameObject = gameObject;
        Children = new List<AGrowable>();
        RelativePercentPosition = relativePercentPosition;
    }

    public void Update()
    {
        UpdateBehaviour();
        UpdatePosition();
        
        // Update children
        for (int i = 0; i < Children.Count; i++)
        {
            Children[i].Update();
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
            foreach (AGrowable child in Children)
            {
                child.Prune();
            }

            Children.RemoveAll(child => child.ShouldDie());
        }
    }

    void UpdatePosition()
    {
        float growthPercent = GetGrowthPercent();

        // Grow
        if (_timer < _growthDuration && GameObject != null)
        {
            _timer += Time.deltaTime;
            
            if (Parent != null && Parent.GameObject != null)
            {
                Vector3 offset = (Parent.GameObject.transform.GetChild(2).position - Parent.GameObject.transform.GetChild(1).position) * RelativePercentPosition;
                GameObject.transform.position = Parent.GameObject.transform.GetChild(1).position + offset; // TODO: cleanup GetChild
            }
            GameObject.transform.localScale = growthPercent * Size; // TODO: size x and z must be a param
        }
    }

    public abstract void UpdateBehaviour();

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
        foreach (AGrowable child in Children)
        {
            child.Die();
        }
        Children.Clear();
        GameObject.Destroy(GameObject);
    }

    public float GetGrowthPercent()
    {
        return Mathf.Clamp(_timer / _growthDuration, 0f, 1f);
    }

    public void AddChild(AGrowable child)
    {
        child.SetParent(this);
        Children.Add(child);
    }

    public void RemoveChild(AGrowable child)
    {
        child.SetParent(null);
        Children.Remove(child);
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
        rotation.x = Random.Range(-45f, 45f);
        rotation.y = Random.Range(-45f, 45f);
        rotation.z = Random.Range(-45f, 45f);

        // We need to rotate only once for now
        GameObject.transform.Rotate(rotation);
    }
}