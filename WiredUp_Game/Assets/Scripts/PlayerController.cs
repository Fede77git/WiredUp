using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
   
    public float velocidad = 5f;
    public float velocidadRotacion = 15f;

    public float fuerzaSalto = 12f; 
    public float alturaDeteccionSuelo = 1.1f; 
    public LayerMask capasSuelo;

    public Transform camaraPrincipal; // Main Camera

    private Rigidbody rb;
    private Vector2 inputMovimiento;
    private bool estaEnElSuelo = false;

    public bool isSwinging = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        inputMovimiento = new Vector2(x, z).normalized;

       
        estaEnElSuelo = Physics.Raycast(transform.position, Vector3.down, alturaDeteccionSuelo, capasSuelo);

        
        Debug.DrawRay(transform.position, Vector3.down * alturaDeteccionSuelo, estaEnElSuelo ? Color.green : Color.red);

        
        if (Input.GetKeyDown(KeyCode.Space) && estaEnElSuelo)
        {
            Saltar();
        }


    }

    void FixedUpdate()
    {
        if (!isSwinging)
        {
            MoverJugador();
        }


    }

    void MoverJugador()
    {


        if (inputMovimiento.magnitude >= 0.1f)
        {
            // Calculamos el ángulo hacia donde mira la cámara
            float anguloObjetivo = Mathf.Atan2(inputMovimiento.x, inputMovimiento.y) * Mathf.Rad2Deg + camaraPrincipal.eulerAngles.y;

            // Suavizamos el giro del robott
            float anguloSuave = Mathf.LerpAngle(transform.eulerAngles.y, anguloObjetivo, velocidadRotacion * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Euler(0f, anguloSuave, 0f);

            // Movemos hacia adelante en la direccion que quedamos mirandoo
            Vector3 direccionMovimiento = Quaternion.Euler(0f, anguloObjetivo, 0f) * Vector3.forward;

            // Aplicamos velocidad manteniendo la gravedad 
            rb.velocity = new Vector3(direccionMovimiento.x * velocidad, rb.velocity.y, direccionMovimiento.z * velocidad);
        }
        else
        {
            // Si no tocamos nada se frena horizontalmente 
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    void Saltar()
    {
        
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

       
        rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
    }

}
