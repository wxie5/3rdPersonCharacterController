using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimController : MonoBehaviour
{
    protected Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        Initialize();
    }

    private void Update()
    {
        AnimCtrlUpdate();
    }

    protected virtual void Initialize(){ }

    protected virtual void AnimCtrlUpdate(){ }
}
