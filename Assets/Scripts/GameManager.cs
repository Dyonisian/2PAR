using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    int _gameState = -1;
    [SerializeField]
    Niantic.ARDK.Extensions.ARPlaneManager _arPlaneManager;
    [SerializeField]
    TapInteractive _wallPrints;
    [SerializeField]
    float _wallSpawnOffset;
    [SerializeField]
    TapInteractive _ceilingBlood;
    
    [SerializeField]
    int _ghostsToSpawn;
    int _ghostsAlive = 0;
    [SerializeField]
    float _ghostDelay;
    [SerializeField]
    float _ghostMoveSpeed;
    [SerializeField]
    InteractiveObjectsManager _interactiveObjectsManager;
    public Niantic.ARDK.Templates.SharedSession _sharedSession;

    [SerializeField]
    float _playerHealth = 100;
    float _timeSinceHit = 0.0f;

    [SerializeField]
    float _healCooldown = 5.0f;
    [SerializeField]
    float _damagePerHit = 10.0f;
    [SerializeField]
    float _invincibilityPeriod = 1.0f;
    [SerializeField]
    Volume _postVolume;
    Vignette _vignette;
    float _startVignette;
    [SerializeField]
    Image _deathPanelImage;

    // Start is called before the first frame update
    void Start()
    {
        var components = _postVolume.profile.components;
        _postVolume.profile.TryGet(out _vignette);
        _startVignette = _vignette.intensity.value;
    }

    // Update is called once per frame
    void Update()
    {
        if(_sharedSession._isHost && _gameState == 0 && !_wallPrints.gameObject.activeSelf && !_ceilingBlood.gameObject.activeSelf)
        {
            ChangeGameState(1);
        }
        if(_sharedSession._isHost && _gameState ==1 && _ghostsAlive == 0)
        {
            ChangeGameState(2);
        }
        _timeSinceHit += Time.deltaTime;
        if(_timeSinceHit > _healCooldown && _playerHealth<100.0f)
        {
            _playerHealth += _damagePerHit;
        }
        _vignette.intensity.value = Mathf.Lerp(0.7f, _startVignette, _playerHealth / 100.0f);
        Color color = _deathPanelImage.color;
        color.a = Mathf.Lerp(1.0f, 0.0f, _playerHealth / 100.0f);
        _deathPanelImage.color = color;
    }
    public void ChangeGameState(int newState)
    {
        if (_gameState == newState)
            return;
        _gameState = newState;
        switch(newState)
        {
            case 0:
                StartPhase0();
                break;
            case 1:
                StartPhase1();
                break;
            case 2:
                StartPhase2();
                break;
        }
    }
    void StartPhase0()
    {
        if (_sharedSession._isHost)
        {
            GameObject spawnTransform = null;
            _arPlaneManager.AddToWall(out spawnTransform);
            _wallPrints.transform.position = spawnTransform.transform.position + spawnTransform.transform.forward * _wallSpawnOffset;
            //_wallPrints.transform.rotation = spawnTransform.transform.rotation;
            //_wallPrints.transform.forward = spawnTransform.transform.forward;
            _wallPrints.gameObject.SetActive(true);

            _arPlaneManager.AddToCeiling(out spawnTransform);
            _ceilingBlood.transform.position = spawnTransform.transform.position - spawnTransform.transform.up * _wallSpawnOffset;
            _ceilingBlood.gameObject.SetActive(true);

            //_sharedSession._messagingManager.BroadcastPhase(0);

        }
        else
        {
            //Zero phase is set by the portal, this broadcast isn't necessary
            //_sharedSession._messagingManager.AskPhase(_sharedSession._host, 0);
        }
    }
    void StartPhase1()
    {
        _ghostsAlive = 0;
        //if (_sharedSession._isHost)
        {
            for(int i =0; i<_ghostsToSpawn; i++)
            {
                _interactiveObjectsManager._ghosts[i].gameObject.SetActive(true);
                _interactiveObjectsManager._ghosts[i]._moveDelay = _ghostDelay * i;
                _interactiveObjectsManager._ghosts[i]._moveSpeed = _ghostMoveSpeed;
                //positions will be overwritten on client
                _interactiveObjectsManager._ghosts[i].transform.position = Camera.main.transform.position + new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)).normalized * 10;
                _interactiveObjectsManager._ghosts[i].transform.position -= Vector3.up/2;
                _interactiveObjectsManager._ghosts[i].OnInteractiveDisable -= GhostDied;
                _interactiveObjectsManager._ghosts[i].OnInteractiveDisable += GhostDied;
                _interactiveObjectsManager._ghosts[i].OnHitPlayer -= Hit;
                _interactiveObjectsManager._ghosts[i].OnHitPlayer += Hit;
                _interactiveObjectsManager._ghosts[i]._isHost = _sharedSession._isHost;
                
                _ghostsAlive++;
            }
        }        
        if(_sharedSession._isHost)
        {
            _sharedSession._messagingManager.BroadcastPhase(1);
        }
    }
    void StartPhase2()
    {
        if (_sharedSession._isHost)
        {
            Debug.Log("Starting Phase 2!");
            _sharedSession._messagingManager.BroadcastPhase(2);
        }
    }
    void GhostDied()
    {
        _ghostsAlive--;
    }
    public void Hit()
    {
        if (_timeSinceHit > _invincibilityPeriod)
        {
            _playerHealth -= _damagePerHit;
            _timeSinceHit = 0;
        }
    }
}
