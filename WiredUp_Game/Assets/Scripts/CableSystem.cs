
using UnityEngine;
using UnityEngine.UI;

public class CableSystem : MonoBehaviour
{
    //base

    public float distanciaMaxima = 30f; 
    public LayerMask capasEnganchables;
    public Transform salidaDelCable;
    public Camera camaraPrincipal;
    public LineRenderer lineaCable;
    public float distanciaRuptura = 35f;

    public GameObject objetoEnganchado;


    //fisicas cable
    public float fuerzaResorte = 10f; // q tan fuerte tira (Spring)
    public float amortiguacion = 7f;  // q tanto frena el rebote (Damper)
    public float velocidadRebobinado = 5f; // Para subir paredes

    private Rigidbody rb;

    // UI
    public Image imagenMira;
    public Color colorNormal = Color.green;
    public Color colorMagnetico = Color.cyan;
    public Color colorCableNormal = Color.white;
    public Color colorCableMagnetico = Color.cyan;

    private SpringJoint joint; // La articulación física
    private float distanciaCuerdaInicial;
    private Vector3 puntoDeEnganche;
    public bool estaEnganchado = false;
    private RaycastHit hitDetectado;

    public PlayerController controladorMovimiento;


    //balanceo
    public float velocidadBalanceo = 10f; 
    public float impulsoSalto = 15f;


    public GameObject efectoImpacto;


    // estados electrico /magnetico

    public bool modoMagnetico = false; // se cambia mediante el Nodo de energiaa
    public float fuerzaDeAtraccion = 25f;
    public float distanciaAgarre = 2.5f;
    public float fuerzaLanzamiento = 15f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (lineaCable != null) lineaCable.enabled = false;
    }


    public void CambiarEstadoElectrico(bool activar)
    {
        modoMagnetico = activar;
    }

    void Update()
    {

        ActualizarMira();


        if (Input.GetMouseButtonDown(0))
        {
            DispararCable();
        }

        //  detectar soltar clic para cortar cable
        if (Input.GetMouseButtonUp(0))
        {
            CortarCable();
        }

        if (estaEnganchado && joint != null)
        {
            if (Input.GetMouseButton(1)) 
            {
                if (modoMagnetico)
                {
                    
                    joint.spring = fuerzaDeAtraccion;
                    joint.maxDistance = 0f;
                }
                else
                {
                    // 3
                    joint.maxDistance -= Time.deltaTime * 15f;

                    
                    if (joint.maxDistance < 2f) joint.maxDistance = 2f;
                }
            }
            else 
            {
                if (modoMagnetico)
                {
                    
                    joint.spring = 0f;
                }
               
            }
        }

        // corte de cable
        if (estaEnganchado)
        {
            float distanciaActual = 0f;
            if (modoMagnetico && joint != null && joint.connectedBody != null)
                distanciaActual = Vector3.Distance(transform.position, joint.connectedBody.transform.position);
            else
                distanciaActual = Vector3.Distance(transform.position, puntoDeEnganche);

            if (distanciaActual > distanciaRuptura)
            {
                CortarCable();
                Debug.Log("se corto el cable");
            }
        }



        if (estaEnganchado && Input.GetKeyDown(KeyCode.Space))
        {
            CortarCable();
            Vector3 impulso = camaraPrincipal.transform.forward + (Vector3.up * 0.5f);
            rb.AddForce(impulso.normalized * impulsoSalto, ForceMode.Impulse);
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
        if (Physics.Raycast(rayo, out hitDetectado, distanciaMaxima, capasEnganchables))
        {
            int capaTocada = hitDetectado.collider.gameObject.layer;
            int layerEnganchable = LayerMask.NameToLayer("Enganchable");
            int layerCaja = LayerMask.NameToLayer("Caja");

            // MODO NORMAL
           
            if (!modoMagnetico && capaTocada == layerEnganchable)
            {
                imagenMira.color = colorNormal;
            }
            // MODO MAGNETICO
            else if (modoMagnetico && capaTocada == layerCaja)
            {
                imagenMira.color = colorMagnetico;
            }
            else
            {
                imagenMira.color = Color.white;
            }
        }
        else
        {
            imagenMira.color = Color.white;
        }
    }


    void DispararCable()
    {
        
        if (imagenMira.color == Color.white) return;

        
        int capaTocado = hitDetectado.collider.gameObject.layer;

        
        int layerEnganchable = LayerMask.NameToLayer("Enganchable");
        int layerCaja = LayerMask.NameToLayer("Caja");

        puntoDeEnganche = hitDetectado.point;
        estaEnganchado = true;
        objetoEnganchado = hitDetectado.collider.gameObject;


        if (modoMagnetico && capaTocado == layerCaja)
        {
            // MODO ATRAER CAJA
            Rigidbody rbCaja = hitDetectado.collider.GetComponent<Rigidbody>();
            if (rbCaja != null)
            {
                joint = gameObject.AddComponent<SpringJoint>();
                joint.connectedBody = rbCaja;
                joint.autoConfigureConnectedAnchor = false;
                joint.anchor = new Vector3(0, 0.5f, distanciaAgarre);
                joint.connectedAnchor = Vector3.zero;

                joint.spring = 0f;
                joint.damper = 5f;
                joint.maxDistance = distanciaCuerdaInicial;

                // cambio el color del cable visual
                lineaCable.startColor = colorCableMagnetico;
                lineaCable.endColor = colorCableMagnetico;
            }
        }
        else if (!modoMagnetico && capaTocado == layerEnganchable)
        {
            // MODO COLGARSE 
            if (controladorMovimiento != null) controladorMovimiento.isSwinging = true;

            joint = gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;

            joint.anchor = Vector3.zero;
            joint.connectedAnchor = puntoDeEnganche;


            joint.maxDistance = distanciaCuerdaInicial;
            joint.minDistance = 0f;

            
            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lineaCable.startColor = colorCableNormal;
            lineaCable.endColor = colorCableNormal;


        }
        else
        {
            // Si tocamos algo que no corresponde al modo actual, cancelamos
            estaEnganchado = false;
        }

        if (estaEnganchado && lineaCable != null) lineaCable.enabled = true;
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

        // si el cable existe y hay una caja conectad
        if (modoMagnetico && joint != null && joint.connectedBody != null)
        {
            Rigidbody rbCaja = joint.connectedBody;

            // hacia dodne mira el player
            Vector3 direccionTiro = camaraPrincipal.transform.forward;

            // impulso a la caja para q vuele
            rbCaja.AddForce(direccionTiro * fuerzaLanzamiento, ForceMode.Impulse);
        }


        estaEnganchado = false;
        if (controladorMovimiento != null) controladorMovimiento.isSwinging = false;
        if (lineaCable != null) lineaCable.enabled = false;

        // se destruye el joint para soltar
        if (joint != null)
        {
            Destroy(joint);
        }

        objetoEnganchado = null;

    }

    void DibujarCable()
    {
        lineaCable.SetPosition(0, salidaDelCable.position);
        
        if (modoMagnetico && joint != null && joint.connectedBody != null)
        {
            lineaCable.SetPosition(1, joint.connectedBody.transform.position);
        }
        else
        {
            lineaCable.SetPosition(1, puntoDeEnganche);
        }
    }
}
