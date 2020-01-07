using UnityEngine;

public class EnvironmentHelper : Singleton<EnvironmentHelper>
{
    [SerializeField]
    private GameObject _sun = null;
    public Vector3 sunPosition
    {
        get { return _sun.transform.position; }
    }

    public bool IsEnlightened(AGrowable growable)
    {
        return !Physics.Raycast(growable.GameObject.transform.position, _sun.transform.position - growable.GameObject.transform.position, 1000);
    }

    private void Update()
    {
        // Spin the object around the world origin at 20 degrees/second.
        float speed = 100f;
        _sun.transform.RotateAround(Vector3.zero, Vector3.up, speed * Time.deltaTime);
    }
}