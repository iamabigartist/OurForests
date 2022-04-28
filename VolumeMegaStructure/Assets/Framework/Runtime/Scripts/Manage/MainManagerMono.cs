using UnityEngine;
namespace VolumeMegaStructure.Manage
{
	public class MainManagerMono : MonoBehaviour
	{
		void OnDestroy()
		{
			MainManager.TerminateManager();
		}
	}
}