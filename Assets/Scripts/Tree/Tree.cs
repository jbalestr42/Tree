using UnityEngine;

// TODO: add oisif namespace
public class Tree : MonoBehaviour
{
    [SerializeField]
    private EnergyRegulator.EnergyData _branchEnergyData = null;
    
    [SerializeField]
    private EnergyRegulator.EnergyData _budEnergyData = null;
    
    [SerializeField]
    private EnergyRegulator.EnergyData _leafEnergyData = null;

    AGrowable _root = null;
    EnvironmentHelper _environmentHelper;
    AssetManager _assetManager;

    void Start()
    {
        _environmentHelper = FindObjectOfType<EnvironmentHelper>();
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

    public EnergyRegulator.EnergyData BranchEnergyData
    {
        get { return _branchEnergyData; }
    }

    public EnergyRegulator.EnergyData BudEnergyData
    {
        get { return _budEnergyData; }
    }

    public EnergyRegulator.EnergyData LeafEnergyData
    {
        get { return _leafEnergyData; }
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
