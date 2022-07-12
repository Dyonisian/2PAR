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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void ChangeGameState(int newState)
    {
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
        GameObject spawnTransform = null;
        _ARPlaneManager.AddToWall(out spawnTransform);
        Instantiate(_wallPrints, spawnTransform.transform.position + spawnTransform.transform.forward * _wallSpawnOffset, spawnTransform.transform.rotation);
        _ARPlaneManager.AddToCeiling(out spawnTransform);
        Instantiate(_ceilingBlood, spawnTransform.transform.position - spawnTransform.transform.up * _wallSpawnOffset, spawnTransform.transform.rotation);
    }
    void StartPhase1()
    {

    }
}
