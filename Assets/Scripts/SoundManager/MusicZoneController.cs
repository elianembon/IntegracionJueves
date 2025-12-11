using UnityEngine;

public class MusicZoneController : MonoBehaviour
{
    [Header("Configuración de Zona")]
    // Aquí arrastrarás el Estado específico (ej: Zoologico) desde el Wwise Picker
    public AK.Wwise.State musicState;

    // Detectar cuando el jugador entra en la zona
    private void OnTriggerEnter(Collider other)
    {
        // Verificamos que sea el Jugador (y no una bala o un enemigo)
        if (other.CompareTag("Player"))
        {
            if (musicState != null)
            {
                Debug.Log("Cambiando música a: " + musicState.Name);
                musicState.SetValue(); // Esto le dice a Wwise que cambie el estado
            }
        }
    }
}
