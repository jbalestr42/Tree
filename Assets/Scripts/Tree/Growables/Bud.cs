using UnityEngine;

public class Bud : AGrowable
{
     // TODO: Params, they must be configurable
    float _energyNeededToCreateBranch = 5f;
    float _energyTransfertPerSecond = 1f;

    public Bud(Tree owner, GameObject gameObject, float relativePercentPosition)
        :base(owner, gameObject, GrowableType.Bud, new Vector3(0.7f, 0.7f, 0.7f), relativePercentPosition, owner.BudEnergyData)
    {}

    public override void UpdateBehaviour(EnergyRegulator energyRegulator, float deltaTime)
    {
        if (energyRegulator.IsDepletedUnrecoverable())
        {
            Kill();
        }
        else
        {
            // Bud get energy from his parent branch
            EnergyRegulator.TransfertEnergy(Parent.EnergyRegulator, energyRegulator, deltaTime * _energyTransfertPerSecond);

            if (CanCreateNewBranch(energyRegulator))
            {
                Owner.AddNewBranch(Parent, RelativePercentPosition);
                energyRegulator.ConsumeEnergy(_energyNeededToCreateBranch);
                Kill();
            }
        }
    }

    bool CanCreateNewBranch(EnergyRegulator energyRegulator)
    {
        return energyRegulator.HasEnergy(_energyNeededToCreateBranch) && !ShouldDie() && GetGrowthPercent() >= 1f;
    }
}