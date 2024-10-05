using System.Collections;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    [SerializeField]
    private GameObject selectBorder;
    [SerializeField]
    private GameObject hpUi;

    private Animator animator;

    private bool isSelected = false;
    private bool targetIsInteractable = false;
    private Coroutine currentCoroutine = null;
    private float lastMoveTime = 0f;
    private float lastAttackTime = 0f;
    private int gatheredResource = 0;

    void Start()
    {
        if (selectBorder == null)
        {
            Debug.LogError($"{transform.name} is selectable but doesn't have selectBorder object assigned!");
        }

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        selectBorder.SetActive(isSelected);
        hpUi.SetActive(isSelected);
    }

    public void Select()
    {
        isSelected = true;
    }

    public void Deselect()
    {
        isSelected = false;
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public void Attack()
    {
        animator.SetBool("isAttacking", true);
    }

    public void Idle()
    {
        animator.SetBool("isAttacking", false);
    }

    public void SetOngoingCoroutine(Coroutine coroutine)
    {
        currentCoroutine = coroutine;
    }

    public Coroutine GetCurrentCoroutine()
    {
        return currentCoroutine;
    }

    public void SetTargetInteractable(bool targetIsInteractable)
    {
        this.targetIsInteractable = targetIsInteractable;
    }

    public bool TargetIsInteractable()
    {
        return targetIsInteractable;
    }

    public void Gather()
    {
        gatheredResource++;
    }

    public int Dump()
    {
        int tmp = gatheredResource;
        gatheredResource = 0;
        return tmp;
    }

    public float LastMoveTime
    {
        get { return lastMoveTime; }
        set { lastMoveTime = value; }
    }

    public float LastAttackTime
    {
        get { return lastAttackTime; }
        set { lastAttackTime = value; }
    }
}
