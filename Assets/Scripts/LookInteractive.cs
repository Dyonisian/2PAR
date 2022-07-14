using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LookInteractive : TapInteractive
{
    [SerializeField]
    Animator _animator;
    float _timer = 0.0f;
    [SerializeField]
    GameObject _lookTarget;
    public bool _isHost;
    public UnityAction OnHitPlayer;
    bool _isLookedAt = false;
    bool _isLookedAway = false;
    bool _isSecondLook = false;
    bool _isAttacking = false;
    [HideInInspector]
    public bool _alwaysMove = false;
    [HideInInspector]
    public float _moveSpeed;
    [SerializeField]
    string _attackTriggerName;
    [SerializeField]
    string _firstAnimTriggerName;
    [SerializeField]
    public Niantic.ARDK.Templates.SharedSession _sharedSession;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _audioTimer += Time.deltaTime;
        if (_audioTimer >= _audioDelay)
        {
            _audioTimer = 0.0f;
            if (Random.Range(0, 100) > 50)
            {
                if (_isAttacking && _audioSource && _nearAudio && !_audioSource.isPlaying && Vector3.Distance(transform.position, Camera.main.transform.position - Vector3.up / 2.0f) < 2.0f)
                {
                    _audioSource.clip = _nearAudio;
                    _audioSource.Play();
                }
            }
        }

        _timer += Time.deltaTime;
        //TODO - Functionality only works for host right now
        _lookTarget.transform.position = Camera.main.transform.position;
        _lookTarget.transform.position = new Vector3(_lookTarget.transform.position.x, transform.position.y, _lookTarget.transform.position.z);
        if (_isHost && _isAttacking)
        {
            if (Vector3.Distance(transform.position, Camera.main.transform.position - Vector3.up / 2.0f) > 1.0f)
            {
                if (_isHost)
                {
                    transform.position = Vector3.MoveTowards(transform.position, Camera.main.transform.position - Vector3.up / 1.5f, _moveSpeed / 100.0f);
                    transform.LookAt(_lookTarget.transform);
                }
            }
            else
            {
                OnHitPlayer?.Invoke();
            }
        }
        if(_alwaysMove)
        {
            if (_isHost)
            {
                if (Vector3.Distance(transform.position, Camera.main.transform.position - Vector3.up / 2.0f) > 1.0f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, Camera.main.transform.position - Vector3.up / 1.5f, _moveSpeed / 100.0f);
                }
                transform.LookAt(_lookTarget.transform);
            }
        }

        float gaze = Vector3.Dot(Camera.main.transform.forward, (transform.position - Camera.main.transform.position).normalized);
        if(!_isLookedAt && gaze>0.8f)
        {
            _isLookedAt = true;
        }
        if(!_isLookedAway && _isLookedAt && gaze<=0.2f)
        {
            _isLookedAway = true;            
        }
        if(_isLookedAway && gaze>0.8f)
        {
            _isSecondLook = true;
        }
        if(_isActivatedOnLook && _isLookedAt)
        {
            if(_isHost)
            Activate();
        }
        if(_isActivatedOnLookAway && _isLookedAway)
        {
            if(_isHost)
            Activate();
        }
        if(_isActivatedOnSecondLook && _isSecondLook)
        {
            if (_isHost)
                Activate();
        }
    }
    public virtual void Activate()
    {
        _isAttacking = true;
        _animator.SetBool(_attackTriggerName, true);
        if (_sharedSession._isHost)
            _sharedSession._messagingManager.BroadcastLookInteractiveActive(_index);
        else
            _sharedSession._messagingManager.AskLookInteractiveActive(_sharedSession._host, _index);
    }
    public virtual void ActivatedByOther()
    {
        _isAttacking = true;
        _animator.SetBool(_attackTriggerName, true);
    }
    public override void Trigger()
    {
        if (!_isAttacking || _isHost)
            return;
        _currentTaps++;
        if(_isActivatedOnTrigger)
        {
            Activate();
        }
        if (_currentTaps >= _tapsNeeded)
        {
            _currentTaps = -1000;
            _destroyEffect.Play();
            StartCoroutine(DisableWithDelay(_destroyDelay));
        }
    }
    public override void TriggeredByOther()
    {
        if (!_isAttacking || _isHost)
            return;
        _currentTaps++;
        if (_currentTaps >= _tapsNeeded)
        {
            _currentTaps = -1000;
            _destroyEffect.Play();
            StartCoroutine(DisableWithDelay(_destroyDelay));
        }
    }
    private void OnEnable()
    {
        if(_firstAnimTriggerName!="")
        _animator.SetBool(_firstAnimTriggerName, true);
    }
}
