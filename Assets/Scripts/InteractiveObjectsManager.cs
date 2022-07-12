using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObjectsManager : MonoBehaviour
{
    public List<InteractiveObject> _interactiveObjects;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _interactiveObjects.Count; i++)
        {
            _interactiveObjects[i]._index = i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
