using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipAmbienceController : MonoBehaviour
{
    public enum ControlMode { Random, Manual }

    [Header("Sonidos Wwise")]
    // 1. Asigna aquí tu evento "Play_SpaceShip" (El de la captura 1)
    public AK.Wwise.Event spaceshipEngineEvent;

    // 2. Asigna aquí tu evento "Play_Random_Effects" (El de la captura 2)
    public AK.Wwise.Event randomEffectsEvent;

    // 3. Asigna aquí el RTPC "Space_Ship_grave"
    public AK.Wwise.RTPC intensityRTPC;

    [Header("Configuración de Intensidad")]
    [Range(0, 100)]
    public int startValue = 0;
    public float stepDelay = 0.1f;
    public int minValue = 0;
    public int maxValue = 100;

    [Header("Modo de Control")]
    public ControlMode mode = ControlMode.Random;

    [Header("Aleatorio")]
    public float changeInterval = 5f;

    [Header("Manual")]
    [Range(0, 100)]
    public int manualTarget = 50;

    private int targetValue;
    private int currentValue;
    private float timeSinceChange;
    private float timeSinceStep;

    void Start()
    {
        currentValue = startValue;
        targetValue = startValue;

        // --- ESTO ES LO QUE TE FALTABA ---
        // Iniciamos ambos sonidos al arrancar el objeto.
        // Al usar 'gameObject', los sonidos se "pegan" a esta nave.
        if (spaceshipEngineEvent != null)
            spaceshipEngineEvent.Post(gameObject);

        if (randomEffectsEvent != null)
            randomEffectsEvent.Post(gameObject);

        // Inicializamos el valor del RTPC
        UpdateRTPC();
    }

    void Update()
    {
        // 1. Lógica para decidir a qué valor ir (Target)
        if (mode == ControlMode.Random)
        {
            timeSinceChange += Time.deltaTime;
            if (timeSinceChange >= changeInterval)
            {
                targetValue = Random.Range(minValue, maxValue + 1);
                timeSinceChange = 0f;
            }
        }
        else
        {
            targetValue = Mathf.Clamp(manualTarget, minValue, maxValue);
        }

        // 2. Lógica para mover el valor actual hacia el target suavemente
        timeSinceStep += Time.deltaTime;
        if (timeSinceStep >= stepDelay)
        {
            timeSinceStep = 0f;

            if (currentValue != targetValue)
            {
                // Mueve currentValue hacia targetValue en pasos de 1
                currentValue = (int)Mathf.MoveTowards(currentValue, targetValue, 1);
                UpdateRTPC();
            }
        }
    }

    void UpdateRTPC()
    {
        // Enviamos el valor a Wwise.
        // Solo la Nave reaccionará porque es la única que tiene el RTPC configurado en Wwise.
        // Los efectos aleatorios ignorarán este mensaje y seguirán sonando normal.
        if (intensityRTPC != null)
        {
            intensityRTPC.SetValue(gameObject, currentValue);
        }
    }

    // Para probar sliders en el editor mientras juegas
    void OnValidate()
    {
        if (Application.isPlaying)
        {
            // Si estamos en modo manual, actualizamos al mover el slider
            if (mode == ControlMode.Manual)
            {
                targetValue = manualTarget; // Actualización inmediata en manual
            }
        }
    }

    private void OnDestroy()
    {
        Debug.Log("¡Destruyendo nave y matando audio!");

        // StopAll sin argumentos detiene TODO el audio del juego
        // Útil si cambias de escena y quieres silencio total antes de empezar la otra.
        AkSoundEngine.StopAll();
    }
}
