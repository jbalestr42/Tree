using UnityEngine;

public class Leaf : AGrowable
{
    float _energyGainPerSecond = 2f;

    public Leaf(Tree owner, GameObject gameObject, float relativePercentPosition)
        :base(owner, gameObject, GrowableType.Leaf, new Vector3(0.7f, 0.7f, 0.7f), relativePercentPosition, new EnergyRegulator.EnergyData(2f, 0f, 10f, 3f))
    {}

    public override void UpdateBehaviour(EnergyRegulator energyRegulator, float deltaTime)
    {
        if (energyRegulator.IsDepletedUnrecoverable())
        {
            Kill();
        }
        else if (!ShouldDie())
        {
            // Accumulate energy from the sun
            if (EnvironmentHelper.Instance.IsEnlightened(this))
            {
                AGrowable growable = Parent;
                while (growable != null)
                {
                    // Energy is distributed to all parent equally
                    // TODO gain energy only if all parent are not depletedUnrecoverable
                    float energyGain = (_energyGainPerSecond * deltaTime) / Depth;
                    growable.EnergyRegulator.GainEnergy(energyGain);
                    growable = growable.Parent;
                }
            }
        }
    }
}