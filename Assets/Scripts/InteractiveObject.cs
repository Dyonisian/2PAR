using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class InteractiveObject : MonoBehaviour
{
    [HideInInspector]
    public int _index;
    
    public bool _shouldBroadcast;
    public bool _isActivatedOnLook;
    public bool _isActivatedOnLookAway;
    public bool _isActivatedOnSecondLook;
    public bool _isActivatedOnTrigger;
    public abstract void Trigger();
    public abstract void TriggeredByOther();
    public UnityAction OnInteractiveDisable;
    protected void OnDisable()
    {
        OnInteractiveDisable?.Invoke();
    }
    [SerializeField]
    protected AudioSource _audioSource;
    [SerializeField]
    protected AudioClip _spawnAudio;
    [SerializeField]
    protected AudioClip _tapAudio;  
    [SerializeField]
    protected AudioClip _dieAudio;
    [SerializeField]
    protected AudioClip _nearAudio;
    protected float _audioTimer = 0.0f;
    [SerializeField]
    protected float _audioDelay = 3.0f;
}
