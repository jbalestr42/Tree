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
            _root.Update(Time.deltaTime);
            _root.Prune();
        }
    }

    // TODO: use template for better reusability
    public Branch AddNewBranch(AGrowable parent, float relativePercentPosition = 0f)
    {
        GameObject gameObject = _assetManager.CreateBranch(transform);

        Branch growable = new Branch(this, gameObject, relativePercentPosition);
        gameObject.GetComponentInChildren<GrowableComponent>().Growable = growable;
        gameObject.transform.localScale = Vector3.zero;

        if (parent != null)
        {
            parent.AddChild(growable);
        }
        return growable;
    }

    public Bud AddNewBud(AGrowable parent, float relativePercentPosition = 0f)
    {
        GameObject gameObject = _assetManager.CreateBud(transform);

        Bud growable = new Bud(this, gameObject, relativePercentPosition);
        gameObject.GetComponent<GrowableComponent>().Growable = growable;
        gameObject.transform.localScale = Vector3.zero;
        
        if (parent != null)
        {
            parent.AddChild(growable);
        }
        return growable;
    }

    public Leaf AddNewLeaf(AGrowable parent, float relativePercentPosition = 0f)
    {
        GameObject gameObject = _assetManager.CreateLeaf(transform);

        Leaf growable = new Leaf(this, gameObject, relativePercentPosition);
        gameObject.GetComponent<GrowableComponent>().Growable = growable;
        gameObject.transform.localScale = Vector3.zero;
        
        if (parent != null)
        {
            parent.AddChild(growable);
        }
        return growable;
    }
}
