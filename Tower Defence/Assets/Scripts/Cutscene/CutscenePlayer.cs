using System.Collections.Generic;
using UnityEngine;

// plays a cutscene as a list of slides, going through them one at a time
public class CutscenePlayer : MonoBehaviour
{
    public static CutscenePlayer instance; // static reference to cutscene player
    public bool isPlaying = false; // true while a cutscene is on screen

    private List<CutsceneSlide> slides = new List<CutsceneSlide>(); // slides in the current cutscene
    private int slideIndex = 0; // which slide we're on

    public float slideDuration = 10f; // each slide moves on by itself after this many seconds
    private float slideTimer = 0f; // how long the current slide has been showing
    private float timeScaleBefore = 1f; // game speed before the cutscene so we can put it back after

    void Awake()
    {
        instance = this;
    }

    // count how long the slide has been up and move on when its been there long enough
    void Update()
    {
        if (!isPlaying)
        {
            return;
        }

        // keep the game frozen, in case something like unpausing tries to start it again mid cutscene
        Time.timeScale = 0f;

        // unscaled time so it still counts when the game is paused behind the cutscene
        slideTimer += Time.unscaledDeltaTime;

        if (slideTimer >= slideDuration)
        {
            NextSlide();
        }
    }

    // start a cutscene from the first slide
    public void Play(List<CutsceneSlide> newSlides)
    {
        if (newSlides == null || newSlides.Count == 0)
        {
            return;
        }

        // remember the speed the game was running at, but only if a cutscene isnt already going
        // (otherwise we'd save 0 and the game would stay frozen after)
        if (!isPlaying)
        {
            timeScaleBefore = Time.timeScale;
        }

        // pause the game so nothing moves or spawns behind the cutscene
        Time.timeScale = 0f;

        slides = newSlides;
        slideIndex = 0;
        slideTimer = 0f;
        isPlaying = true;
    }

    // move on to the next slide, ends the cutscene after the last one
    public void NextSlide()
    {
        if (!isPlaying)
        {
            return;
        }

        slideIndex++;
        slideTimer = 0f; // new slide gets the full 10 seconds, whether it was tapped or timed out

        // last slide done, unpause back to whatever speed it was before
        if (slideIndex >= slides.Count)
        {
            isPlaying = false;
            Time.timeScale = timeScaleBefore;
        }
    }

    // the slide that should be on screen right now, null if nothing is playing
    public CutsceneSlide GetCurrentSlide()
    {
        if (!isPlaying || slideIndex >= slides.Count)
        {
            return null;
        }

        return slides[slideIndex];
    }
}
