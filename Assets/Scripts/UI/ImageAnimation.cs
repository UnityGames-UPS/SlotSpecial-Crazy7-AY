using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ImageAnimation : MonoBehaviour
{
	public enum ImageState
	{
		NONE,
		PLAYING,
		PAUSED
	}

	public static ImageAnimation Instance;

	public List<Sprite> textureArray;

	public Image rendererDelegate;

	public bool useSharedMaterial = true;

	public bool doLoopAnimation = true;
	[SerializeField] private bool StartOnAwake;

	[HideInInspector]
	public ImageState currentAnimationState;

	private int indexOfTexture;

	private float idealFrameRate = 0.0416666679f;

	private float delayBetweenAnimation;

	public float AnimationSpeed = 5f;

	public float delayBetweenLoop;

	private Dictionary<GameObject, Tween> pulseTweens = new Dictionary<GameObject, Tween>();
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}

	}

	private void OnEnable()
	{
		if (StartOnAwake)
		{
			StartAnimation();
		}
	}

	private void OnDisable()
	{
		//rendererDelegate.sprite = textureArray[0];
		StopAnimation();
	}

	private void AnimationProcess()
	{
		SetTextureOfIndex();
		indexOfTexture++;
		if (indexOfTexture == textureArray.Count)
		{
			indexOfTexture = 0;
			if (doLoopAnimation)
			{
				Invoke("AnimationProcess", delayBetweenAnimation + delayBetweenLoop);
			}
		}
		else
		{
			Invoke("AnimationProcess", delayBetweenAnimation);
		}
	}

	public void StartAnimation()
	{
		indexOfTexture = 0;
		if (currentAnimationState == ImageState.NONE)
		{
			RevertToInitialState();
			delayBetweenAnimation = idealFrameRate * (float)textureArray.Count / AnimationSpeed;
			currentAnimationState = ImageState.PLAYING;
			Invoke("AnimationProcess", delayBetweenAnimation);
		}
	}

	public void PauseAnimation()
	{
		if (currentAnimationState == ImageState.PLAYING)
		{
			CancelInvoke("AnimationProcess");
			currentAnimationState = ImageState.PAUSED;
		}
	}

	public void ResumeAnimation()
	{
		if (currentAnimationState == ImageState.PAUSED && !IsInvoking("AnimationProcess"))
		{
			Invoke("AnimationProcess", delayBetweenAnimation);
			currentAnimationState = ImageState.PLAYING;
		}
	}

	public void StopAnimation()
	{
		if (currentAnimationState != 0)
		{
			rendererDelegate.sprite = textureArray[0];
			CancelInvoke("AnimationProcess");
			currentAnimationState = ImageState.NONE;
		}
	}

	public void RevertToInitialState()
	{
		indexOfTexture = 0;
		SetTextureOfIndex();
	}

	private void SetTextureOfIndex()
	{
		if (useSharedMaterial)
		{
			rendererDelegate.sprite = textureArray[indexOfTexture];
		}
		else
		{
			rendererDelegate.sprite = textureArray[indexOfTexture];
		}
	}
	public void StartPulse(GameObject target, float scaleAmount = 1.2f, float duration = 0.5f)
	{
		// Kill any existing tween on this object first
		if (pulseTweens.ContainsKey(target))
		{
			pulseTweens[target].Kill();
			pulseTweens.Remove(target);
		}

		// Store original scale
		Vector3 originalScale = target.transform.localScale;

		// Create tween
		Tween pulseTween = target.transform
			.DOScale(originalScale * scaleAmount, duration)
			.SetLoops(-1, LoopType.Yoyo) // infinite up & down
			.SetEase(Ease.InOutSine);

		// Store the tween so we can stop it later
		pulseTweens[target] = pulseTween;
	}
	public void StopPulse(GameObject target)
	{
		if (pulseTweens.ContainsKey(target))
		{
			pulseTweens[target].Kill();
			pulseTweens.Remove(target);
			// Optionally reset to original scale
			target.transform.localScale = Vector3.one;
		}
	}

}
