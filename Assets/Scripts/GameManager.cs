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
        _wallPrints.transform.position = spawnTransform.transform.position + spawnTransform.transform.forward * _wallSpawnOffset;
        _wallPrints.transform.rotation = spawnTransform.transform.rotation;
        _wallPrints.gameObject.SetActive(true);

        _ARPlaneManager.AddToCeiling(out spawnTransform);
        _ceilingBlood.transform.position = spawnTransform.transform.position - spawnTransform.transform.up * _wallSpawnOffset;
        _ceilingBlood.gameObject.SetActive(true);
    }
    void StartPhase1()
    {

    }
}
