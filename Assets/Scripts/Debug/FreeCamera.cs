using System.IO;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CameraController))]
public class FreeCamera : MonoBehaviour
{
    [SerializeField] private float _speed;

    private CameraController _controller;
    private bool _enabled = false;

    private void Awake() => _controller = GetComponent<CameraController>();

    private void OnP()
    {
        if (!Debug.isDebugBuild || !Application.isEditor) return;

        _enabled = !_enabled;
        _controller.SetControlBlock(!_enabled, !_enabled, !_enabled);
        _controller.enabled = !_enabled;
    }

    private void OnI()
    {
        if (!Debug.isDebugBuild || !Application.isEditor) return;

        string directoryPath = Path.Combine(Application.dataPath, "Images");
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);
        string path = Path.Combine(directoryPath, $"{Directory.GetFiles(directoryPath).Length + 1}.png");
        if (File.Exists(path))
            OnI();
        ScreenCapture.CaptureScreenshot(path);
    }

    private void FixedUpdate()
    {
        if (!_enabled) return;

        Vector3 motion = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            motion += transform.forward;
        else if (Input.GetKey(KeyCode.S))
            motion -= transform.forward;

        if (Input.GetKey(KeyCode.D))
            motion += transform.right;
        else if (Input.GetKey(KeyCode.A))
            motion -= transform.right;

        if (Input.GetKey(KeyCode.Space))
            motion += Vector3.up;
        else if (Input.GetKey(KeyCode.LeftControl))
            motion += Vector3.down;


        Vector3 mouse = new(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), -transform.eulerAngles.z);

        transform.position += _speed * Time.deltaTime * motion;
        transform.Rotate(mouse);
    }
}
