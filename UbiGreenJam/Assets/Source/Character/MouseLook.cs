using UnityEngine;

[DisallowMultipleComponent]
public class MouseLook : CharacterComponentBase
{
    [field: Header("Mouse Look Components")]

    [field: SerializeField]
    public Camera playerCam { get; private set; }

    private float mouseSensitivity = 100f;

    private Transform characterTransform;

    float xRotation = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    { 
        base.Start();

        Cursor.lockState = CursorLockMode.Locked;

        if (!playerCam)
        {
            Camera cam = Camera.main;

            Vector3 assumeHeadPos = transform.position + Vector3.up * 1.6f;

            if (characterUsingComponent && characterUsingComponent.GetType() == typeof(PlayerCharacter))
            {
                PlayerCharacter playerChar = (PlayerCharacter)characterUsingComponent;

                if(playerChar.characterMovement && playerChar.characterMovement.characterController)
                {
                    CharacterController charController = playerChar.characterMovement.characterController;

                    assumeHeadPos = transform.position + charController.center + new Vector3(0.0f, charController.height / 3.0f, 0.0f);
                }
            }

            if (!cam)
            {
                cam = new GameObject("PlayerCam").AddComponent<Camera>();
            }

            cam.transform.position = assumeHeadPos;

            cam.transform.SetParent(transform);

            playerCam = cam;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!enabled) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

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
