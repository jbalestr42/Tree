using UnityEngine;

public class EnergyRegulator
{
    public struct EnergyData
    {
        public float StartingEnergy;
        public float EnergyConsumptionPerSecond;
        public float EnergyMax;
        public float DepletedDurationBeforeUnrecoverable;

        public EnergyData(float startingEnergy, float energyConsumptionPerSecond, float energyMax, float depletedDurationBeforeUnrecoverable)
        {
            StartingEnergy = startingEnergy;
            EnergyConsumptionPerSecond = energyConsumptionPerSecond;
            EnergyMax = energyMax;
            DepletedDurationBeforeUnrecoverable = depletedDurationBeforeUnrecoverable;
        }
    }

    EnergyData _data;
    float _currentEnergy;
    float _depletedTimer;

    public EnergyRegulator(EnergyData data)
    {
        _data = data;
        _currentEnergy = data.StartingEnergy;
        _depletedTimer = 0f;
    }

    public void Update(float deltaTime)
    {
        if (IsDepleted())
        {
            _depletedTimer += deltaTime;
        }
        else
        {
            _depletedTimer = 0f;
            ConsumeEnergy(deltaTime * _data.EnergyConsumptionPerSecond);
        }
    }

    public bool IsDepleted()
    {
        return _currentEnergy <= 0f;
    }

    public bool IsDepletedSince(float second)
    {
        return IsDepleted() && _depletedTimer >= second;
    }
    
    public bool IsDepletedUnrecoverable()
    {
        return IsDepletedSince(_data.DepletedDurationBeforeUnrecoverable);
    }

    public void ConsumeEnergy(float energy)
    {
        _currentEnergy -= energy;
        _currentEnergy = Mathf.Max(_currentEnergy, 0f);
    }

    public void GainEnergy(float energy)
    {
        _currentEnergy += energy;
        _currentEnergy = Mathf.Min(_currentEnergy, _data.EnergyMax);
    }

    public bool HasEnergy(float energy)
    {
        return _currentEnergy >= energy;
    }

    public static bool TransfertEnergy(EnergyRegulator regulatorSource, EnergyRegulator regulatorDestination, float amount)
    {
        bool isTransfertDone = false;

        if (regulatorSource.HasEnergy(amount))
        {
            regulatorSource.ConsumeEnergy(amount);
            regulatorDestination.GainEnergy(amount);
            isTransfertDone = true;
        }

        return isTransfertDone;
    }
}