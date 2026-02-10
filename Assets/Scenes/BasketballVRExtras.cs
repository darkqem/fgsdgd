using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Rigidbody))]
public class BasketballVRExtras : MonoBehaviour
{
    [Header("Freeze (VR button)")]
    [Tooltip("Boolean action для заморозки мяча")]
    public SteamVR_Action_Boolean freezeAction;

    [Header("Respawn (VR button)")]
    [Tooltip("Boolean action для респавна мяча по кнопке")]
    public SteamVR_Action_Boolean respawnAction;

    [Tooltip("Какая рука слушает кнопки")]
    public SteamVR_Input_Sources inputSource = SteamVR_Input_Sources.Any;

    [Tooltip("Если true — freeze/respawn не сработают, пока мяч в руке")]
    public bool ignoreWhileHeld = true;

    [Header("Auto Respawn (optional)")]
    [Tooltip("Если включено — мяч респавнится, когда падает ниже minY")]
    public bool respawnWhenBelowY = true;

    [Tooltip("Если мяч упал ниже этого Y — респавн (если respawnWhenBelowY включен)")]
    public float minY = -5f;

    [Header("Respawn point")]
    [Tooltip("Точка респавна. Если пусто — будет стартовая позиция мяча")]
    public Transform respawnPoint;

    private Rigidbody rb;
    private Interactable interactable;

    private Vector3 startPos;
    private Quaternion startRot;

    private bool frozen;
    private bool prevKinematic;
    private RigidbodyConstraints prevConstraints;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        interactable = GetComponent<Interactable>();

        startPos = transform.position;
        startRot = transform.rotation;
    }

    void Update()
    {
        // Respawn по кнопке
        if (respawnAction != null && respawnAction.GetStateDown(inputSource))
        {
            if (ignoreWhileHeld && interactable != null && interactable.attachedToHand != null)
                return;

            Respawn();
            return;
        }

        // Freeze по кнопке
        if (freezeAction != null && freezeAction.GetStateDown(inputSource))
        {
            if (ignoreWhileHeld && interactable != null && interactable.attachedToHand != null)
                return;

            ToggleFreeze();
        }

        // Авто-респавн только по Y (по желанию)
        if (respawnWhenBelowY && transform.position.y < minY)
        {
            Respawn();
        }
    }

    private void ToggleFreeze()
    {
        frozen = !frozen;

        if (frozen)
        {
            prevKinematic = rb.isKinematic;
            prevConstraints = rb.constraints;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            rb.constraints = prevConstraints;
            rb.isKinematic = prevKinematic;
        }
    }

    public void Respawn()
    {
        // если заморожен — размораживаем
        if (frozen)
        {
            frozen = false;
            rb.constraints = prevConstraints;
            rb.isKinematic = prevKinematic;
        }

        Vector3 pos = (respawnPoint != null) ? respawnPoint.position : startPos;
        Quaternion rot = (respawnPoint != null) ? respawnPoint.rotation : startRot;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.SetPositionAndRotation(pos, rot);
    }
}
