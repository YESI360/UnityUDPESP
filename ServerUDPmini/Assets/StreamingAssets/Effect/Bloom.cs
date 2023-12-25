using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Bloom")]
public class Bloom : MonoBehaviour
{
	public Bloom scriptbloom;

	[Range(0.0f, 1.5f)] public float threshold = 0.07f;
	[Range(0.00f, 4.0f)] public float intensity = 0.54f;//182.60f;
	public float intensityMax;
	public float intensityMin;
	[Range(0,3600)]
	public float intesityTransitionTime;

	[Range(0.25f, 5.5f)] public float blurSize = 1.0f;
	public float blurSizeMax;
	public float blurSizeMin;
	[Range(0, 3600)]
	public float blurTransitionTime;
	[Range(1, 4)] public int blurIterations = 2;
	public Material fastBloomMaterial = null;

	public void Update()
	{
		//if (Input.GetKey("v")) // apagar
		//{
		//	Debug.Log("BloomOff");
		//	scriptbloom.enabled = false;			
		//}
		//if (Input.GetKey("y")) // prender
		//{
		//	Debug.Log("BloomOn");
		//	scriptbloom.enabled = true;
		//}
		///////////////////////////////////////////
		//if (Input.GetKey("1")) // (Input.GetKeyDown("1"))//
		//{
		//	ChangeGlow(intensityMax,intesityTransitionTime);
		//}
		//if (Input.GetKey("2")) // (Input.GetKeyDown("1"))//
		//{
		//	ChangeGlow(intensityMin, intesityTransitionTime);
		//}
		//if (Input.GetKey("3")) // (Input.GetKeyDown("1"))//
		//{
		//	ChangeBlur(blurSizeMax, blurTransitionTime);
		//}
        //if (Input.GetKey("4")) // (Input.GetKeyDown("1"))//
        //{
        //    ChangeBlur(blurSizeMin,blurTransitionTime);
        //}

    }
    public void ChangeGlow(float destineValue, float transitionTime)
	{
		StartCoroutine(SoftChangeValue(intensityMin, intensityMax, destineValue, true, transitionTime));
	}

	public void ChangeBlur(float destineValue, float transitionTime) {
		StartCoroutine(SoftChangeValue(blurSizeMin, blurSizeMax, destineValue, false, transitionTime));
	}


    private IEnumerator SoftChangeValue(float minValue, float maxValue, float valueToSet, bool isIntensity, float transitionTime) {

		// inicializamos cositas
		float timeElapsed = 0;
		float distancePerCycle = 0;
		float distanceToValue = 0;
		float initValue = 0;

		// Comprobamos que no se vaya de los margenes
		if (valueToSet < minValue) { valueToSet = minValue; }
		else if (valueToSet > maxValue) { valueToSet = maxValue; }

		if (transitionTime < 0) { transitionTime = 0; }
		else if (transitionTime > 3600) { transitionTime = 3600; }

		if (isIntensity) {

			// Si el tiempo de transicion es 0, entonces cambiamos el valor inmediatamente
			if (transitionTime <= 0) {
				intensity = valueToSet;
			}
			else {

			    distanceToValue = Mathf.Abs(intensity - valueToSet);
				initValue = intensity;

				// Cambiamos paulatinamente la intensidad
				while (true) {
					if (timeElapsed > transitionTime) { break; }
					float toLerp = timeElapsed / transitionTime;
					distancePerCycle = Mathf.Lerp(0, distanceToValue, toLerp);
					if (intensity > valueToSet) { distancePerCycle *= -1; }
					intensity =  initValue + distancePerCycle;
					yield return new WaitForFixedUpdate();
					timeElapsed += Time.deltaTime;
				}
			}
		}
		else {
			// Si el tiempo de transicion es 0, entonces cambiamos el valor inmediatamente
			if (transitionTime <= 0) {
				blurSize = valueToSet;
			}
			else {
				distanceToValue = Mathf.Abs(blurSize - valueToSet);

				initValue = blurSize;

				// Cambiamos paulatinamente la intensidad
				while (true) {
					if (timeElapsed > transitionTime) { break; }
					float toLerp = timeElapsed / transitionTime;
					distancePerCycle = Mathf.Lerp(0, distanceToValue, toLerp);
					if (blurSize > valueToSet) { distancePerCycle *= -1; }
					blurSize = initValue + distancePerCycle;
					yield return new WaitForFixedUpdate();
					timeElapsed += Time.deltaTime;
				}
			}
		}

    }


    public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		int rtW = source.width / 4;
		int rtH = source.height / 4;

		RenderTexture rt = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
		rt.DiscardContents();

		fastBloomMaterial.SetFloat("_Spread", blurSize);
		fastBloomMaterial.SetVector("_ThresholdParams", new Vector2(1.0f, -threshold));
		Graphics.Blit(source, rt, fastBloomMaterial, 0);

		for (int i = 0; i < blurIterations - 1; i++)
		{
			RenderTexture rt2 = RenderTexture.GetTemporary(rt.width / 2, rt.height / 2, 0, source.format);
			rt2.DiscardContents();

			fastBloomMaterial.SetFloat("_Spread", blurSize);
			Graphics.Blit(rt, rt2, fastBloomMaterial, 1);
			RenderTexture.ReleaseTemporary(rt);
			rt = rt2;
		}

		for (int i = 0; i < blurIterations - 1; i++)
		{
			RenderTexture rt2 = RenderTexture.GetTemporary(rt.width * 2, rt.height * 2, 0, source.format);
			rt2.DiscardContents();

			fastBloomMaterial.SetFloat("_Spread", blurSize);
			Graphics.Blit(rt, rt2, fastBloomMaterial, 2);
			RenderTexture.ReleaseTemporary(rt);
			rt = rt2;
		}

		fastBloomMaterial.SetFloat("_BloomIntensity", intensity);
		fastBloomMaterial.SetTexture("_BloomTex", rt);
		Graphics.Blit(source, destination, fastBloomMaterial, 3);

		RenderTexture.ReleaseTemporary(rt);
	}



}

