using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NodoReceptor : MonoBehaviour
{
    public Renderer meshReceptor;
    public Color colorApagado = Color.black;
    public Color colorEncendido = Color.yellow;

    public UnityEvent alRecibirEnergia;
    private bool yaSeActivo = false;

    void Start()
    {
        if (meshReceptor != null) meshReceptor.material.color = colorApagado;
    }

    public void Encender()
    {
        if (!yaSeActivo)
        {
            yaSeActivo = true; 
            if (meshReceptor != null) meshReceptor.material.color = colorEncendido;

            alRecibirEnergia.Invoke(); // puerta o mecanismo abierto
            
        }
    }
}
