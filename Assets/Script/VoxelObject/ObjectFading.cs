using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFading : MonoBehaviour {

    [SerializeField] Color[] firstColor;
    [SerializeField] Color[] secondColor;
    [SerializeField] float transitionTime;

    private bool showNeeded;
    private bool hideNeeded;

    private MeshRenderer meshRenderer;
    private Coroutine currentCoroutine;

    // Use this for initialization
    void Start () {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        for (int i = 0; i < meshRenderer.materials.Length; ++i)
        {
            meshRenderer.materials[i].SetColor("_Color", new Color(0, 0, 0, 0));
            meshRenderer.materials[i].SetColor("_SColor", new Color(0, 0, 0, 0));
        }
        meshRenderer.enabled = false;
    }

    private void tryShow()
    {
        if (showNeeded == false)
            showNeeded = true;
        if (showNeeded == true && hideNeeded == true)
            hideNeeded = false;
    }

    private void tryHide()
    {
        if (hideNeeded == false)
            hideNeeded = true;
        if (hideNeeded == true && showNeeded == true)
            showNeeded = false;
    }

    public void show()
    {
        tryShow();
        if (currentCoroutine == null && showNeeded)
        {
            showNeeded = false;
            meshRenderer.enabled = true;
            currentCoroutine = StartCoroutine(Cshow());
        }
    }

    private IEnumerator Cshow()
    {
        for (int i = 0; i < meshRenderer.materials.Length; ++i)
        {
            meshRenderer.materials[i].SetColor("_Color", new Color(0, 0, 0, 0));
            meshRenderer.materials[i].SetColor("_SColor", new Color(0, 0, 0, 0));
        }
        float time = 0;
        float[] color = new float[4];
        float[] scolor = new float[4];
        while (time < 1)
        {
            color[0] = Mathf.Lerp(0, firstColor[0].r, time);
            color[1] = Mathf.Lerp(0, firstColor[0].g, time);
            color[2] = Mathf.Lerp(0, firstColor[0].b, time);
            color[3] = Mathf.Lerp(0, firstColor[0].a, time);
            scolor[0] = Mathf.Lerp(0, firstColor[1].r, time);
            scolor[1] = Mathf.Lerp(0, firstColor[1].g, time);
            scolor[2] = Mathf.Lerp(0, firstColor[1].b, time);
            scolor[3] = Mathf.Lerp(0, firstColor[1].a, time);
            for (int i = 0; i < meshRenderer.materials.Length; ++i)
            {
                meshRenderer.materials[i].SetColor("_Color", new Color(color[0], color[1], color[2], color[3]));
                meshRenderer.materials[i].SetColor("_SColor", new Color(scolor[0], scolor[1], scolor[2], scolor[3]));
            }
            time += Time.deltaTime / transitionTime;
            yield return new WaitForFixedUpdate();
        }
        time = 0;
        while (time < 1)
        {
            color[0] = Mathf.Lerp(firstColor[0].r, secondColor[0].r, time);
            color[1] = Mathf.Lerp(firstColor[0].r, secondColor[0].g, time);
            color[2] = Mathf.Lerp(firstColor[0].r, secondColor[0].b, time);
            color[3] = Mathf.Lerp(firstColor[0].r, secondColor[0].a, time);
            scolor[0] = Mathf.Lerp(firstColor[1].r, secondColor[1].r, time);
            scolor[1] = Mathf.Lerp(firstColor[1].r, secondColor[1].g, time);
            scolor[2] = Mathf.Lerp(firstColor[1].r, secondColor[1].b, time);
            scolor[3] = Mathf.Lerp(firstColor[1].r, secondColor[1].a, time);
            for (int i = 0; i < meshRenderer.materials.Length; ++i)
            {
                meshRenderer.materials[i].SetColor("_Color", new Color(color[0], color[1], color[2], color[3]));
                meshRenderer.materials[i].SetColor("_SColor", new Color(scolor[0], scolor[1], scolor[2], scolor[3]));
            }
            time += Time.deltaTime / transitionTime;
            yield return new WaitForFixedUpdate();
        }
        currentCoroutine = null;
        if (hideNeeded)
        {
            hideNeeded = false;
            currentCoroutine = StartCoroutine(Chide());
        }
        yield return 0;
    }


    public void hide()
    {
        tryHide();
        if (currentCoroutine == null && hideNeeded)
        {
            hideNeeded = false;
            currentCoroutine = StartCoroutine(Chide());
        }
    }

    private IEnumerator Chide()
    {
        float time = 0;
        float[] color = new float[4];
        float[] scolor = new float[4];
        while (time < 1)
        {
            color[0] = Mathf.Lerp(secondColor[0].r, firstColor[0].r, time);
            color[1] = Mathf.Lerp(secondColor[0].g, firstColor[0].g, time);
            color[2] = Mathf.Lerp(secondColor[0].b, firstColor[0].b, time);
            color[3] = Mathf.Lerp(secondColor[0].a, firstColor[0].a, time);
            scolor[0] = Mathf.Lerp(secondColor[1].r, firstColor[1].r, time);
            scolor[1] = Mathf.Lerp(secondColor[1].b, firstColor[1].g, time);
            scolor[2] = Mathf.Lerp(secondColor[1].g, firstColor[1].b, time);
            scolor[3] = Mathf.Lerp(secondColor[1].a, firstColor[1].a, time);
            for (int i = 0; i < meshRenderer.materials.Length; ++i)
            {
                meshRenderer.materials[i].SetColor("_Color", new Color(color[0], color[1], color[2], color[3]));
                meshRenderer.materials[i].SetColor("_SColor", new Color(scolor[0], scolor[1], scolor[2], scolor[3]));
            }
            time += Time.deltaTime / transitionTime;
            yield return new WaitForFixedUpdate();
        }
        time = 0;
        while (time < 1)
        {
            color[0] = Mathf.Lerp(firstColor[0].r, 0, time);
            color[1] = Mathf.Lerp(firstColor[0].g, 0, time);
            color[2] = Mathf.Lerp(firstColor[0].b, 0, time);
            color[3] = Mathf.Lerp(firstColor[0].a, 0, time);
            scolor[0] = Mathf.Lerp(firstColor[1].r, 0, time);
            scolor[1] = Mathf.Lerp(firstColor[1].g, 0, time);
            scolor[2] = Mathf.Lerp(firstColor[1].b, 0, time);
            scolor[3] = Mathf.Lerp(firstColor[1].a, 0, time);
            for (int i = 0; i < meshRenderer.materials.Length; ++i)
            {
                meshRenderer.materials[i].SetColor("_Color", new Color(color[0], color[1], color[2], color[3]));
                meshRenderer.materials[i].SetColor("_SColor", new Color(scolor[0], scolor[1], scolor[2], scolor[3]));
            }
            time += Time.deltaTime / transitionTime;
            yield return new WaitForFixedUpdate();
        }
        meshRenderer.enabled = false;
        currentCoroutine = null;
        if (showNeeded)
        {
            showNeeded = false;
            meshRenderer.enabled = true;
            currentCoroutine = StartCoroutine(Cshow());
        }
        yield return 0;
    }
}
