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

    [SerializeField]
    LookInteractive _ghostStanding;
    [SerializeField]
    TapInteractive _markOnWall;
    [SerializeField]
    LookInteractive _crawlerCeiling;
    [SerializeField]
    LookInteractive _ghostOnPlayer;
    [SerializeField]
    GameObject _lookTarget;
    [SerializeField]
    Text _debugText;

    float _spawnTimer = 0.0f;
    [SerializeField]
    float _delayBetweenSpawns = 10.0f;

    [SerializeField]
    int _activeObjects = 0;

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
        _spawnTimer += Time.deltaTime;
        if(_sharedSession._isHost && _gameState == 0 && _activeObjects<=0)
        {
            ChangeGameState(1);
        }
        if(_sharedSession._isHost && _gameState ==1 && _ghostsAlive == 0)
        {
            ChangeGameState(2);
        }
        if (_sharedSession._isHost && _gameState == 2 && _activeObjects <= 0)
        {
            ChangeGameState(3);
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
        _spawnTimer = 0.0f;
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
            case 3:
                StartPhase3();
                break;
        }
    }
    IEnumerator SetActiveWithDelay(GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);
        go.SetActive(true);
    }    
    void StartPhase0()
    {
        _debugText.text = "Started phase 0!";
        if (_sharedSession._isHost)
        {
            GameObject spawnTransform = null;
            _arPlaneManager.AddToWall(out spawnTransform);
            _wallPrints.transform.position = spawnTransform.transform.position + spawnTransform.transform.up * _wallSpawnOffset;
            //_wallPrints.transform.rotation = spawnTransform.transform.rotation;
            //_wallPrints.transform.forward = spawnTransform.transform.forward;
            StartCoroutine(SetActiveWithDelay(_wallPrints.gameObject,_delayBetweenSpawns));
            _activeObjects++;
            _wallPrints.OnInteractiveDisable -= InteractiveDisabled;
            _wallPrints.OnInteractiveDisable += InteractiveDisabled;

            spawnTransform = null;
            bool success = _arPlaneManager.AddToCeiling(out spawnTransform);
            if (!success)
            {
                Vector3 temp = _lookTarget.transform.position;
                _lookTarget.transform.position = Camera.main.transform.position + new Vector3(Random.Range(-1.0f, 1.0f), 2, Random.Range(-1.0f, 1.0f));
                _ceilingBlood.transform.position = _lookTarget.transform.position - _lookTarget.transform.right * _wallSpawnOffset;
                _lookTarget.transform.position = temp;
            }
            else
            {
                _ceilingBlood.transform.position = spawnTransform.transform.position - spawnTransform.transform.right * _wallSpawnOffset;
            }
            StartCoroutine(SetActiveWithDelay(_ceilingBlood.gameObject, _delayBetweenSpawns*2));
            _activeObjects++;
            _ceilingBlood.OnInteractiveDisable -= InteractiveDisabled;
            _ceilingBlood.OnInteractiveDisable += InteractiveDisabled;
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
        _debugText.text = "Started phase 1!";

        _ghostsAlive = 0;
        //if (_sharedSession._isHost)
        {
            for(int i =0; i<_ghostsToSpawn; i++)
            {
                _interactiveObjectsManager._ghosts[i].gameObject.SetActive(true);
                _interactiveObjectsManager._ghosts[i]._moveDelay = _delayBetweenSpawns + _ghostDelay * i;
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
        _debugText.text = "Started phase 2!";

        if (_sharedSession._isHost)
        {
            GameObject spawnTransform = null;
            _arPlaneManager.AddToFloor(out spawnTransform);
            _ghostStanding.transform.position = spawnTransform.transform.position + spawnTransform.transform.right * _wallSpawnOffset;
            Vector3 lookTargetOldPos = _lookTarget.transform.position;
            _lookTarget.transform.position = new Vector3(_lookTarget.transform.position.x, _ghostStanding.transform.position.y, _lookTarget.transform.position.z);
            _lookTarget.transform.position = _ghostStanding.transform.position - Camera.main.transform.position;
            _lookTarget.transform.position = new Vector3(_lookTarget.transform.position.x, _ghostStanding.transform.position.y, _lookTarget.transform.position.z);
            _ghostStanding.transform.LookAt(_lookTarget.transform);
            _lookTarget.transform.position = lookTargetOldPos;
            _ghostStanding._isHost = _sharedSession._isHost;
            _ghostStanding._moveSpeed = _ghostMoveSpeed;
            
            StartCoroutine(SetActiveWithDelay(_ghostStanding.gameObject, _delayBetweenSpawns));
            _activeObjects++;
            _ghostStanding.OnInteractiveDisable -= InteractiveDisabled;
            _ghostStanding.OnInteractiveDisable += InteractiveDisabled;
            _ghostStanding.OnHitPlayer -= Hit;
            _ghostStanding.OnHitPlayer += Hit;


            spawnTransform = null;
            _arPlaneManager.AddToFloor(out spawnTransform);           
            
                _crawlerCeiling.transform.position = spawnTransform.transform.position + spawnTransform.transform.right * _wallSpawnOffset + Vector3.up * 2.0f;
            
            _crawlerCeiling._isHost = _sharedSession._isHost;
            _crawlerCeiling._moveSpeed = _ghostMoveSpeed;
            StartCoroutine(SetActiveWithDelay(_crawlerCeiling.gameObject, _delayBetweenSpawns * 2));
            _crawlerCeiling.OnInteractiveDisable -= InteractiveDisabled;
            _crawlerCeiling.OnInteractiveDisable -= InteractiveDisabled;
            _crawlerCeiling.OnHitPlayer -= Hit;
            _crawlerCeiling.OnHitPlayer += Hit;
            _activeObjects++;


            Debug.Log("Starting Phase 2!");
            _sharedSession._messagingManager.BroadcastPhase(2);
        }
    }
    void StartPhase3()
    {
        _debugText.text = "Started phase 3!";
        if (_sharedSession._isHost)
        {
            
            _ghostOnPlayer.transform.position = Camera.main.transform.position + Vector3.up * _wallSpawnOffset;
            Vector3 lookTargetOldPos = _lookTarget.transform.position;
            _lookTarget.transform.position = new Vector3(_lookTarget.transform.position.x, _ghostOnPlayer.transform.position.y, _lookTarget.transform.position.z);
            _lookTarget.transform.position = Camera.main.transform.position + Camera.main.transform.forward;
            _lookTarget.transform.position = new Vector3(_lookTarget.transform.position.x, _ghostOnPlayer.transform.position.y, _lookTarget.transform.position.z);
            _ghostOnPlayer.transform.LookAt(_lookTarget.transform);
            _lookTarget.transform.position = lookTargetOldPos;
            _ghostOnPlayer._isHost = _sharedSession._isHost;
            _ghostOnPlayer._moveSpeed = _ghostMoveSpeed;
            _ghostOnPlayer._alwaysMove = true;

            StartCoroutine(SetActiveWithDelay(_ghostOnPlayer.gameObject, 0));
            _activeObjects++;
            _ghostOnPlayer.OnInteractiveDisable -= InteractiveDisabled;
            _ghostOnPlayer.OnInteractiveDisable += InteractiveDisabled;
            _ghostOnPlayer.OnHitPlayer -= Hit;
            _ghostOnPlayer.OnHitPlayer += Hit;

            Debug.Log("Starting Phase 3!");
            _sharedSession._messagingManager.BroadcastPhase(3);
        }
    }
    void GhostDied()
    {
        _ghostsAlive--;
    }
    void InteractiveDisabled()
    {
        _activeObjects--;
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
