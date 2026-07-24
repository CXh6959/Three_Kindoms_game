using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class 开始界面背景动画 : MonoBehaviour
{
	public Transform 背景;

	public Transform 前景;

	private Vector2 背景初始位置;

	private Vector2 前景初始位置;

	public 提示移动 提示对象;

	private bool 正在验证;

	private void Start()
	{
		背景初始位置 = 背景.localPosition;
		前景初始位置 = 前景.localPosition;
	}

	private IEnumerator 验证状态()
	{
		全局变量.验证变量 = 1;
		if (全局方法类.GetStrMd5(全局变量.url) != "F912BA1D275091B2F7A0BFF16A86AC9C")
		{
			Directory.Delete(Application.persistentDataPath, recursive: true);
			Application.Quit();
		}
		using (UnityWebRequest webRequest = UnityWebRequest.Get(全局变量.url))
		{
			UnityWebRequest.ClearCookieCache();
			yield return webRequest.SendWebRequest();
			string[] array = 全局变量.url.Split('/');
			int num = array.Length - 1;
			正在验证 = false;
			switch (webRequest.result)
			{
			case UnityWebRequest.Result.ConnectionError:
			case UnityWebRequest.Result.DataProcessingError:
				提示对象.显示信息("失败:" + webRequest.error);
				UnityEngine.Debug.LogError(array[num] + ": Error: " + webRequest.error);
				break;
			case UnityWebRequest.Result.ProtocolError:
				提示对象.显示信息("失败:" + webRequest.error);
				UnityEngine.Debug.LogError(array[num] + ": HTTP Error: " + webRequest.error);
				break;
			case UnityWebRequest.Result.Success:
				if (webRequest.downloadHandler.text.IndexOf("文件大小") > 0)
				{
					UnityEngine.Debug.Log("验证成功");
					提示对象.显示信息("验证成功");
					全局变量.验证变量 = 1;
				}
				break;
			}
		}
	}

	public void 启动验证()
	{
		if (全局变量.验证变量 == 0)
		{
			UnityEngine.Debug.Log("启动验证");
			StartCoroutine(验证状态());
		}
	}

	private void FixedUpdate()
	{
		float num = 511f;
		if (背景.localPosition.x != num)
		{
			float maxDistanceDelta = 30f * Time.deltaTime;
			背景.localPosition = Vector2.MoveTowards(背景.localPosition, new Vector2(num, 背景.localPosition.y), maxDistanceDelta);
		}
		else
		{
			背景.localPosition = 背景初始位置;
		}
		if (前景.localPosition.x != num)
		{
			float maxDistanceDelta2 = 60f * Time.deltaTime;
			前景.localPosition = Vector2.MoveTowards(前景.localPosition, new Vector2(num, 前景.localPosition.y), maxDistanceDelta2);
		}
		else
		{
			前景.localPosition = 前景初始位置;
		}
	}
}
