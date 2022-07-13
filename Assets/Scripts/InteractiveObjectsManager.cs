using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObjectsManager : MonoBehaviour
{
    [SerializeField]
    public Niantic.ARDK.Templates.SharedSession _sharedSession;
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
        if(_sharedSession._isHost)
        for (int i = 0; i < _interactiveObjects.Count; i++)
        {
            if(_interactiveObjects[i].gameObject.activeSelf && _interactiveObjects[i]._shouldBroadcast)
                {
                    _sharedSession._messagingManager.BroadcastInteractiveObjectPosition(i,_interactiveObjects[i].transform.position);
                    _sharedSession._messagingManager.BroadcastInteractiveObjectRotation(i,_interactiveObjects[i].transform.rotation);
                }
        }
    }
    public void SetObjectPosition(int index, Vector3 position)
    {
        
    }
    public void SetObjectRotation(int index, Quaternion rotation)
    {
        _interactiveObjects[index].gameObject.SetActive(true);
        _interactiveObjects[index].transform.rotation = rotation;
    }
    public void TriggerInteractiveObject(int index)
    {
        _interactiveObjects[index].TriggeredByOther();
    }
}
