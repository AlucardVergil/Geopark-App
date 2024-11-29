using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class VideoPlayerUI : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Reference to the VideoPlayer
    public Slider progressBar; // Slider to show video progress
    public TMP_Text currentTimeText; // Text to display current time
    public TMP_Text totalTimeText; // Text to display total video length
    public Button playPauseButton; // Button to toggle play/pause
    public Sprite playIcon; // Icon for play
    public Sprite pauseIcon; // Icon for pause

    private bool isDragging; // To track if the slider is being dragged

    private void Start()
    {
        // Prepare the video
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;

        // Set up play/pause button
        playPauseButton.onClick.AddListener(TogglePlayPause);

        // Add listener to the slider
        progressBar.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void Update()
    {
        // Update the progress bar and time text if not dragging
        if (!isDragging && videoPlayer.isPlaying)
        {
            UpdateProgressBar();
            UpdateTimeText();
        }
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        // Set the total time text
        totalTimeText.text = FormatTime((float)vp.length);

        // Enable the progress bar interaction
        progressBar.maxValue = (float)vp.length;
        progressBar.value = 0;
    }

    private void UpdateProgressBar()
    {
        if (videoPlayer.frameCount > 0)
        {
            // Update the slider's value based on the current time
            progressBar.value = (float)videoPlayer.time;
        }
    }

    private void UpdateTimeText()
    {
        // Update the current time text
        currentTimeText.text = FormatTime((float)videoPlayer.time);
    }

    public void TogglePlayPause()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            playPauseButton.image.sprite = playIcon;
        }
        else
        {
            videoPlayer.Play();
            playPauseButton.image.sprite = pauseIcon;
        }
    }

    private void OnSliderValueChanged(float value)
    {
        if (isDragging)
        {
            // Update the current time text while dragging
            currentTimeText.text = FormatTime(value);
        }
    }

    public void OnSliderDragStart()
    {
        isDragging = true;
    }

    public void OnSliderDragEnd()
    {
        isDragging = false;

        // Seek the video to the new time
        videoPlayer.time = progressBar.value;
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60F);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60F);
        return $"{minutes:00}:{seconds:00}";
    }
}
