using UnityEngine.Events;
using UnityEngine;

public class InterruptorPresion : MonoBehaviour
{

    //visual
    public Renderer meshBoton;
    public Color colorApagado = Color.red;
    public Color colorEncendido = Color.green;

    //eventos
    public UnityEvent alPresionar;
    public UnityEvent alSoltar;

    private int objetosEncima = 0;

    void Start()
    {
        if (meshBoton != null) meshBoton.material.color = colorApagado;
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Caja") || other.CompareTag("Player"))
        {
            objetosEncima++;
            if (objetosEncima == 1) 
            {
                if (meshBoton != null) meshBoton.material.color = colorEncendido;
                alPresionar.Invoke();
                
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Caja") || other.CompareTag("Player"))
        {
            objetosEncima--;
            if (objetosEncima <= 0)
            {
                objetosEncima = 0;
                if (meshBoton != null) meshBoton.material.color = colorApagado;
                alSoltar.Invoke();
                
            }
        }
    }
}
