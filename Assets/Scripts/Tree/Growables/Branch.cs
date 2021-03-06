﻿using UnityEngine;

public class Branch : AGrowable
{
     // TODO: Params, they must be configurable
    int _maxBuds = 5;
    float _budSpawnPercentMin = 0.50f;
    float _budSpawnPercentMax = 0.95f;
    float _nextBudSpawn = 1f;
    float _energyNeededToCreateBud = 1f;
    
    int _maxLeafs = 2;
    float _leafSpawnPercentMin = 0.90f;
    float _leafSpawnPercentMax = 1.0f;
    float _nextLeafSpawn = 0.6f;
    float _energyNeededToCreateLeaf = 1f;

    float _sizeConsumptionFactor = 1f;

    public Branch(Tree owner, GameObject gameObject, float relativePercentPosition)
        :base(owner, gameObject, GrowableType.Branch, new Vector3(0.5f, 2f, 0.5f), 3f, relativePercentPosition, owner.BranchEnergyData)
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
    }
    
    public override float GetGrowthFactor()
    {
        // TODO This function is a hack, I need to remove hardcoded values
        float growthFactor = 0.2f / Mathf.Min(CountAllChildren(true), 7); 
        if (Parent != null)
        {
            growthFactor = Mathf.Min(growthFactor, Parent.GetGrowthFactor());
        }
        return  growthFactor;
    }

    public override void SetParentBehaviour(AGrowable parent)
    {
        // TODO clean
        MaxSize = parent.MaxSize / 2f;

        // TODO: Add these angles in a param
        Vector3 rotation;
        rotation.x = UnityEngine.Random.Range(-45f, 45f);
        rotation.y = UnityEngine.Random.Range(-45f, 45f);
        rotation.z = UnityEngine.Random.Range(-45f, 45f);

        // We need to rotate only once for now
        GameObject.transform.Rotate(rotation);
    }

    bool CanCreateNewBud(EnergyRegulator energyRegulator)
    {
        return CountChildren(GrowableType.Leaf) >= 1 // Make sure to have at least one leaf otherwise there is no way to gain more energy
                && energyRegulator.HasEnergy(_energyNeededToCreateBud) 
                && (CountChildren(GrowableType.Bud) + CountChildren(GrowableType.Branch)) < _maxBuds;
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