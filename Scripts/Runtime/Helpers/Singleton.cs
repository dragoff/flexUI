using UnityEngine;

namespace FlexUI
{
	public interface ISingleton { }
	public class Singleton<T> : MonoBehaviour, ISingleton where T : MonoBehaviour
	{
		/// <summary>
		/// Please use public static methods to expose your singleton.
		/// </summary>
		protected static T Instance;
		protected void Awake()
		{
			if (Instance == null)
			{
				Instance = this as T;
			}
			else if (Instance != this)
			{
				Destroy(gameObject);
				return;
			}

			if (Application.isPlaying)
				DontDestroyOnLoad(this);

			OnAwake();
		}

		protected virtual void OnAwake() { }
	}
}
