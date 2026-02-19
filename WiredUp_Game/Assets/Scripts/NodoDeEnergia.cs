using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodoDeEnergia : MonoBehaviour
{
    public bool activarMagnetismo; // true = modo atraccion, False = modo colgarse

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            CableSystem cable = other.GetComponent<CableSystem>();
            if (cable != null)
            {
                cable.CambiarEstadoElectrico(activarMagnetismo);
                Debug.Log("Energía cambiada. ¿Es magnético?: " + activarMagnetismo);
            }
        }
    }
}
