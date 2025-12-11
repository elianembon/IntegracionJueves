using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Wwise Music Event")]
    // Arrastra aquí el evento 'Play_Musica_Juego'
    public AK.Wwise.Event musicEvent;

    void Start()
    {
        // Iniciamos la música. 
        // Usamos 'gameObject' para que el sonido venga de este manager 
        // (o déjalo global si el evento en Wwise no tiene posicionamiento 3D).
        if (musicEvent != null)
        {
            musicEvent.Post(gameObject);
        }
    }
}
