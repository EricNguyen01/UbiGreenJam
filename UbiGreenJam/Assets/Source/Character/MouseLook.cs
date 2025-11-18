using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MouseLook : CharacterComponentBase
{
    public float mouseSensitivity = 100f;

    public Transform characterTransform;

    float xRotation = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    { 
        base.Start();

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!enabled) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        characterTransform.Rotate(Vector3.up * mouseX);
    }

    public override bool InitCharacterComponentFrom(CharacterBase character)
    {
        if (!base.InitCharacterComponentFrom(character)) return false;

        characterTransform = character.transform;

        mouseSensitivity = character.characterSOData.mouseLookSensitivity;

        return true;
    }
}
