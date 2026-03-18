using UnityEngine;
using System.Collections;

public class IdleYawn : MonoBehaviour
{
    private Animator animator; 


    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(WaitThenYawn());

    }

    IEnumerator WaitThenYawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(5.6f);
            animator.SetTrigger("Yawn");
        }
    }
}