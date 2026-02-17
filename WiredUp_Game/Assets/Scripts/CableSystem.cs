using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableSystem : MonoBehaviour
{
    
    public float distanciaMaxima = 20f; 
    public LayerMask capasEnganchables;
    public Transform salidaDelCable;
    public Camera camaraPrincipal;
    public LineRenderer lineaCable;


    public float fuerzaResorte = 10f; // q tan fuerte tira (Spring)
    public float amortiguacion = 7f;  // q tanto frena el rebote (Damper)
    public float velocidadRebobinado = 5f; // Para subir paredes

    private SpringJoint joint; // La articulación física
    private Vector3 puntoDeEnganche;
    private bool estaEnganchado = false;
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            DispararCable();
        }

        //  detectar soltar Clic para cortar cable
        if (Input.GetMouseButtonUp(0))
        {
            CortarCable();
        }

        if (estaEnganchado && Input.GetMouseButton(1))
        {
            RebobinarCable();
        }

        // dibujar el cable si esta activo
        if (estaEnganchado)
        {
            DibujarCable();
        }
    }

    void DispararCable()
    {
        // lanzamos un rayo invisible desde la mira
        Ray rayo = camaraPrincipal.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(rayo, out hit, distanciaMaxima, capasEnganchables))
        {
            puntoDeEnganche = hit.point;
            estaEnganchado = true;

            
            // agregamos el componente SpringJoint al jugador 
            joint = gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = puntoDeEnganche;

            // cable tenso
            float distanciaAlPunto = Vector3.Distance(transform.position, puntoDeEnganche);

            joint.maxDistance = distanciaAlPunto * 0.8f; // mas corto mas tirante
            joint.minDistance = 0f; // puede acortarse hasta 0

            joint.spring = fuerzaResorte;   
            joint.damper = amortiguacion;   // resistencia
            joint.massScale = 1f;         // scala de masa para que no dependa tanto del peso del robot

            lineaCable.enabled = true;
        }
    }


    void RebobinarCable()
    {
        // if tenemos un joint, reducimos su distancia máxima para subir
        if (joint != null)
        {
            joint.maxDistance -= velocidadRebobinado * Time.deltaTime;

            // evitar negativ
            if (joint.maxDistance < 0) joint.maxDistance = 0;
        }
    }

    void CortarCable()
    {
        estaEnganchado = false;
        lineaCable.enabled = false;

        // se destruye el joint para soltar
        if (joint != null)
        {
            Destroy(joint);
        }

    }

    void DibujarCable()
    {
        // si no hay joint no dibuja
        if (!joint) return;
        // punto A: El Robot , o punto de origen
        lineaCable.SetPosition(0, salidaDelCable.position);

        // punto B: donde pega el rayo
        lineaCable.SetPosition(1, puntoDeEnganche);
    }
}
