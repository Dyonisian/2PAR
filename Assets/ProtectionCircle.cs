using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectionCircle : MonoBehaviour
{
    [SerializeField]
    ParticleSystem _particleSystem;
    [SerializeField]
    ParticleSystemRenderer _spikesRenderer;
    float _health = 100.0f;
    [SerializeField]
    float _damagePerHit = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
    }
    private void OnEnable()
    {
        _particleSystem.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Hit()
    {
        _health -= _damagePerHit;
        Color color = _spikesRenderer.material.color;
        color.a = _health;
        _spikesRenderer.material.color = color;
        if (!_particleSystem.isEmitting)
            _particleSystem.Play();
        if(_health<0.0f)
        {
            gameObject.SetActive(false);
        }
    }
    public void Reset()
    {
        _health = 100;
        _particleSystem.Stop();
        gameObject.SetActive(true);
    }

}
