using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergiaRobot : MonoBehaviour
{
    public CableSystem cable;
    public Renderer meshRobot;
    public Color colorApagado = Color.white;
    public Color colorEnergizado = Color.yellow;

    //estado no tocar
    public bool tieneEnergia = false;

    private bool tocandoEmisor = false;
    private GameObject objetoTocado = null;

    void Update()
    {
        // check si toca emisor con corriente
        bool cableEnEmisor = cable.estaEnganchado && cable.objetoEnganchado != null && cable.objetoEnganchado.CompareTag("Emisor");

        // robot con energia
        tieneEnergia = tocandoEmisor || cableEnEmisor;

        // feedback visual
        if (meshRobot != null)
        {
            meshRobot.material.color = tieneEnergia ? colorEnergizado : colorApagado;
        }

        // completa el circuito? 7 9
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

    // abrir mecanismo o puierta
    private void ActivarPuerta(GameObject receptor)
    {
        NodoReceptor nodo = receptor.GetComponent<NodoReceptor>();
        if (nodo != null) nodo.Encender();
    }
}
