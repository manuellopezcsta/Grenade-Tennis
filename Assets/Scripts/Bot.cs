using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{

    public float speed = 50;
    public float HitForce;
    
    public GameManager GM;
    Animator _animator;
    public Transform Ball;
    public Transform AimTarget;
    public Transform[] Targets;

    Vector3 _targetPosition;
    Vector3 _AimTargetInitialPos;

    ShotManager _SM;
    Shot _currentShotType;


    void Start()
    {
        _targetPosition = transform.position;
        _animator = GameObject.Find("/Bot/Raqueta").GetComponent<Animator>();
        _AimTargetInitialPos = AimTarget.position;

        _SM = GetComponent<ShotManager>();
        _currentShotType = _SM.topSpin;
    }

    // Update is called once per frame

    void Update()
    {
        if(Ball != null)
        {
            _targetPosition.x = Ball.position.x;
            if(Ball.position.z >= 12.44f)
            {
                _targetPosition.z = Ball.position.z;
            }
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    Vector3 PickTarget()
    {
        int _randomValue = Random.Range(0, Targets.Length);
        return Targets[_randomValue].position;
    }

    Shot PickShot()
    {
        int RandomValue = Random.Range(0,2);
        if (RandomValue == 0)
        {
            return _SM.topSpin;
        } else {
            return _SM.flat;
        }
    }

    void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Granade"))
        {
            GM.PlaySound();
            _currentShotType = PickShot();
            //Debug.Log("Bot Boop");
            Vector3 _dir = PickTarget() - transform.position; // Calculo el Vector desde el jugador al blanco al que apunto.
            other.GetComponent<Rigidbody>().velocity = _dir.normalized * _currentShotType.hitForce +  new Vector3 (0, _currentShotType.upForce, 0);

            // Vamos a usar el vector anterior de direccion para ver en que dir esta llendo la bola. Eje X

            Vector3 _ballDir = Ball.position - transform.position;

            if(_ballDir.x >= 0)
            {
                _animator.Play("Golpe Derecha");
            } else {
                _animator.Play("Golpe Izq");
            }
        }
    }
}
