using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour
{
    public GameObject explosionEffect;
    private GameManager gm;
    private Player Player1;

    private float delay = 600f;
    private float TrueDelay;
    float countdown;
    public float radius = 5f;
    public float force = 700f;

    public bool hasExploded = false;
    
    [SerializeField] private bool _TimeSet = false;

    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        Player1 = GameObject.Find("Player").GetComponent<Player>();
        countdown = delay;
        TrueDelay = Random.Range(10.0f, 20.0f);

        // Creo una referencia a Player 1 para acceder a HasServed y cambiar el tiempo de la Granada.
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime; //is the ammount of time past since we drew the last frame( Basicamente es como un contador de tiempo)
        if (countdown <= 0 && !hasExploded)
        {
            Explode();
            hasExploded = true;
            gm.NeedANewGranade = true;
        }
            if(!_TimeSet && Player1.HasServed){
                countdown = TrueDelay;
                _TimeSet = true;
        }
    }

    void Explode()
    {
        Debug.Log("Boom");
        // Play Boom Sound
        gm.PlayBoom();
        // Show explosion eff
        Instantiate(explosionEffect, transform.position, transform.rotation);
        // Find Nearby Objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        // Add Forces to them
        foreach(Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if(rb != null)
            {
                // Add Force
                rb.AddExplosionForce(force, transform.position, radius);
            }
        }

        // Delete granade.
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player Side"))
        {
            gm.CurrentCourtside = "Player Side";
        } else if(other.gameObject.CompareTag("Bot Side"))
        {
            gm.CurrentCourtside = "Bot Side";
        }
    }
}
