using UnityEngine;

public class Branch : AGrowable
{
     // Params, they must be configurable
    int _maxBuds = 3;
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
            _nextBudSpawn = Random.Range(_budSpawnPercentMin, _budSpawnPercentMax);
        }
    }

    bool CanCreateNewBud()
    {
        return Children.Count < _maxBuds && GetGrowthPercent() > _nextBudSpawn;
    }
}