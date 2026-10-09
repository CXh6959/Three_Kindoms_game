using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class 移动端界面适配 : MonoBehaviour
{
	private static readonly Vector2 参考分辨率 = new Vector2(960f, 540f);

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void 创建适配器()
	{
		if (FindObjectOfType<移动端界面适配>() != null)
		{
			return;
		}
		GameObject host = new GameObject("移动端界面适配");
		DontDestroyOnLoad(host);
		host.AddComponent<移动端界面适配>();
	}

	private void Awake()
	{
		应用横屏设置();
		SceneManager.sceneLoaded += 场景加载完成;
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= 场景加载完成;
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (hasFocus)
		{
			应用横屏设置();
		}
	}

	private void 场景加载完成(Scene scene, LoadSceneMode mode)
	{
		StopAllCoroutines();
		StartCoroutine(延迟适配场景(scene));
	}

	private IEnumerator 延迟适配场景(Scene scene)
	{
		yield return null;
		适配场景画布(scene);
	}

	private static void 应用横屏设置()
	{
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		Screen.orientation = ScreenOrientation.AutoRotation;
	}

	private void 适配场景画布(Scene scene)
	{
		Canvas[] canvases = FindObjectsOfType<Canvas>(true);
		for (int i = 0; i < canvases.Length; i++)
		{
			Canvas canvas = canvases[i];
			if (canvas == null || canvas.gameObject.scene != scene || canvas.renderMode == RenderMode.WorldSpace || !canvas.isRootCanvas)
			{
				continue;
			}
			CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
			if (scaler != null)
			{
				scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
				scaler.referenceResolution = 参考分辨率;
				scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
				scaler.matchWidthOrHeight = 0.5f;
			}
		}
	}
}
