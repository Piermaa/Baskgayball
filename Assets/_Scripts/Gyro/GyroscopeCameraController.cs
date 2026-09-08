using UnityEngine;
// Usamos el namespace del nuevo sistema de entrada
using UnityEngine.InputSystem;
using Gyroscope = UnityEngine.InputSystem.Gyroscope;
public class PureGyroController : MonoBehaviour
{
    void Start()
    {
        if (Gyroscope.current != null)
        {
            InputSystem.EnableDevice(Gyroscope.current);

            Gyroscope.current.Setup();

            Gyroscope.current.samplingFrequency = 50f;

        }
        else
        {
            Debug.LogError("Este dispositivo no cuenta con hardware de Giroscopio.");
        }
    }

    void Update()
    {
        bool hasCurrebt = Gyroscope.current != null;
        bool isenabled = Gyroscope.current.enabled;
        if (Gyroscope.current != null)
        {
            Vector3 angularVelocity = Gyroscope.current.angularVelocity.ReadValue();

            print($"Angular Velocity: {angularVelocity}");

            transform.Rotate(-angularVelocity.x * Time.deltaTime * Mathf.Rad2Deg,
                             -angularVelocity.y * Time.deltaTime * Mathf.Rad2Deg,
                             0);
        }
    }
}