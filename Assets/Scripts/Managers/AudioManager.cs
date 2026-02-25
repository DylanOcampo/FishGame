using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    public static AudioManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindAnyObjectByType<AudioManager>();
            }
            return _instance;
        }
    }

    public AudioSource Music, Sounds;
    public AudioClip[] sounds = new AudioClip[5];
    public AudioClip[] music = new AudioClip[3];

    List<string> subs;

    private GameObject SubsFinalPosition, SubsText, timeobject;
    Tweener SubsAni;
    private int SubsPosition;
    private AudioClip audioEcplise;
    Vector3 Pos;



    void Update()
    {

    }

    public void setObjects(GameObject _SubsFinalPosition, GameObject _SubsText, GameObject _timeobject)
    {

        SubsFinalPosition = _SubsFinalPosition;
        SubsText = _SubsText;
        timeobject = _timeobject;

    }
    public void PlayClipMusic(int i)
    {
        Music.PlayOneShot(music[i]);
    }

    public void PlayClip(int i)
    {
        Sounds.PlayOneShot(sounds[i]);
    }

    public void Subs(List<string> _subs, AudioClip _audio)
    {
        audioEcplise = _audio;
        Sounds.PlayOneShot(audioEcplise);
        Pos = timeobject.transform.position;
        SubsAni = SubsText.transform.DOMove(SubsFinalPosition.transform.position, .5f).Pause().SetAutoKill(false);
        subs = _subs;

        SubsPosition = 0;
        SubsLogic();

    }



    private void SubsLogic()
    {
        if (subs[SubsPosition] == "")
        {
            SubsPosition++;
            SubsLogic();
        }
        else
        {
            timeobject.transform.position = Pos;
            SubsText.GetComponentInChildren<TextMeshProUGUI>().text = subs[SubsPosition];
            SubsAni.Play();
            SubsAni.OnComplete(() =>
            {
                timeobject.transform.DOMove(SubsFinalPosition.transform.position, audioEcplise.length / subs.Count).OnComplete(() =>
                {
                    SubsAni.Rewind();
                    SubsPosition++;
                    SubsLogic();

                }); ;


            });
        }

    }





    public void OnvalChangeMusic(float val)
    {
        Music.volume = val;
    }
    public void OnvalChangeSFX(float val)
    {
        Sounds.volume = val;
    }

}