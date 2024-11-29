using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using System.Collections;

public class VideoPlayerUI : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Reference to the VideoPlayer
    public AudioSource audioSource; // Reference to the AudioSource for video audio
    public Slider progressBar; // Slider to show video progress
    public TMP_Text currentTimeText; // Text to display current time
    public TMP_Text totalTimeText; // Text to display total video length
    public Button playPauseButton; // Button to toggle play/pause
    public Sprite playIcon; // Icon for play
    public Sprite pauseIcon; // Icon for pause

    private bool isDragging; // To track if the slider is being dragged

    public GameObject videoControls; // Parent GameObject containing all controls
    private float controlsTimer; // Timer to track inactivity
    private bool controlsVisible = true; // Whether the controls are visible
    public float autoHideDelay = 4.0f; // Time in seconds before hiding controls

    private float lastProgressBarValue = 0;


    private void Start()
    {
        // Configure the VideoPlayer to use the AudioSource
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, audioSource);

        // Prepare the video
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;

        // Set up play/pause button
        playPauseButton.onClick.AddListener(TogglePlayPause);

        // Add listener to the slider
        progressBar.onValueChanged.AddListener(OnSliderValueChanged);

        // Detect clicks on the video player area
        videoPlayer.targetCameraAlpha = 1.0f; // Ensure video is clickable
    }

    private void Update()
    {
        // Update the progress bar and time text if not dragging
        if (!isDragging && videoPlayer.isPlaying)
        {
            UpdateProgressBar();
            UpdateTimeText();
        }


        // Auto-hide controls logic
        if (videoPlayer.isPlaying && controlsVisible && !isDragging)
        {
            controlsTimer += Time.deltaTime;

            if (controlsTimer >= autoHideDelay)
            {
                HideControls();
            }
        }
    }


    // Method to show the controls
    private void ShowControls()
    {
        foreach (Transform child in videoControls.transform)
            child.gameObject.SetActive(true);

        controlsVisible = true;
    }

    // Method to hide the controls
    private void HideControls()
    {
        foreach (Transform child in videoControls.transform)
            child.gameObject.SetActive(false);

        controlsVisible = false;
    }

    // Detect clicks on the video player to toggle controls
    public void OnVideoClicked()
    {
        if (!controlsVisible)
        {
            ResetControlsTimer();
        }
        else
        {
            controlsTimer = 0;
        }
    }

    // Method to reset the controls timer
    private void ResetControlsTimer()
    {
        controlsTimer = 0;
        if (!controlsVisible)
        {
            ShowControls();
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
            audioSource.Pause(); // Pause audio
            playPauseButton.image.sprite = playIcon;            
        }
        else
        {
            //lastProgressBarValue = progressBar.value;
            //videoPlayer.Play();
            //videoPlayer.time = lastProgressBarValue;

            //audioSource.Play(); // Play audio
            //playPauseButton.image.sprite = pauseIcon;
            StartCoroutine(StartVideo());
        }
    }


    IEnumerator StartVideo()
    {
        lastProgressBarValue = progressBar.value;

        yield return new WaitForSeconds(0.05f);
        videoPlayer.Play();
        yield return new WaitForSeconds(0.05f);
        videoPlayer.time = lastProgressBarValue;

        audioSource.Play(); // Play audio
        playPauseButton.image.sprite = pauseIcon;
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

        // Synchronize audio with video after seeking
        audioSource.time = (float)videoPlayer.time;
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60F);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60F);
        return $"{minutes:00}:{seconds:00}";
    }
}
