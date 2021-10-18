using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeThrower : MonoBehaviour
{
    public float throw_Force = 40f;
    public GameObject granadePrefab;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            ThrowGranade();
        }
    }

    void ThrowGranade()
    {
        // Creamos la granada.
        GameObject granade = Instantiate(granadePrefab, transform.position, transform.rotation);
        // Le aplicamos una fuerza para lanzarla.
        Rigidbody rb =  granade.GetComponent<Rigidbody>();
        if(rb != null){
            // Agregamos una fuerza hacia adelante.
            rb.AddForce(transform.forward * throw_Force, ForceMode.VelocityChange);
        }
    }
}
