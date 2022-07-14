using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField]
    float _ghostDelay;
    [SerializeField]
    float _ghostMoveSpeed;
    [SerializeField]
    InteractiveObjectsManager _interactiveObjectsManager;
    public Niantic.ARDK.Templates.SharedSession _sharedSession;
    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_gameState==0 && !_wallPrints.gameObject.activeSelf && !_ceilingBlood.gameObject.activeSelf)
        {
            ChangeGameState(1);
        }
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
            //_sharedSession._messagingManager.AskPhase(_sharedSession._host, 0);
        }
    }
    void StartPhase1()
    {
        if (_sharedSession._isHost)
        {
            for(int i =0; i<_ghostsToSpawn; i++)
            {
                _interactiveObjectsManager._ghosts[i].gameObject.SetActive(true);
                _interactiveObjectsManager._ghosts[i]._moveDelay = _ghostDelay * i;
                _interactiveObjectsManager._ghosts[i]._moveSpeed = _ghostMoveSpeed;
                _interactiveObjectsManager._ghosts[i].transform.position = Camera.main.transform.position + new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)).normalized * 10;
                _interactiveObjectsManager._ghosts[i].transform.position -= Vector3.up/2;
            }
        }
        else
        {
            //same?
        }
    }
    void StartPhase2()
    {

    }
}
