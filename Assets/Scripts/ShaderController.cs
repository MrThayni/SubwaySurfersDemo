using System.Collections;
using UnityEngine;

public class ShaderValueModifier : MonoBehaviour
{
    [SerializeField] private Material[] materials;
    private float duration = 2f;
    private float minValue = -1f;
    private float maxValue = 1f;
    private float elapsedTime = 0f;
    private float waitForDelay = 3f;
    private float currentValue1;
    private float currentValue2;
    private float modifiedValue1;
    private float modifiedValue2;
    GameObject gameManager;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController");
    }

    private void Start()
    {
        foreach (var m in materials)
        {
            m.SetFloat(Shader.PropertyToID("_CurveX"), 0);
            m.SetFloat(Shader.PropertyToID("_CurveY"), 0);
        }
        StartCoroutine(ModifyShaderProperty());

        if (gameManager.GetComponent<GameManager>().isAlive)
        {
            StopCoroutine(ModifyShaderProperty());
        }
    }

    private IEnumerator ModifyShaderProperty()
    {
        yield return new WaitForSeconds(6);

        while (true)
        {
            float targetValue1 = Random.Range(minValue, maxValue);
            float targetValue2 = Random.Range(minValue, maxValue);
            if (currentValue1 < 0 && targetValue1 < 0)
            {
                targetValue1 += 1;
            }
            else if(currentValue1 > 0 && targetValue1 > 0)
            {
                targetValue1 -= 1;
            }
            if (currentValue2 < 0 && targetValue2 < 0)
            {
                targetValue1 += 1;
            }
            else if (currentValue2 > 0 && targetValue2 > 0)
            {
                targetValue1 -= 1;
            }
            else
            {
                float startTime = Time.time;

                while (Time.time - startTime < duration)
                {
                    elapsedTime = Time.time - startTime;
                    modifiedValue1 = Mathf.Lerp(currentValue1, targetValue1, duration * Time.deltaTime);
                    modifiedValue2 = Mathf.Lerp(currentValue2, targetValue2, duration * Time.deltaTime);
                    currentValue1 = modifiedValue1;
                    currentValue2 = modifiedValue2;

                    foreach (var m in materials)
                    {
                        m.SetFloat(Shader.PropertyToID("_CurveX"), modifiedValue1);
                        m.SetFloat(Shader.PropertyToID("_CurveY"), modifiedValue2);
                    }
                    yield return null;
                }
                yield return new WaitForSeconds(waitForDelay);
            }
        }
    }
}

