using UnityEngine;

public class Leaf : AGrowable
{
     // Params, they must be configurable
    float _energy = 0f;
    float _energyNeededToCreateBranch = 5f;

    public Leaf(Tree owner, GameObject gameObject, float relativePercentPosition)
        :base(owner, gameObject, GrowableType.Leaf, new Vector3(0.7f, 0.7f, 0.7f), relativePercentPosition)
    {}

    public override void UpdateBehaviour(float deltaTime)
    {
        // Accumulate energy from the sun
        _energy += deltaTime;
    }

    bool CanCreateNewBranch()
    {
        return (_energy >= _energyNeededToCreateBranch);
    }
}