
using UnityEngine;
using UnityEngine.UI;

public class CableSystem : MonoBehaviour
{
    
    public float distanciaMaxima = 30f; 
    public LayerMask capasEnganchables;
    public Transform salidaDelCable;
    public Camera camaraPrincipal;
    public LineRenderer lineaCable;


    public float fuerzaResorte = 10f; // q tan fuerte tira (Spring)
    public float amortiguacion = 7f;  // q tanto frena el rebote (Damper)
    public float velocidadRebobinado = 5f; // Para subir paredes

    private Rigidbody rb;

    public Image imagenMira; 
    public Color colorRangoValido = Color.green;
    public Color colorRangoInvalido = Color.white;

    private SpringJoint joint; // La articulación física
    private Vector3 puntoDeEnganche;
    private bool estaEnganchado = false;
    private RaycastHit hitDetectado;

    public PlayerController controladorMovimiento;


    //balanceo
    public float velocidadBalanceo = 10f; 
    public float impulsoSalto = 15f;


    public GameObject efectoImpacto;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {

        ActualizarMira();


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


        if (estaEnganchado && Input.GetKeyDown(KeyCode.Space))
        {
            CortarCable();
            //  empujon
            Vector3 direccionSalto = camaraPrincipal.transform.forward + Vector3.up;
            rb.AddForce(direccionSalto.normalized * impulsoSalto, ForceMode.Impulse);
        }

        // dibujar el cable si esta activo
        if (estaEnganchado)
        {
            DibujarCable();


        }
    }


    void FixedUpdate()
    {
        if (estaEnganchado)
        {
            
            Vector3 direccionAlGancho = puntoDeEnganche - transform.position;
            direccionAlGancho.y = 0; 

            if (direccionAlGancho != Vector3.zero)
            {
                Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionAlGancho);

               
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, rotacionObjetivo, Time.fixedDeltaTime * 5f));
            }
        }



    }

    void ActualizarMira()
    {
        // se lanza un rayo invisible todo el tiempo para ver q hay en el centro
        Ray rayo = camaraPrincipal.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        // if el rayo pega en algo valido dentro de la distancia
        if (Physics.Raycast(rayo, out hitDetectado, distanciaMaxima, capasEnganchables))
        {
            if (imagenMira != null) imagenMira.color = colorRangoValido; // Verde
        }
        else
        {
            if (imagenMira != null) imagenMira.color = colorRangoInvalido; // Blanco
        }
    }


    void DispararCable()
    {
        // usamos el hitDetectado que ya calculamos en ActualizarMira
        // check si la mira estaba en verde 
        if (imagenMira.color == colorRangoValido)
        {
            puntoDeEnganche = hitDetectado.point;
            estaEnganchado = true;
           
            controladorMovimiento.isSwinging = true;

            if (efectoImpacto != null)
            {
                Instantiate(efectoImpacto, puntoDeEnganche, Quaternion.identity);
            }

            // robot mira hacia el punto de impacto 
            //Vector3 objetivoMirada = new Vector3(puntoDeEnganche.x, transform.position.y, puntoDeEnganche.z);
            //transform.LookAt(objetivoMirada);

            // joint
            joint = gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = puntoDeEnganche;

            float distanciaAlPunto = Vector3.Distance(transform.position, puntoDeEnganche);
            joint.maxDistance = distanciaAlPunto * 0.8f;
            joint.minDistance = 0f;
            joint.spring = fuerzaResorte;
            joint.damper = amortiguacion;
            joint.massScale = 4.5f;

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
        controladorMovimiento.isSwinging = false;
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
