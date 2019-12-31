using UnityEngine;

public class Leaf : AGrowable
{
    public Leaf(Tree owner, GameObject gameObject, float relativePercentPosition)
        :base(owner, gameObject, GrowableType.Leaf, new Vector3(0.7f, 0.7f, 0.7f), relativePercentPosition)
    {}

    public override void UpdateBehaviour(float deltaTime)
    {
        if (!ShouldDie())
        {
            // Accumulate energy from the sun
            if (Owner.IsEnlightened(this))
            {
                Owner.AddEnergy(2f * deltaTime);
            }
        }
    }
}