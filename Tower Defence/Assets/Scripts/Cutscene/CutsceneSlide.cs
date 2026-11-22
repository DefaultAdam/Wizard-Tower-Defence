// one screen of a cutscene, who is talking and what they say
public class CutsceneSlide
{
    public string speaker; // name above the text, empty means its the narrator
    public string text; // what gets said on this slide

    public CutsceneSlide(string slideSpeaker, string slideText)
    {
        speaker = slideSpeaker;
        text = slideText;
    }
}
