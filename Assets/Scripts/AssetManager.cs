using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _branchPrefab;

    [SerializeField]
    private GameObject _budPrefab;

    public GameObject CreateBranch(Transform parent)
    {
        return Create(_branchPrefab, parent);
    }

    public GameObject CreateBud(Transform parent)
    {
        return Create(_budPrefab, parent);
    }

    public GameObject Create(GameObject prefab, Transform parent)
    {
        GameObject gameObject = Instantiate(prefab);
        if (parent != null)
        {
            gameObject.transform.SetParent(parent);
        }
        return gameObject;
    }
}
