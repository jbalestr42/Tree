using UnityEngine;

public class Bud : AGrowable
{
     // Params, they must be configurable
    float _energy = 0f;
    float _energyNeededToCreateBranch = 5f;

    public Bud(Tree owner, GameObject gameObject, float relativePercentPosition)
        :base(owner, gameObject, new Vector3(0.7f, 0.7f, 0.7f), relativePercentPosition)
    {}

    public override void UpdateBehaviour(float deltaTime)
    {
        // Accumulate energy from the sun
        // For now it's in this class, but the energy must come from the leaf and is consummed by the bud
        _energy += deltaTime;
        
        if (!ShouldDie() && CanCreateNewBranch())
        {
            Owner.AddNewBranch(Parent, RelativePercentPosition);
            Kill();
        }
    }

    bool CanCreateNewBranch()
    {
        return (_energy >= _energyNeededToCreateBranch);
    }
}