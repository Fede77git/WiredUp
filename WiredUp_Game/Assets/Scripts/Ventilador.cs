using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ventilador : MonoBehaviour
{
    public float fuerzaPlayer = 5f; 
    public float fuerzaCajaCarton = 8f;
    // flecha azul 
    public LayerMask capasQueTapanElViento;

    private void OnTriggerStay(Collider other)
    {

        Vector3 dirViento = transform.forward;

        // escudo?
        bool estaProtegido = Physics.Raycast(other.bounds.center, -dirViento, 4f, capasQueTapanElViento);

        if (!estaProtegido)
        {
            if (other.CompareTag("Player"))
            {
                
                other.transform.position += dirViento * fuerzaPlayer * Time.deltaTime;
            }
            else if (other.CompareTag("Caja"))
            {
                Rigidbody rbCaja = other.GetComponent<Rigidbody>();
                
                if (rbCaja != null && rbCaja.mass < 2f)
                {
                    rbCaja.AddForce(dirViento * fuerzaCajaCarton, ForceMode.Force);
                }
            }
        }
    }
}

