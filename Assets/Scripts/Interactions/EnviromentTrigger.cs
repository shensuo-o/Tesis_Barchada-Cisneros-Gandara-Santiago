using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentTrigger : MonoBehaviour
{
    [SerializeField] private string[] allowedOximorons;
    [SerializeField] private Animator animator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 11)
        {
            for (int i = 0; i < allowedOximorons.Length; i++)
            {
                if (allowedOximorons[i] == other.GetComponent<StatsOximorones>().oxiName)
                {
                    StartCoroutine(ActivateMoveAnim());
                }
            }
        }
    }

    public IEnumerator ActivateMoveAnim()
    {
        animator.SetBool("Move", true);   
        yield return new WaitForSeconds(5f);
        animator.SetBool("Move", false); 
    }

}
