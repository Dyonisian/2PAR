using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class InteractiveObject : MonoBehaviour
{
    [HideInInspector]
    public int _index;
    
    public bool _shouldBroadcast;
    public abstract void Trigger();
    public abstract void TriggeredByOther();
    public UnityAction OnInteractiveDisable;
    protected void OnDisable()
    {
        OnInteractiveDisable?.Invoke();
    }
}
