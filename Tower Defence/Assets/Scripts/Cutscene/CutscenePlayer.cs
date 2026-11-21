using System.Collections.Generic;
using UnityEngine;

// plays a cutscene as a list of slides, going through them one at a time
public class CutscenePlayer : MonoBehaviour
{
    public static CutscenePlayer instance; // static reference to cutscene player
    public bool isPlaying = false; // true while a cutscene is on screen

    private List<CutsceneSlide> slides = new List<CutsceneSlide>(); // slides in the current cutscene
    private int slideIndex = 0; // which slide we're on

    void Awake()
    {
        instance = this;
    }

    // start a cutscene from the first slide
    public void Play(List<CutsceneSlide> newSlides)
    {
        if (newSlides == null || newSlides.Count == 0)
        {
            return;
        }

        slides = newSlides;
        slideIndex = 0;
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

        if (slideIndex >= slides.Count)
        {
            isPlaying = false;
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
