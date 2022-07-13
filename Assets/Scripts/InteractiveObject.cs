using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractiveObject : MonoBehaviour
{
    [HideInInspector]
    public int _index;
    public abstract void Trigger();
    public abstract void TriggeredByOther();

}
