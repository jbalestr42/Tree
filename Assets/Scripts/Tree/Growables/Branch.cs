using UnityEngine;

public interface IEnergyConsumer
{
    bool ShouldDieIfNotEnoughEnergy();
    bool HasEnoughEnergy();
    void ConsumeEnergyFromRoot(AGrowable growable);
}

public class Branch : AGrowable
{
     // Params, they must be configurable
    int _maxBuds = 3;
    float _budSpawnPercentMin = 0.60f;
    float _budSpawnPercentMax = 0.90f;
    float _nextBudSpawn = 1f;
    
    int _maxLeafs = 1;
    float _leafSpawnPercentMin = 0.90f;
    float _leafSpawnPercentMax = 1.0f;
    float _nextLeafSpawn = 0.6f;

    public Branch(Tree owner, GameObject gameObject, float relativePercentPosition)
        :base(owner, gameObject, GrowableType.Branch, new Vector3(0.5f, 2f, 0.5f), relativePercentPosition)
    {
        _nextBudSpawn = Random.Range(_budSpawnPercentMin, _budSpawnPercentMax);
        _nextLeafSpawn = Random.Range(_leafSpawnPercentMin, _leafSpawnPercentMax);
    }

    public override void UpdateBehaviour(float deltaTime)
    {
        if (CanCreateNewBud())
        {
            /*Bud child = */Owner.AddNewBud(this, _nextBudSpawn);
            _nextBudSpawn = Random.Range(_budSpawnPercentMin, _budSpawnPercentMax);
        }

        if (CanCreateNewLeaf())
        {
            /*Leaf child = */Owner.AddNewLeaf(this, _nextLeafSpawn);
            _nextLeafSpawn = Random.Range(_leafSpawnPercentMin, _leafSpawnPercentMax);
        }
    }

    bool CanCreateNewBud()
    {
        return CountChildren(GrowableType.Bud) < _maxBuds && GetGrowthPercent() > _nextBudSpawn;
    }

    bool CanCreateNewLeaf()
    {
        return CountChildren(GrowableType.Leaf) < _maxLeafs && GetGrowthPercent() > _nextLeafSpawn;
    }

    bool IsLastBranch()
    {
        return CountChildren(GrowableType.Branch) == 0;
    }
}