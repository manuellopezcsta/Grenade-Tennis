using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody _rigidbody;
    [SerializeField] // La hace visible y editable desde Unity.
    Vector3 v3ForceH;
    [SerializeField]
    KeyCode keyPositiveH; // Definimos 2 botones.
    [SerializeField]
    KeyCode keyNegativeH;
    [SerializeField] 
    Vector3 v3ForceV;
    [SerializeField]
    KeyCode keyPositiveV; 
    [SerializeField]
    KeyCode keyNegativeV;

    public Transform AimTarget;
    public Transform Ball;
    bool hitting;
    public float TargetSpeed;
    public float HitForce;
    float h,v;

    public GameManager GM;
    Animator _animator;
    ShotManager _SM;
    Shot _currentShotType;

    public bool HasServed = false;

    Vector3 _AimTargetInitialPos;
    public bool GameStarted = false;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>(); 
        _animator = GameObject.Find("/Player/Raqueta").GetComponent<Animator>();
        _AimTargetInitialPos = AimTarget.position;
        _SM = GetComponent<ShotManager>();

        _currentShotType = _SM.topSpin;

    }

    void Update()
    {
        h = Input.GetAxis("Horizontal"); // Usando el sistema de Axis de unity, tomamos el input.
        v = Input.GetAxis("Vertical");

        if(Input.GetKeyDown(KeyCode.F) && HasServed == true) // Will return true if F is being held down.
        {
            hitting = true;
            _currentShotType = _SM.topSpin;
        } else if ((Input.GetKeyUp(KeyCode.F)) && HasServed == true)
        {
            hitting = false;
        }

        if(Input.GetKeyDown(KeyCode.C) && HasServed == true) // Will return true if C is being held down.
        {
            hitting = true;
            _currentShotType = _SM.flat;
        } else if ((Input.GetKeyUp(KeyCode.C)) && HasServed == true)
        {
            hitting = false;
        }

        // Serving

        if(Input.GetKeyDown(KeyCode.E) && HasServed == false && GameStarted) // Will return true if E is being held down.
        {
            hitting = true;
            _currentShotType = _SM.flatServe;
            GetComponent<BoxCollider>().enabled = false;
            _animator.Play("Serve-Prepare");
        }

        if(Input.GetKeyDown(KeyCode.R) && HasServed == false && GameStarted) // Will return true if E is being held down.
        {
            hitting = true;
            _currentShotType = _SM.kickServe;
            GetComponent<BoxCollider>().enabled = false;
            _animator.Play("Serve-Prepare");
        }

        if ((Input.GetKeyUp(KeyCode.E) || Input.GetKeyUp(KeyCode.R)) && HasServed == false && GameStarted) // Cuando se levanta la tecla.
        {
            //Debug.Log(HasServed);
            hitting = false;
            GetComponent<BoxCollider>().enabled = true;
            Ball.transform.position = transform.position + new Vector3(0.2f, 1, 0);
            _animator.Play("Serve");
            Vector3 _dir = AimTarget.position - transform.position; // Calculo el Vector desde el jugador al blanco al que apunto.
            Ball.GetComponent<Rigidbody>().velocity = _dir.normalized * _currentShotType.hitForce +  new Vector3 (0, _currentShotType.upForce, 0);
            HasServed = true;
        }

    }

    // Update is called once per frame
    void FixedUpdate() // Corre 60 veces x segundo, se usa para las fisicas.
    {
        MoveCharacter(hitting);        

        if (hitting)
        {
            AimTarget.Translate( new Vector3(h,0,0) * TargetSpeed * 2 * Time.deltaTime);
        }
    }

    void MoveCharacter(bool hitting)
    {
        if(!hitting)
        {
            if(Input.GetKey(keyPositiveH)){
                _rigidbody.velocity += v3ForceH;
            }
            if(Input.GetKey(keyNegativeH)){
                _rigidbody.velocity -= v3ForceH;
            }

            if(Input.GetKey(keyPositiveV)){
                _rigidbody.velocity += v3ForceV;
            }
            if(Input.GetKey(keyNegativeV)){
                _rigidbody.velocity -= v3ForceV;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Granade"))
        {
            GM.PlaySound();
            //Debug.Log("Boop");
            Vector3 _dir = AimTarget.position - transform.position; // Calculo el Vector desde el jugador al blanco al que apunto.
            other.GetComponent<Rigidbody>().velocity = _dir.normalized * _currentShotType.hitForce +  new Vector3 (0, _currentShotType.upForce, 0);

            // Vamos a usar el vector anterior de direccion para ver en que dir esta llendo la bola. Eje X

            Vector3 _ballDir = Ball.position - transform.position;

            if(_ballDir.x >= 0)
            {
                _animator.Play("Golpe Derecha");
            } else {
                _animator.Play("Golpe Izq");
            }

            // Reseteamos el Aim a su pos inicial despues de cada golpe.
            AimTarget.position = _AimTargetInitialPos;
        }
    }
}
