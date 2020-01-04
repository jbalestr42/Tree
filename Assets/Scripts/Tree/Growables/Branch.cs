using UnityEngine;

public class Branch : AGrowable
{
     // TODO: Params, they must be configurable
    int _maxBuds = 2;
    float _budSpawnPercentMin = 0.60f;
    float _budSpawnPercentMax = 0.90f;
    float _nextBudSpawn = 1f;
    float _energyNeededToCreateBud = 1f;
    
    int _maxLeafs = 1;
    float _leafSpawnPercentMin = 0.90f;
    float _leafSpawnPercentMax = 1.0f;
    float _nextLeafSpawn = 0.6f;
    float _energyNeededToCreateLeaf = 1f;

    public Branch(Tree owner, GameObject gameObject, float relativePercentPosition)
        :base(owner, gameObject, GrowableType.Branch, new Vector3(0.5f, 2f, 0.5f), relativePercentPosition, owner.BranchEnergyData)
    {
        _nextBudSpawn = Random.Range(_budSpawnPercentMin, _budSpawnPercentMax);
        _nextLeafSpawn = Random.Range(_leafSpawnPercentMin, _leafSpawnPercentMax);
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
    }

    bool CanCreateNewBud(EnergyRegulator energyRegulator)
    {
        return CountChildren(GrowableType.Leaf) >= 1 // Make sure to have at least one leaf otherwise there is no way to gain more energy
                && energyRegulator.HasEnergy(_energyNeededToCreateBud) 
                && (CountChildren(GrowableType.Bud) + CountChildren(GrowableType.Branch)) < _maxBuds 
                && GetGrowthPercent() > _nextBudSpawn;
    }

    bool CanCreateNewLeaf(EnergyRegulator energyRegulator)
    {
        return energyRegulator.HasEnergy(_energyNeededToCreateLeaf) && CountChildren(GrowableType.Leaf) < _maxLeafs && GetGrowthPercent() > _nextLeafSpawn;
    }

    bool IsLastBranch()
    {
        return CountChildren(GrowableType.Branch) == 0;
    }
}