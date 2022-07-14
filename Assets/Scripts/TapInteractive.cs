using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapInteractive : InteractiveObject
{
    [SerializeField]
    protected int _tapsNeeded;
    protected int _currentTaps;
    [SerializeField]
    protected ParticleSystem _tapEffect;
    [SerializeField]
    protected ParticleSystem _destroyEffect;
    [SerializeField]
    protected float _destroyDelay;
    [SerializeField]
    protected GameObject _floorObject;
    public override void Trigger()
    {
        _currentTaps++;
        _tapEffect.Play();
        if (_tapAudio && !_audioSource.isPlaying)
        {
            _audioSource.clip = _tapAudio;
            _audioSource.Play();
                }
        if(_currentTaps>=_tapsNeeded)
        {
            _currentTaps = -1000;
            _destroyEffect.Play();
            if(_dieAudio)
            {
                _audioSource.clip = _dieAudio;
                _audioSource.Play();
            }
            StartCoroutine(DisableWithDelay(_destroyDelay));
        }
    }
    private void OnEnable()
    {
        if(_floorObject)
        {
            _floorObject.transform.position = new Vector3(_floorObject.transform.position.x, Camera.main.transform.position.y - 0.5f, _floorObject.transform.position.z);
        }
        if(_spawnAudio)
        {
            _audioSource.clip = _spawnAudio;
            _audioSource.Play();
        }
    }

    public override void TriggeredByOther()
    {
        _currentTaps++;
        _tapEffect.Play();
        if (_currentTaps >= _tapsNeeded)
        {
            _currentTaps = -1000;
            _destroyEffect.Play();
            StartCoroutine(DisableWithDelay(_destroyDelay));
        }
    }
    protected IEnumerator DisableWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

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
                if (!_audioSource.isPlaying)
                {
                    _audioSource.clip = _nearAudio;
                    _audioSource.Play();
                }
            }
        }
    }
}
