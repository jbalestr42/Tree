using UnityEngine;

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
        gameObject.GetComponentInChildren<GrowableComponent>().Growable = branch;
        
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
        gameObject.GetComponent<GrowableComponent>().Growable = bud;
        
        if (parent != null)
        {
            parent.AddChild(bud);
        }
        return bud;
    }
}
