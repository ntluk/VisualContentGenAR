using System;
using System.Collections;
using System.Collections.Generic;
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
        "okay computer"
    };
    
    private Coroutine Timeout;
    private Action ttsCallback;
    
    Queue<(string text, Action onComplete)> paragraphs = new Queue<(string text, Action onComplete)>();
    private Coroutine Narration;
    
    bool immersiveReading = false;
    int currentMilestone = 0;
    List<ReadingMilestone> milestones = new List<ReadingMilestone>();
    
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
            // Check Transcript for wake word
            foreach (string wake in wakeWords)
            {
                if (text.Contains(wake))
                {
                    // Start listening for commands
                    listening = true;
                    // play sound effect
                    wakeWordDetected.Invoke();
                    transcriptionText.text = string.Empty;

                    if (Timeout != null)
                        StopCoroutine(Timeout);

                    Timeout = StartCoroutine(ListeningTimeout());
                    
                    // filter out wake word
                    text = text.Replace(wake, "").Trim();
                }
            }

            // Execute voice command
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
        //if (text.Contains("audiobook mode"))
        if (text.Contains("audiobook"))
        {
            TtsSpeak(
                "Starting immersive audiobook mode.",
                () => ImmersiveAudiobook("Count")
            );
        }
        //else if (text.Contains("reading mode"))
        else if (text.Contains("reading"))
        {
            TtsSpeak(
                "Starting immersive reading mode.",
                () => ImmersiveReading("Count")
            );
        }
        else if (text.Contains("drawing mode"))
        {
            TtsSpeak("Due to camera feed access limitaion this mode is not available over quest link!");
        }
        else if (text.Contains("stop"))
        {
            // stop
            // just change mode instead
        }
        else
        {
            TtsSpeak("Sorry, I didn’t quite catch that.");
        }
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
            "bring to life the nations that had perished, and rebuild the ancient cities so vast and stupendous in the light of the imagination, and that pass before the eye glowing with celestial colors in Martin's Babylonian pictures.",
            () => roomManager.floorSpawner.enabled = true
        );
        
        EnqueueParagraph(
            "He could not do this, he whose past life was so short, whose present so melancholy, and his future so doubtful.",
            () => roomManager.ceilingSpawner.enabled = true
        );
        
        EnqueueParagraph(
            "Nineteen years of light to reflect upon in eternal darkness!",
            () => roomManager.wallFaceSpawner.enabled = true
        );
        
        EnqueueParagraph(
            "No distraction could come to his aid; his energetic spirit, that would have exalted in thus revisiting the past, was imprisoned like an eagle in a cage.",
            null
        );
        
        EnqueueParagraph(
            "He clung to one idea -- that of his happiness, destroyed, without apparent cause, by an unheard-of fatality; he considered and reconsidered this idea, devoured it (so to speak), as the implacable Ugolino devours the skull of Archbishop Roger in the Inferno of Dante",
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

            p.onComplete.Invoke();
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
            triggerPhrases = new[] { "light of the imagination", "imagination" },
            onTrigger = () => roomManager.floorSpawner.enabled = true
        });
        
        milestones.Add(new ReadingMilestone {
            triggerPhrases = new[] { "present so melancholy", "present" },
            onTrigger = () => roomManager.floorSpawner.enabled = true
        });

        milestones.Add(new ReadingMilestone {
            triggerPhrases = new[] { "nineteen years", "eternal darkness" },
            onTrigger = () => roomManager.wallFaceSpawner.enabled = true
        });
    }
    
    private void CheckReadingMilestones(string transcript)
    {
        if (immersiveReading && currentMilestone < milestones.Count)
        {
            ReadingMilestone m = milestones[currentMilestone];

            foreach (string phrase in m.triggerPhrases)
            {
                if (transcript.Contains(phrase))
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