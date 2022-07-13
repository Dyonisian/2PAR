using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapInteractive : InteractiveObject
{
    [SerializeField]
    int _tapsNeeded;
    int _currentTaps;
    [SerializeField]
    ParticleSystem _tapEffect;
    [SerializeField]
    ParticleSystem _destroyEffect;
    [SerializeField]
    float _destroyDelay;
    public override void Trigger()
    {
        _currentTaps++;
        _tapEffect.Play();
        if(_currentTaps>=_tapsNeeded)
        {
            _currentTaps = -1000;
            _destroyEffect.Play();
            DisableWithDelay(_destroyDelay);
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
            DisableWithDelay(_destroyDelay);
        }
    }
    IEnumerator DisableWithDelay(float delay)
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
