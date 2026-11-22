using System.Collections.Generic;

// all the games cutscenes kept in one place so the story is easy to change later
public static class CutsceneLibrary
{
    // cutscene that plays at the start of each round
    // just placeholder panels for now, swap the text for the real story later
    public static List<CutsceneSlide> GetRoundCutscene(int round)
    {
        List<CutsceneSlide> slides = new List<CutsceneSlide>();

        slides.Add(new CutsceneSlide("Round " + round, ""));
        slides.Add(new CutsceneSlide("", "(cutscene text goes here)"));

        return slides;
    }
}
