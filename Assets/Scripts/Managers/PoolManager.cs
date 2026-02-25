using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PoolManager : MonoBehaviour
{
    
    private int maxAmmountOfPrefabs = 5;
    public GameObject prefab;

    private List<GameObject> pooledObjects = new List<GameObject>();

    private static PoolManager _instance;

    public static PoolManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindAnyObjectByType<PoolManager>();
            }
            return _instance;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < maxAmmountOfPrefabs; i++)
        {
            SpawnItem();
        }
    }

    private GameObject SpawnItem()
    {
        GameObject _prefab = Instantiate(prefab);
            
        _prefab.name = _prefab.name + pooledObjects.Count;
        _prefab.transform.SetParent(gameObject.transform);
        pooledObjects.Add(_prefab);
        _prefab.SetActive(false);
        return _prefab;
    }

    // Update is called once per frame
    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        
        return SpawnItem();
    }

    public void ReturnToPool(GameObject _object)
    {
        _object.SetActive(false);
        _object.transform.SetParent(gameObject.transform);
    }

}