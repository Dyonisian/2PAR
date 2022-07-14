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
        if(_currentTaps>=_tapsNeeded)
        {
            _currentTaps = -1000;
            _destroyEffect.Play();
            StartCoroutine(DisableWithDelay(_destroyDelay));
        }
    }
    private void OnEnable()
    {
        if(_floorObject)
        {
            _floorObject.transform.position = new Vector3(_floorObject.transform.position.x, Camera.main.transform.position.y - 0.5f, _floorObject.transform.position.z);
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
        
    }
}
