using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GhostInteractive : TapInteractive
{
    [HideInInspector]
    public float _moveDelay;
    [HideInInspector]
    public float _moveSpeed;
    [SerializeField]
    Animator _ghostAnimator;
    float _timer = 0.0f;
    [SerializeField]
    float _moveBackTime = 0.5f;
    [SerializeField]
    GameObject _lookTarget;
    public bool _isHost;
    public UnityAction OnHitPlayer;
    public bool _isFinal = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Ghosts are server authoritative
        _audioTimer += Time.deltaTime;
        if(_audioTimer>= _audioDelay)
        {
            _audioTimer = 0.0f;
            if(Random.Range(0,100)> 50)
            {
                if(_audioSource && _nearAudio && !_audioSource.isPlaying && Vector3.Distance(transform.position, Camera.main.transform.position - Vector3.up / 2.0f) < 4.0f)
                {
                    _audioSource.clip = _nearAudio;
                    _audioSource.Play();
                }
            }
        }
        _timer += Time.deltaTime;
        if(_timer>_moveDelay)
        {
            if (Vector3.Distance(transform.position, Camera.main.transform.position - Vector3.up / 2.0f) > 1.0f)
            {
                if(_isHost)
                transform.position = Vector3.MoveTowards(transform.position, Camera.main.transform.position - Vector3.up / 1.5f, _moveSpeed / 100.0f);
            }
            else
            {
                if (_ghostAnimator.GetBool("IsAttack") != true)
                    _ghostAnimator.SetBool("IsAttack", true);
                if (_ghostAnimator.GetBool("IsWalk"))
                    _ghostAnimator.SetBool("IsWalk", false);
                //Damage player
                OnHitPlayer?.Invoke();
            }
        }
        _lookTarget.transform.position = Camera.main.transform.position;
        _lookTarget.transform.position = new Vector3(_lookTarget.transform.position.x, transform.position.y, _lookTarget.transform.position.z);
        if (_isHost)
            transform.LookAt(_lookTarget.transform);
    }
    IEnumerator MoveBackwards(float time)
    {
        while(time>0.0f)
        {
            time -= Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, transform.position - (Camera.main.transform.position - Vector3.up/1.5f - transform.position), _moveSpeed /30.0f);
            yield return null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Protection")
        {
            //Trigger circle
            if (_isHost)
            {
                StartCoroutine(MoveBackwards(_moveBackTime));
                other.GetComponent<ProtectionCircle>()?.Hit();
            }
        }
    }
    private void OnEnable()
    {
        _timer = 0.0f;
        if (!_isFinal)
        {
            _ghostAnimator.SetBool("IsWalk", true);
            _ghostAnimator.SetBool("IsAttack", false);
        }
    }
    public override void TriggeredByOther()
    {
        _currentTaps++;
        if (_currentTaps >= _tapsNeeded)
        {
            _currentTaps = -1000;
            _destroyEffect.Play();
            StartCoroutine(DisableWithDelay(_destroyDelay));
        }
    }
}
