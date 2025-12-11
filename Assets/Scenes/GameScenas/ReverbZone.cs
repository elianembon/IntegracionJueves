using UnityEngine;

public class ReverbZone : MonoBehaviour
{
    [Header("Wwise State Configuration")]
    [Tooltip("Nombre exacto del State Group en Wwise")]
    [SerializeField] private string stateGroup = "Reverbs";

    [Tooltip("Nombre del Estado cuando se ENTRA en la zona")]
    [SerializeField] private string onState = "ReverOn";

    [Tooltip("Nombre del Estado cuando se SALE de la zona")]
    [SerializeField] private string offState = "ReverOff";

    private void OnTriggerEnter(Collider other)
    {
        // Verificamos si es el jugador quien entra
        if (other.CompareTag("Player"))
        {
            // Activamos el estado de Reverb
            AkSoundEngine.SetState(stateGroup, onState);
            Debug.Log($"Wwise State: {stateGroup} -> {onState}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Desactivamos el estado (volvemos a Off o Default)
            AkSoundEngine.SetState(stateGroup, offState);
            Debug.Log($"Wwise State: {stateGroup} -> {offState}");
        }
    }
}