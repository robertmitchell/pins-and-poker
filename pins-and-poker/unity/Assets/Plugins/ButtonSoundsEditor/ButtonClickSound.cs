using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
namespace Assets.Plugins.ButtonSoundsEditor
{
    public class ButtonClickSound : MonoBehaviour, IPointerClickHandler
    {
        public AudioSource AudioSource;
        public AudioClip ClickSound;

        private void OnEnable()
        {
          /*  if (SceneManager.GetActiveScene().buildIndex==1|| SceneManager.GetActiveScene().buildIndex == 2|| SceneManager.GetActiveScene().buildIndex == 3 || SceneManager.GetActiveScene().buildIndex == 4)
            {
                AudioSource=GameManager.Instance.buttonAudioSource;

            }*/

        }
        private void Awake()
        {
            AudioSource = FindObjectOfType<AudioSource>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            PlayClickSound();
        }

        private void PlayClickSound()
        {
            try
            {
                AudioSource?.PlayOneShot(ClickSound);              
            }
            catch (System.Exception ex)
            {
                Debug.LogError("ex: "+ ex.Message);
                throw;
            }
        }
    }

}
