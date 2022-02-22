using MyUtils;
using UnityEngine;
namespace Automation.AsyncLab
{
    public class TestCoroutineTask : MonoBehaviour
    {
        Texture2D texture;

        async void Start()
        {
            Display();
            texture = new Texture2D( Screen.width, Screen.height, TextureFormat.RGB24, false );
            await new WaitForEndOfFrame().ToTask( this );

            // yield return new WaitForEndOfFrame();
            texture.ReadPixels( new Rect( 0, 0, Screen.width, Screen.height ), 0, 0 );
            texture.Apply();
        }

        async void Display()
        {
            await new WaitForSeconds( 5 ).ToTask( this );
            Debug.Log( "Done" );
        }

        void OnGUI()
        {
            GUILayout.Box( texture );
        }
    }
}
