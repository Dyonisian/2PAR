using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnterPortal : InteractiveObject
{
    [SerializeField]
    ParticleSystem _dustParticles;
    [SerializeField]
    UnityEngine.Rendering.Volume _postProcessVolume;
    [SerializeField]
    ParticleSystem _portalParticles;
    [SerializeField]
    GameManager _gameManager;
    [SerializeField]
    AudioSource _ambientSource;
    public override void Trigger()
    {
        _postProcessVolume.enabled = true;
        _dustParticles.gameObject.SetActive(true);
        _portalParticles.Stop();
        _gameManager.ChangeGameState(2);
        _ambientSource.Play();
        gameObject.SetActive(false);
    }
    public override void TriggeredByOther()
    {
        _postProcessVolume.enabled = true;
        _dustParticles.gameObject.SetActive(true);
        _portalParticles.Stop();
        _gameManager.ChangeGameState(2);
        _ambientSource.Play();

        gameObject.SetActive(false);


    }

    // Start is called before the first frame update
    void Start()
    {
        _portalParticles.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }    
}
