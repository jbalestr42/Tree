using UnityEngine;

public class EnvironmentHelper : Singleton<EnvironmentHelper>
{
    [SerializeField]
    private GameObject _sun = null;

    public bool IsEnlightened(AGrowable growable)
    {
        return !Physics.Raycast(growable.GameObject.transform.position, _sun.transform.forward, 1000);
    }
}
