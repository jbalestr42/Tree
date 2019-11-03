using System.Collections;
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
        }
    }

    void UpdatePosition()
    {
        float growthPercent = GetGrowthPercent();

        // Grow
        if (_timer < _growthDuration && GameObject != null)
        {
            _timer += Time.deltaTime;
            
            if (Parent != null && GameObject != null)
            {
                Vector3 offset = (Parent.GameObject.transform.GetChild(2).position - Parent.GameObject.transform.GetChild(1).position) * RelativePercentPosition;
                GameObject.transform.position = Parent.GameObject.transform.GetChild(1).position + offset; // TODO: cleanup GetChild
            }
            GameObject.transform.localScale = growthPercent * Size; // TODO: size x and z must be a param
        }
    }

    public abstract void UpdateBehaviour();

    public virtual bool ShouldDie()
    {
        return false;
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

public class Bud : AGrowable
{
     // Params, they must be configurable
    float _energy = 0f;
    float _energyNeededToCreateBranch = 3f;
    bool _shouldDie = false;

    public Bud(Tree owner, GameObject gameObject, float relativePercentPosition)
        :base(owner, gameObject, new Vector3(0.7f, 0.7f, 0.7f), relativePercentPosition)
    {
    }

    public override void UpdateBehaviour()
    {
        // Accumulate energy from the sun
        // For now it's in this class, but the energy must come from the leaf and is consummed by the bud
        _energy += Time.deltaTime;
        
        if (!_shouldDie && CanCreateNewBranch())
        {
            Owner.AddNewBranch(Parent, RelativePercentPosition);
            _shouldDie = true;
        }
    }

    public override bool ShouldDie()
    {
        return _shouldDie;
    }

    bool CanCreateNewBranch()
    {
        return (_energy >= _energyNeededToCreateBranch);
    }
}

public class Branch : AGrowable
{
     // Params, they must be configurable
    int _maxBuds = 1;
    float _budSpawnPercentMin = 0.60f;
    float _budSpawnPercentMax = 0.90f;
    float _nextBudSpawn = 1f;

    float _energy = 0f;

    public Branch(Tree owner, GameObject gameObject, float relativePercentPosition)
        :base(owner, gameObject, new Vector3(0.5f, 2f, 0.5f), relativePercentPosition)
    {
        _nextBudSpawn = Random.Range(_budSpawnPercentMin, _budSpawnPercentMax);
    }

    public override void UpdateBehaviour()
    {
        // Accumulate energy from the sun 
        _energy += Time.deltaTime;

        /* TODO
         * Create bud instead of branch
         * the bud is getting energy from the sun
         * The energy is transfered to the root
        */
        if (CanCreateNewBud())
        {
            /*Bud child = */Owner.AddNewBud(this, _nextBudSpawn);
            _nextBudSpawn = Random.Range(_nextBudSpawn, _budSpawnPercentMax);
        }
    }

    bool CanCreateNewBud()
    {
        return Children.Count < _maxBuds && GetGrowthPercent() > _nextBudSpawn;
    }
}

public class Tree : MonoBehaviour
{
    AGrowable _root = null;
    AssetManager _assetManager;

    void Start()
    {
        _assetManager = FindObjectOfType<AssetManager>();
        _root = AddNewBranch(null);
    }

    void Update()
    {
        if (_root != null)
        {
            _root.Update();
            _root.Prune();
        }
    }

    public Branch AddNewBranch(AGrowable parent, float relativePercentPosition = 0f)
    {
        GameObject gameObject = _assetManager.CreateBranch(transform);

        Branch branch = new Branch(this, gameObject, relativePercentPosition);
        
        if (parent != null)
        {
            parent.AddChild(branch);
        }
        return branch;
    }

    public Bud AddNewBud(AGrowable parent, float relativePercentPosition = 0f)
    {
        GameObject gameObject = _assetManager.CreateBud(transform);

        Bud bud = new Bud(this, gameObject, relativePercentPosition);
        
        if (parent != null)
        {
            parent.AddChild(bud);
        }
        return bud;
    }
}
