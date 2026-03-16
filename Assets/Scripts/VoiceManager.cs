using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Meta.WitAi.TTS.Data;
using Meta.WitAi.TTS.Utilities;
using UnityEngine;
using UnityEngine.Events;
using Oculus.Voice;
using TMPro;
using UnityEngine.Serialization;

public class VoiceManager : MonoBehaviour
{
    [Header("Wit Config")]
    [SerializeField] 
    private AppVoiceExperience appVoiceExperience;
    [SerializeField] 
    private TextMeshProUGUI transcriptionText;
    
    [Header("TTS Config")]
    [SerializeField] 
    private TTSSpeaker ttsSpeaker;
    [SerializeField] 
    private RoomManager roomManager;
    [SerializeField] 
    private GenerationManager genManager;
    
    [Header("Wake Word Settings")]
    [SerializeField] 
    private float timeout = 4f;

    [Header("Voice Events")]
    [SerializeField] 
    private UnityEvent wakeWordDetected;
    [SerializeField] 
    private UnityEvent<string> transcriptionComplete;
    
    private bool listening;
    private string[] wakeWords =
    {
        "computer",
        "hey computer",
        "hello computer",
        "okay computer",
        "Railey",
        "Hailey"
    };
    
    private Coroutine Timeout;
    private Action ttsCallback;
    
    Queue<(string text, Action onComplete)> paragraphs = new Queue<(string text, Action onComplete)>();
    private Coroutine Narration;
    
    bool immersiveReading = false;
    int currentMilestone = 0;
    List<ReadingMilestone> milestones = new List<ReadingMilestone>();
    
    private TaskCompletionSource<string> generationPrompt;
    
    public class ReadingMilestone
    {
        public string[] triggerPhrases;
        public Action onTrigger;
        public bool triggered;
    }
    
    private void Awake()
    {
        appVoiceExperience.VoiceEvents.OnRequestCompleted.AddListener(ReactivateVoiceExperience);
        appVoiceExperience.VoiceEvents.OnPartialTranscription.AddListener(OnPartialTranscription);
        appVoiceExperience.VoiceEvents.OnFullTranscription.AddListener(OnFullTranscription);
        //appVoiceExperience.VoiceEvents.OnPartialTranscription.AddListener(CheckReadingMilestones);
        
        appVoiceExperience.Activate();
    }

    private void OnDestroy()
    {
        appVoiceExperience.VoiceEvents.OnRequestCompleted.RemoveListener(ReactivateVoiceExperience);
        appVoiceExperience.VoiceEvents.OnPartialTranscription.RemoveListener(OnPartialTranscription);
        appVoiceExperience.VoiceEvents.OnFullTranscription.RemoveListener(OnFullTranscription);
    }

    private void ReactivateVoiceExperience()
    {
        appVoiceExperience.Activate();
    }
    
    private void OnPartialTranscription(string transcription)
    {
        if (listening)
            transcriptionText.text = transcription;
    }

    private void OnFullTranscription(string transcription)
    {
        string text = transcription.ToLower();

        if (immersiveReading)
        {
            CheckReadingMilestones(text);
        }
        else
        {
            // check Transcript for wake word
            foreach (string wakeWord in wakeWords)
            {
                if (text.Contains(wakeWord))
                {
                    // start listening for commands
                    listening = true;
                    // play sound effect
                    wakeWordDetected.Invoke();
                    transcriptionText.text = string.Empty;

                    if (Timeout != null)
                        StopCoroutine(Timeout);

                    Timeout = StartCoroutine(ListeningTimeout());
                    
                    // filter out wake word
                    text = text.Replace(wakeWord, "").Trim();
                }
            }

            // execute voice command
            if (listening && !string.IsNullOrWhiteSpace(text))
            {
                listening = false;
                // play sound effect
                transcriptionComplete.Invoke(text);
                ExecuteCommand(text);
            }
        }
    }

    private IEnumerator ListeningTimeout()
    {
        yield return new WaitForSeconds(timeout);
        listening = false;
    }

    public void ExecuteCommand(string text)
    {
        text = text.ToLower();
        string type = "";
        
        if (text.Contains("audiobook"))
        {
            TtsSpeak(
                "Starting immersive audiobook mode.",
                () => ImmersiveAudiobook("Count")
            );
            genManager.room.wallArtSpawner.AnchorPrefabSpawnerObjects.Values.ElementAt(0).gameObject.SetActive(false);
            genManager.room.defaultPainting.SetActive(false);
            genManager.room.virtualPainting.SetActive(false);
            GameObject.Find("OlivenhainAnim 1").SetActive(false);
            return;
        }
        else if (text.Contains("reading"))
        {
            TtsSpeak(
                "Starting immersive reading mode.",
                () => ImmersiveReading("Count")
            );
            genManager.room.wallArtSpawner.AnchorPrefabSpawnerObjects.Values.ElementAt(0).gameObject.SetActive(false);
            genManager.room.defaultPainting.SetActive(false);
            genManager.room.virtualPainting.SetActive(false);
            GameObject.Find("OlivenhainAnim 1").SetActive(false);
            return;
        }
        
        if (text.Contains("object") && (text.Contains("create") || text.Contains("generate"))) 
            type = "object";
        else if (text.Contains("image") && (text.Contains("create") || text.Contains("generate") || text.Contains("draw")))
            type = "image";

        if (!string.IsNullOrEmpty(type))
        {
            ExtractPrompt(text, type);
            return;
        }
        
        if (text.Contains("animate") || text.Contains("anime") || text.Contains("any mate") || text.Contains("any made")) 
        {
            if (text.Contains("virtual"))
            {
                genManager.AnimatePainting("", 1); 
                TtsSpeak("Animating virtual Image.");
            }
            else if (text.Contains("real"))
            {
                genManager.AnimatePainting("", 0); 
                TtsSpeak("Animating real Image.");
            }
            return;
        }

        if (text.Contains("drawing mode"))
        {
            TtsSpeak("Due to camera feed access limitation this mode is not available over quest link!");
            return;
        }
        
        if (text.Contains("thank you"))
        { 
            TtsSpeak("No Biggie!"); 
            return; 
        }
        
        if (text.Contains("stop")) 
            return;
        
        TtsSpeak("Sorry, I didn’t quite catch that.");
    }
    
    private async void ExtractPrompt(string text, string type)
    {
        string prompt = "";
        
        if (text.Contains("prompt"))
        {
            int index = text.IndexOf("prompt", StringComparison.OrdinalIgnoreCase);
            prompt = text.Substring(index + "prompt".Length).Trim();
        }
        else
        {
            int index = text.IndexOf(type, StringComparison.OrdinalIgnoreCase);
            prompt = text.Substring(index + type.Length).Trim();
        }
        
        if (string.IsNullOrWhiteSpace(prompt))
        {
            TtsSpeak($"Please say the prompt for the {type}.");
            prompt = await WaitForUserPrompt();
        }

        TtsSpeak($"Generating {type} with prompt: {prompt}");

        if (type == "object")
            genManager.TranscriptPromptToObject(prompt);
        else
            genManager.TranscriptPromptToImage(prompt);
    }
    
    private void TtsSpeak(string message)
    {
        appVoiceExperience.Deactivate();
        ttsSpeaker.Speak(message);
        ttsSpeaker.Events.OnPlaybackComplete.AddListener(OnTTSFinished);
    }
    
    private void TtsSpeak(string message, Action onFinished)
    {
        appVoiceExperience.Deactivate();
        ttsCallback = onFinished;
        ttsSpeaker.Events.OnPlaybackComplete.AddListener(OnTtsFinishedWithCallback);
        ttsSpeaker.Speak(message);
    }
    
    private void OnTTSFinished(TTSSpeaker speaker, TTSClipData clip)
    {
        ttsSpeaker.Events.OnPlaybackComplete.RemoveListener(OnTTSFinished);
        appVoiceExperience.Activate();
    }
    
    private void OnTtsFinishedWithCallback(TTSSpeaker speaker, TTSClipData clip)
    {
        ttsSpeaker.Events.OnPlaybackComplete.RemoveListener(OnTtsFinishedWithCallback);
        appVoiceExperience.Activate();
        
        if (ttsCallback != null)
        {
            ttsCallback.Invoke();
            ttsCallback = null;
        }
    }

    private async Task<string> WaitForUserPrompt()
    {
        appVoiceExperience.Activate();

        generationPrompt = new TaskCompletionSource<string>();
        
        void OnPromptTranscription(string transcript)
        {
            if (!string.IsNullOrWhiteSpace(transcript))
            {
                generationPrompt.SetResult(transcript);
                appVoiceExperience.VoiceEvents.OnFullTranscription.RemoveListener(OnPromptTranscription);
            }
        }

        appVoiceExperience.VoiceEvents.OnFullTranscription.AddListener(OnPromptTranscription);
        
        string result = await generationPrompt.Task;

        return result;
    }

    private void ImmersiveAudiobook(string title)
    {
        //if title ...
        SetTtsVoice("WIT$BRITISH BUTLER");
        
        EnqueueParagraph(
            "Then gloom settled heavily upon him.",
            () => roomManager.tableSpawner.enabled = true
        );

        EnqueueParagraph(
            "Dantes was a man of great simplicity of thought, and without education; ",
            () => roomManager.storageSpawner.enabled = true
        );

        EnqueueParagraph(
            "he could not, therefore, in the solitude of his dungeon, traverse in mental vision the history of the ages, ",
            () => roomManager.couchSpawner.enabled = true
        );
        
        EnqueueParagraph(
            "bring to life the nations that had perished,",
            () => roomManager.plantSpawner.enabled = true
        );
        
        EnqueueParagraph(
            "and rebuild the ancient cities so vast and stupendous in the light of the imagination,",
            () => roomManager.screenSpawner.enabled = true
        );
        
        EnqueueParagraph(
            "and that pass before the eye glowing with celestial colors in Martin's Babylonian pictures.",
            () => roomManager.otherSpawner.enabled = true
        );
        
        EnqueueParagraph(
            "He could not do this, he whose past life was so short,",
            () => roomManager.ceilingSpawner.enabled = true
        );
        
        EnqueueParagraph(
            "whose present so melancholy, and his future so doubtful.",
            () => roomManager.lampSpawner.enabled = true
        );
        
        EnqueueParagraph(
            "Nineteen years of light to reflect upon in eternal darkness!",
            null
        );
        
        EnqueueParagraph(
            "No distraction could come to his aid; ",
            () => roomManager.bedSpawner.enabled = true
        );
        
        EnqueueParagraph(
            "his energetic spirit, that would have exalted in thus revisiting the past, was imprisoned like an eagle in a cage.",
            () => roomManager.floorSpawner.enabled = true
        );
        
        EnqueueParagraph(
            "He clung to one idea -- ",
            () => roomManager.wallFaceSpawner.enabled = true
        );
        
        EnqueueParagraph(
            "that of his happiness, destroyed, without apparent cause, by an unheard-of fatality;",
            () => roomManager.windowSpawner.enabled = true
        );
        
        EnqueueParagraph(
            "he considered and reconsidered this idea, ",
            () => roomManager.doorSpawner.enabled = true
        );
        
        EnqueueParagraph(
            "devoured it (so to speak), as the implacable Ugolino devours the skull of Archbishop Roger in the Inferno of Dante.",
            null
        );
    }
    
    private void SetTtsVoice(string voiceId)
    {
        if (!ttsSpeaker.IsSpeaking)
            ttsSpeaker.VoiceID = voiceId;
    }
    
    public void EnqueueParagraph(string text, Action onComplete = null)
    {
        paragraphs.Enqueue((text, onComplete));

        if (Narration == null)
            Narration = StartCoroutine(NarrationLoop());
    }

    private IEnumerator NarrationLoop()
    {
        while (paragraphs.Count > 0)
        {
            var p = paragraphs.Dequeue();
            
            ttsSpeaker.Speak(p.text);
            
            yield return new WaitUntil(() => ttsSpeaker.IsSpeaking);
            yield return new WaitWhile(() => ttsSpeaker.IsSpeaking);
            yield return new WaitForSeconds(0.1f);

            p.onComplete?.Invoke();
        }

        Narration = null;
        SetTtsVoice("WIT$RAILEY");
    }
    
    private void ImmersiveReading(string title)
    {
        immersiveReading = true;
        currentMilestone = 0;
        milestones.Clear();
        
        //if title ...

        milestones.Add(new ReadingMilestone {
            triggerPhrases = new [] { "gloom settled", "gloom" },
            onTrigger = () => roomManager.tableSpawner.enabled = true
        });

        milestones.Add(new ReadingMilestone {
            triggerPhrases = new[] { "simplicity of thought", "simplicity" },
            onTrigger = () => roomManager.storageSpawner.enabled = true
        });
        
        milestones.Add(new ReadingMilestone {
            triggerPhrases = new[] { "history of the ages", "history" },
            onTrigger = () => roomManager.couchSpawner.enabled = true
        });
        
        milestones.Add(new ReadingMilestone {
            triggerPhrases = new[] { "bring to life", "nations" },
            onTrigger = () => roomManager.plantSpawner.enabled = true
        });
        
        milestones.Add(new ReadingMilestone {
            triggerPhrases = new[] { "rebuild the ancient cities", "imagination" },
            onTrigger = () => roomManager.screenSpawner.enabled = true
        });
        
        milestones.Add(new ReadingMilestone {
            triggerPhrases = new[] { "pass before the eye", "pictures" },
            onTrigger = () => roomManager.otherSpawner.enabled = true
        });
        
        milestones.Add(new ReadingMilestone {
            triggerPhrases = new[] { "could not do this", "past" },
            onTrigger = () => roomManager.ceilingSpawner.enabled = true
        });
        
        milestones.Add(new ReadingMilestone {
            triggerPhrases = new[] { "whose present", "future" },
            onTrigger = () => roomManager.lampSpawner.enabled = true
        });
        
        milestones.Add(new ReadingMilestone {
            triggerPhrases = new[] { "distraction", "aid" },
            onTrigger = () => roomManager.bedSpawner.enabled = true
        });
        
        milestones.Add(new ReadingMilestone {
            triggerPhrases = new[] { "spirit", "eagle" },
            onTrigger = () => roomManager.floorSpawner.enabled = true
        });

        milestones.Add(new ReadingMilestone {
            triggerPhrases = new[] { "He clung to one idea", "idea" },
            onTrigger = () => roomManager.wallFaceSpawner.enabled = true
        });
        
        milestones.Add(new ReadingMilestone {
            triggerPhrases = new[] { "happiness", "cause" },
            onTrigger = () => roomManager.windowSpawner.enabled = true
        });

        milestones.Add(new ReadingMilestone {
            triggerPhrases = new[] { "skull", "inferno" },
            onTrigger = () => roomManager.doorSpawner.enabled = true
        });
    }
    
    private void CheckReadingMilestones(string transcript)
    {
        if (immersiveReading && currentMilestone < milestones.Count)
        {
            ReadingMilestone m = milestones[currentMilestone];

            foreach (string triggerPhrase in m.triggerPhrases)
            {
                if (transcript.Contains(triggerPhrase))
                {
                    if (!m.triggered)
                    {
                        m.triggered = true;
                        m.onTrigger.Invoke();
                        currentMilestone++;
                    }
                    break;
                }
            }
        }
    }
    
    private void AnimatePainting(int num)
    {
        // anim painting on command
        //genManager.animatePainting;
    }
    
    private void ReplacePainting(int num)
    {
        // gen object according to voice desc at anchor location
        //genManager.animatePainting;
    }
    
    private void PlaceObject(string label)
    {
        // gen object according to voice desc in free space on floor
        // or on labled object based on size?
    }
    
    private void ScribbleToObject()
    {
        // tts: does not work without cam access over link
    }
    
}