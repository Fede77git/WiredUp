using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergiaRobot : MonoBehaviour
{
    public CableSystem cable;
    public Renderer meshRobot;
    public Color colorApagado = Color.white;
    public Color colorEnergizado = Color.yellow;


    public float tiempoBateria = 3f; 
    private float temporizador = 0f;

    //estado no tocar
    public bool tieneEnergia = false;

    private bool tocandoEmisor = false;
    private GameObject objetoTocado = null;

    void Update()
    {
        bool cableEnEmisor = cable.estaEnganchado && cable.objetoEnganchado != null && cable.objetoEnganchado.CompareTag("Emisor");

        
        if (tocandoEmisor || cableEnEmisor)
        {
            temporizador = tiempoBateria; 
        }
        else
        {
            temporizador -= Time.deltaTime; 
        }

        bool estadoAnterior = tieneEnergia;
        tieneEnergia = temporizador > 0f; 


        
        if (meshRobot != null)
        {
            meshRobot.material.color = tieneEnergia ? colorEnergizado : colorApagado;
        }

        
        if (tieneEnergia)
        {
            bool cableEnReceptor = cable.estaEnganchado && cable.objetoEnganchado != null && cable.objetoEnganchado.CompareTag("Receptor");
            bool tocandoReceptor = objetoTocado != null && objetoTocado.CompareTag("Receptor");

            if (cableEnReceptor) ActivarPuerta(cable.objetoEnganchado);
            if (tocandoReceptor) ActivarPuerta(objetoTocado);
        }
    }

    // q toca con cuerpo
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Emisor")) tocandoEmisor = true;
        objetoTocado = col.gameObject;
    }

    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag("Emisor")) tocandoEmisor = false;
        if (objetoTocado == col.gameObject) objetoTocado = null;
    }

    private void ActivarPuerta(GameObject receptor)
    {
        NodoReceptor nodo = receptor.GetComponent<NodoReceptor>();
        if (nodo != null) nodo.Encender();
    }
}
