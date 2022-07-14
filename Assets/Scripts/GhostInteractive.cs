using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostInteractive : TapInteractive
{
    [HideInInspector]
    public float _moveDelay;
    [HideInInspector]
    public float _moveSpeed;
    [SerializeField]
    Animator _ghostAnimator;
    float _timer = 0.0f;
    [SerializeField]
    float _moveBackTime = 0.5f;
    [SerializeField]
    GameObject _lookTarget;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if(_timer>_moveDelay)
        {
            if(Vector3.Distance(transform.position, Camera.main.transform.position - Vector3.up/2.0f)>1.0f)
            transform.position = Vector3.MoveTowards(transform.position, Camera.main.transform.position - Vector3.up/1.5f, _moveSpeed / 100.0f);
            else
            {
                if(_ghostAnimator.GetBool("IsAttack") != true)
                _ghostAnimator.SetBool("IsAttack", true);
                if (_ghostAnimator.GetBool("IsWalk"))
                    _ghostAnimator.SetBool("IsWalk", false);
                //Damage player
            }
        }
        _lookTarget.transform.position = Camera.main.transform.position;
        _lookTarget.transform.position = new Vector3(_lookTarget.transform.position.x, transform.position.y, _lookTarget.transform.position.z);
        transform.LookAt(_lookTarget.transform);
    }
    IEnumerator MoveBackwards(float time)
    {
        while(time>0.0f)
        {
            time -= Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, transform.position - (Camera.main.transform.position - Vector3.up/1.5f - transform.position), _moveSpeed /30.0f);
            yield return null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Protection")
        {
            //Trigger circle
            StartCoroutine(MoveBackwards(_moveBackTime));
        }
    }
    private void OnEnable()
    {
        _timer = 0.0f;
        _ghostAnimator.SetBool("IsWalk", true);
        _ghostAnimator.SetBool("IsAttack", false);        

    }
}
