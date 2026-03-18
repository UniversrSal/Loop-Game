using UnityEngine;
using System.Collections;

public class IdleBored : MonoBehaviour
{
    private Animator animator;


    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(WaitThenBored());

    }

    IEnumerator WaitThenBored()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            animator.SetTrigger("Bored");
        }
    }
}