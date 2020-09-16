using UnityEngine;

public class CameraController : MonoBehaviour
{
    [HideInInspector] public Transform target = null;
    [Tooltip("Whether or not to use the custom camera offset or using the current camera position in the scene.")]
    [SerializeField] private bool useCustomCameraOffset = false;
    [Tooltip("The camera's offset in relation to the player.")]
    public Vector3 offset = Vector3.zero;
    [Tooltip("The transform the camera is forced to look at.")]
    public Transform pivot = null;
    [Tooltip("Maximum amount of degrees above the world's horizontal axis to be able to angle the camera.")]
    [SerializeField] private float maxVerticalDegrees = 60f;

    [Tooltip("Minimum amount of degrees above the world's horizontal axis to be able to angle the camera.")]
    [SerializeField] private float minVerticalDegrees = 0f;
    private float minVerticalDegreesResult = 0f;

    [Tooltip("Whether moving the mouse vertically should move the camera up or down.")]
    [SerializeField] private bool useInvertedVerticalRotation = false; //TODO: Should be taken from game settings


    [Tooltip("The standard rotation speed of the camera.")]
    [SerializeField] private float rotateSpeed = 3f;
    [SerializeField] private string mouseXInputName = "Mouse X";
    [SerializeField] private string mouseYInputName = "Mouse Y";

    [HideInInspector] public static CameraController instance = null;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    private void Start()
    {
        //Finding target
        target = FindObjectOfType<PlayerController>().transform;

        //Check if offset should be the custom value or the camera's position in the scene
        if (!useCustomCameraOffset) { offset = target.position - transform.position; }

        InitPivot();
        Cursor.lockState = CursorLockMode.Locked; //FIX Cursor logic should be in its own script

        //Assuming we don't want to change minVerticalDegrees:
        minVerticalDegreesResult = 360 - minVerticalDegrees;
    }
    private void LateUpdate()
    {

        pivot.transform.position = target.transform.position;


        //Get vertical input
        float vertical = Input.GetAxisRaw(mouseYInputName) * rotateSpeed;
        if (useInvertedVerticalRotation) { pivot.Rotate(vertical, 0, 0); }
        else { pivot.Rotate(-vertical, 0, 0); }

        //Get horizontal input
        float horizontal = Input.GetAxisRaw(mouseXInputName) * rotateSpeed;
        pivot.Rotate(0, horizontal, 0);

        LimitVerticalRotation();

        float desiredXAngle = pivot.eulerAngles.x;
        float desiredYAngle = pivot.eulerAngles.y;

        //Apply the rotation result to the camera
        Quaternion rotation = Quaternion.Euler(desiredXAngle, desiredYAngle, 0);
        transform.position = target.position - (rotation * offset);

        transform.LookAt(target);
        
    }
    private void InitPivot()
    {
        //Camera pivot is now an orphan
        pivot.transform.parent = null;
    }
    private void LimitVerticalRotation()
    {
        if (pivot.rotation.eulerAngles.x > maxVerticalDegrees && pivot.rotation.eulerAngles.x < 180) { pivot.rotation = Quaternion.Euler(maxVerticalDegrees, pivot.eulerAngles.y, 0f); }
        if (pivot.rotation.eulerAngles.x < minVerticalDegreesResult && pivot.rotation.eulerAngles.x > 180) { pivot.rotation = Quaternion.Euler(minVerticalDegreesResult, pivot.eulerAngles.y, 0f); }
    }
}
