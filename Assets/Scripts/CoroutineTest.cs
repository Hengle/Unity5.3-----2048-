using UnityEngine;
using System.Collections;

public class CoroutineTest : MonoBehaviour
{
    private int coroutineCounter = 0;
    void Start()
    {
        TheMethod();

        StartCoroutine(TheCoroutine());

        StartCoroutine(TheObservingCoroutine());
    }

    private void TheMethod()
    {
        int i = 0;
        while (i<10)
        {
            Debug.Log("Method says " + i.ToString());
            i++;
        }
    }

    private IEnumerator TheCoroutine()
    {
        while (true)
        {
            Debug.Log("Coroutine says " + coroutineCounter.ToString());
            coroutineCounter++;
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator TheObservingCoroutine()
    {
        while (coroutineCounter < 10)
        {
            yield return null;
        }

        Debug.Log("TheObservingCoroutine says that is 10.");
    }
}
