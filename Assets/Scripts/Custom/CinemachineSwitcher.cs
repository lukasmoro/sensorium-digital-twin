using UnityEngine;
using UnityEngine.InputSystem;

public class CinemachineSwitcher : MonoBehaviour
{   
    [SerializeField]
    private InputAction action;
    private Animator animator;
    private bool cameraFlag = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        action.Enable();
    }

    private void OnDisable()
    {
        action.Disable();
    }

    void Start()
    {
        action.performed += _ => SwitchState();
    }

    private void SwitchState()
    {
        if (cameraFlag)
        {
            animator.Play("Camera Virtual Twin 1");
        }
        else 
        {
            animator.Play("Camera Virtual Twin 2");
        }
        cameraFlag = !cameraFlag;
    }
}
