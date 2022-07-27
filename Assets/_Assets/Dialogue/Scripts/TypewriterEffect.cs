using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private float writingSpeed = 50;

    [SerializeField] private Color textColor;
    [SerializeField] private AudioSource voiceAudioSource;
    public Color TextColor => textColor;

    public bool isRunning { get; private set; }

    private readonly List<Punctuation> punctuations = new List<Punctuation>()
    {
        new Punctuation(new HashSet<char>(){'.','!','?'}, 0.6f),
        new Punctuation(new HashSet<char>(){','}, 0.3f),
        new Punctuation(new HashSet<char>(){' '}, 0.01f)
    };

    private Coroutine typingCoroutine;

    public void Run(string textToType, TMP_Text textLabel, AudioClip voiceClip)
    {
        typingCoroutine = StartCoroutine(TypeText(textToType, textLabel, voiceClip));
    }

    public void Stop()
    {
        StopCoroutine(typingCoroutine);
        isRunning = false;
    }

    private IEnumerator TypeText(string textToType, TMP_Text textLabel, AudioClip voiceClip)
    {
        TMP_TextInfo textInfo = textLabel.textInfo;
        Color32[] newVertexColors;
        Color32 c0 = textColor;
        textLabel.color = Color.clear;

        isRunning = true;

        textLabel.text = textToType;

        float t = 0;
        int charIndex = 0;

        if(voiceClip != null)
        {
            voiceAudioSource.clip = voiceClip;
            voiceAudioSource.time = 0;
            voiceAudioSource.Play();
        }

        while (charIndex < textToType.Length)
        {
            int lastCharIndex = charIndex;

            t += Time.deltaTime * writingSpeed;
            charIndex = Mathf.FloorToInt(t);

            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

            for(int i = lastCharIndex; i<charIndex; i++)
            {
                bool isLast = i >= textToType.Length - 1;

                //Update text color to type text
                {
                    // Get the index of the material used by the current character.
                    int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

                    // Get the vertex colors of the mesh used by this text element (character or sprite).
                    newVertexColors = textInfo.meshInfo[materialIndex].colors32;

                    // Get the index of the first vertex used by this text element.
                    int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                    // Only change the vertex color if the text element is visible.
                    if (textInfo.characterInfo[i].isVisible)
                    {
                        newVertexColors[vertexIndex + 0] = c0;
                        newVertexColors[vertexIndex + 1] = c0;
                        newVertexColors[vertexIndex + 2] = c0;
                        newVertexColors[vertexIndex + 3] = c0;

                        // New function which pushes (all) updated vertex data to the appropriate meshes when using either the Mesh Renderer or CanvasRenderer.
                        textLabel.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                        // This last process could be done to only update the vertex data that has changed as opposed to all of the vertex data but it would require extra steps and knowing what type of renderer is used.
                        // These extra steps would be a performance optimization but it is unlikely that such optimization will be necessary.
                    }
                }

                if (IsPunctuation(textToType[i], out float waitTime) && !isLast && !IsPunctuation(textToType[i + 1], out _))
                {
                    if (voiceClip != null)
                    {
                        voiceAudioSource.time = 0;
                        voiceAudioSource.Play();
                    }
                    yield return new WaitForSeconds(waitTime);
                }
            }

            yield return null;
        }

        isRunning = false;
    }

    private bool IsPunctuation(char character, out float waitTime)
    {
        foreach(Punctuation punctuationCategory in punctuations)
        {
            if (punctuationCategory.punctuations.Contains(character))
            {
                waitTime = punctuationCategory.waitTime;
                return true;
            }
        }

        waitTime = default;
        return false;
    }

    private readonly struct Punctuation
    {
        public readonly HashSet<char> punctuations;
        public readonly float waitTime;

        public Punctuation(HashSet<char> hashset, float _waitTime)
        {
            waitTime = _waitTime;
            punctuations = hashset;
        }
    }
}
