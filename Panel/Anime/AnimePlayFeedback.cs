using UnityEngine;

namespace YoutubePlayer
{
    public class AnimePlayFeedback : MonoBehaviour
    {
        public YoutubePlayer youtubePlayer;
        public GameObject nowLoadingImage;

        private void Awake()
        {
            Prepare();
        }
        private async void Prepare()
        {
            nowLoadingImage.SetActive(true);
            await youtubePlayer.PrepareVideoAsync();
            nowLoadingImage.SetActive(false);
        }
    }
}
