using UnityEngine;

public class Branch : AGrowable
{
    // TODO: Params, they must be configurable
    int _maxBuds = 5;
    float _budSpawnPercentMin = 0.40f;
    float _budSpawnPercentMax = 0.95f;
    float _nextBudSpawn = 1f;
    float _energyNeededToCreateBud = 1f;

    int _maxLeafs = 2;
    float _leafSpawnPercentMin = 0.90f;
    float _leafSpawnPercentMax = 1.0f;
    float _nextLeafSpawn = 0.6f;
    float _energyNeededToCreateLeaf = 1f;

    float _sizeConsumptionFactor = 1f;

    public Branch(Tree owner, GameObject gameObject, float relativePercentPosition) : base(owner, gameObject, GrowableType.Branch, new Vector3(0.5f, 2f, 0.5f), 3f, relativePercentPosition, owner.BranchEnergyData)
    {
        _nextBudSpawn = Random.Range(_budSpawnPercentMin, _budSpawnPercentMax);
        _nextLeafSpawn = Random.Range(_leafSpawnPercentMin, _leafSpawnPercentMax);

        // Awafull test
        // Ca devrait etre une valeur degressive, plus on de branches "solitaires" plus on a de chances que la branche se divise
        // Les bracnhes "solitaires" representent en faite une seule branche qui modifie sa forme
        if (Random.value < 0.2f)
        {
            _maxBuds = 1;
            _nextBudSpawn = 1f;
        }
    }

    public override void UpdateBehaviour(EnergyRegulator energyRegulator, float deltaTime)
    {
        if (energyRegulator.IsDepletedUnrecoverable())
        {
            if (IsLastBranch())
            {
                Kill();
            }
            else
            {
                // Don't kill but stop growing and stop all energy transfert
                // the consequence is that the child branch will die which seems good in this case
            }
        }
        else
        {
            // Branch consume additional energy based on their size
            //energyRegulator.ConsumeEnergy(deltaTime * CurrentSize * 0.2f * CountAllChildren(true)/* _sizeConsumptionFactor*/);

            if (CanCreateNewLeaf(energyRegulator))
            {
                Owner.AddNewLeaf(this, _nextLeafSpawn);
                energyRegulator.ConsumeEnergy(_energyNeededToCreateLeaf);
                _nextLeafSpawn = Random.Range(_leafSpawnPercentMin, _leafSpawnPercentMax);
            }

            // TODO make a component to manage Growable creation
            if (CanCreateNewBud(energyRegulator))
            {
                Owner.AddNewBud(this, _nextBudSpawn);
                energyRegulator.ConsumeEnergy(_energyNeededToCreateBud);
                _nextBudSpawn = Random.Range(_budSpawnPercentMin, _budSpawnPercentMax);
            }
        }

        // Test naif d'ajout d'un angle representant le fait que la derniere branch tend vers une positon inverse a la gravité alors que le reste tend vers le sol
        // Get light position from more than one ray to simulate ambiant light
        ComputeRotation();
    }

    private Vector3 _currentVelocityLight = Vector3.zero;
    private float _smoothTimeLight = 0.1f;
    private Vector3 _currentVelocityGravity = Vector3.zero;
    private float _smoothTimeGravity = 0.1f;
    private void ComputeRotation()
    {
        Vector3 targetUp = EnvironmentHelper.Instance.sunPosition - GameObject.transform.position;
        float maxSpeedUp = 0.4f / Mathf.Sqrt(CountAllChildren(true) + 10);
        if (!EnvironmentHelper.Instance.IsEnlightened(this))
        {
            maxSpeedUp = 0f;
        }
        Vector3 localUpTowardWorldUp = Vector3.SmoothDamp(GameObject.transform.up, targetUp, ref _currentVelocityLight, _smoothTimeLight, maxSpeedUp);
        Vector3 targetDown = Vector3.ProjectOnPlane(GameObject.transform.up, Vector3.up).normalized;
        targetDown = Vector3.Lerp(Vector3.up, targetDown, 0.7f);
        float maxSpeedDown = 0.1f * (1f - (1f / Mathf.Sqrt((CountAllChildren(true) + 1))));
        if (CountAllChildren(true) > 100)
        {
            maxSpeedDown = 0f;
        }
        Vector3 localUpTowardWorldDown = Vector3.SmoothDamp(GameObject.transform.up, targetDown, ref _currentVelocityGravity, _smoothTimeGravity, maxSpeedDown);
        GameObject.transform.up = Vector3.Lerp(localUpTowardWorldUp, localUpTowardWorldDown, 0.8f);
    }

    public override float GetGrowthFactor()
    {
        // TODO This function is a hack, I need to remove hardcoded values
        float growthFactor = 0.1f / Mathf.Sqrt(CountAllChildren(true) + 10);
        if (Parent == null)
        {
            // Debug.Log(growthFactor);
        }
        // if (Parent != null) {
        // growthFactor = 0.1f / Mathf.Min(CountAllChildren(true), 7);;
        // }
        return growthFactor;
    }

    public override void SetParentBehaviour(AGrowable parent)
    {
        // TODO clean
        MaxSize = parent.MaxSize / 2f;

        // TODO: Add these angles in a param
        Vector3 rotation;
        float angle = 75;
        rotation.x = UnityEngine.Random.Range(-angle, angle);
        rotation.y = UnityEngine.Random.Range(-angle, angle);
        rotation.z = UnityEngine.Random.Range(-angle, angle);

        // We need to rotate only once for now
        GameObject.transform.Rotate(rotation);
    }

    bool CanCreateNewBud(EnergyRegulator energyRegulator)
    {
        return CountChildren(GrowableType.Leaf) >= 1 // Make sure to have at least one leaf otherwise there is no way to gain more energy
            &&
            energyRegulator.HasEnergy(_energyNeededToCreateBud) &&
            (CountChildren(GrowableType.Bud) + CountChildren(GrowableType.Branch)) < _maxBuds;
    }

    bool CanCreateNewLeaf(EnergyRegulator energyRegulator)
    {
        return energyRegulator.HasEnergy(_energyNeededToCreateLeaf) && CountChildren(GrowableType.Leaf) < _maxLeafs;
    }

    bool IsLastBranch()
    {
        return CountChildren(GrowableType.Branch) == 0;
    }
}