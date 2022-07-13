using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    int _gameState = 0;
    [SerializeField]
    Niantic.ARDK.Extensions.ARPlaneManager _ARPlaneManager;
    [SerializeField]
    TapInteractive _wallPrints;
    [SerializeField]
    float _wallSpawnOffset;
    [SerializeField]
    TapInteractive _ceilingBlood;
    public Niantic.ARDK.Templates.SharedSession _sharedSession;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
            _ARPlaneManager.AddToWall(out spawnTransform);
            _wallPrints.transform.position = spawnTransform.transform.position + spawnTransform.transform.forward * _wallSpawnOffset;
            _wallPrints.transform.rotation = spawnTransform.transform.rotation;
            _wallPrints.gameObject.SetActive(true);

            _ARPlaneManager.AddToCeiling(out spawnTransform);
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

    }
}
