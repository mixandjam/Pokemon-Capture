using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;

public class CaptureManager : MonoBehaviour
{
    [Header("Public References")]
    public Transform pokeball;
    public Transform pokemon;
    [Space]
    [Header("Throw Settings")]
    public float throwArc;
    public float throwDuration;
    [Space]
    [Header("Hit Settings")]
    public Vector3 hitOffset;
    public Transform jumpPosition;
    public float jumpPower = .5f;
    public float jumpDuration;
    [Space]
    [Header("Open Settings")]
    public float openAngle;
    public float openDuration;
    [Space]
    [Header("Open Settings")]
    public float fallDuration = .6f;
    [Space]
    [Header("Cameras Settings")]
    public GameObject secondCamera;
    public float finalZoomDuration = .5f;

    [Space]
    [Header("Particles")]
    public ParticleSystemForceField forceField;
    public ParticleSystem throwParticle;
    public ParticleSystem firstLines;
    public ParticleSystem firstCircle;
    public ParticleSystem firstFlash;
    public ParticleSystem firstDust;
    public ParticleSystem beam;

    [Space]
    public ParticleSystem capture1;
    public ParticleSystem capture2;
    public ParticleSystem capture3;

    [Space]
    public ParticleSystem yellowBlink;
    public ParticleSystem blueBlink;
    public ParticleSystem finalCircle;
    public ParticleSystem stars;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ThrowPokeball();
        }

        if (Input.GetKey(KeyCode.S))
        {
            Time.timeScale = .5f;
        }
        else
        {
            Time.timeScale = 1;
        }

        Debug();
    }

    void ThrowPokeball()
    {

        Sequence throwSequence = DOTween.Sequence();

        //Throw the pokeball
        throwSequence.Append(pokeball.DOJump(pokemon.position + hitOffset, throwArc, 1, throwDuration));
        throwSequence.Join(pokeball.DORotate(new Vector3(300, 0, 0), throwDuration, RotateMode.FastBeyond360));

        //Pokeball Jump
        throwSequence.Append(pokeball.DOJump(jumpPosition.position, jumpPower, 1, jumpDuration));
        throwSequence.Join(pokeball.DOLookAt(pokemon.position, jumpDuration));

        throwSequence.AppendCallback(()=>throwParticle.Stop());
        throwSequence.AppendCallback(() => firstCircle.Play());
        throwSequence.AppendCallback(() => firstLines.Play());
        throwSequence.AppendCallback(() => firstFlash.Play());
        throwSequence.AppendCallback(() => firstDust.Play());

        //Pokemon Disappear
        throwSequence.AppendCallback(() => PokemonDisappear());

        //Pokeball Open
        throwSequence.Append(pokeball.GetChild(0).GetChild(0).DOLocalRotate(new Vector3(-openAngle, 0, 0), openDuration).SetEase(Ease.OutBack));
        throwSequence.Join(pokeball.GetChild(0).GetChild(1).DOLocalRotate(new Vector3(openAngle, 0, 0), openDuration).SetEase(Ease.OutBack));

        throwSequence.AppendCallback(() => forceField.gameObject.SetActive(true));
        throwSequence.Join(firstDust.transform.DORotate(new Vector3(0, 0, 100), .5f, RotateMode.FastBeyond360));

        throwSequence.Join(beam.transform.DOMove(jumpPosition.position, .2f));

        //Camera Change
        throwSequence.AppendCallback(() => ChangeCamera());

        //Pokeball Close
        throwSequence.Append(pokeball.GetChild(0).GetChild(0).DOLocalRotate(Vector3.zero, openDuration/3));
        throwSequence.Join(pokeball.GetChild(0).GetChild(1).DOLocalRotate(Vector3.zero, openDuration/3));

        throwSequence.AppendCallback(() => capture1.Play());
        throwSequence.AppendCallback(() => capture2.Play());
        throwSequence.AppendCallback(() => capture2.Play());
        throwSequence.AppendCallback(() => secondCamera.transform.DOShakePosition(.2f, .1f, 15, 90, false, true));

        throwSequence.Join(pokeball.DORotate(Vector3.zero, openDuration/3).SetEase(Ease.OutBack));

        //Interval
        throwSequence.AppendInterval(.3f);

        //Pokeball Fall
        throwSequence.Append(pokeball.DOMoveY(.18f, fallDuration).SetEase(Ease.OutBounce));
        throwSequence.Join(pokeball.DOPunchRotation(new Vector3(-40, 0, 0), fallDuration, 5, 10));
    }

    private void PokemonDisappear()
    {
        pokemon.DOScale(0, .3f);
    }

    private void ChangeCamera()
    {
        secondCamera.SetActive(true);

        Transform cam = secondCamera.transform;

        Sequence cameraSequence = DOTween.Sequence();
        cameraSequence.Append(cam.DOMoveY(.3f, 1.5f)).SetDelay(.5f);

        cameraSequence.AppendInterval(.5f);
        cameraSequence.Append(cam.DOMoveZ(.3f, finalZoomDuration).SetEase(Ease.InExpo));

        //Particle
        cameraSequence.AppendCallback(() => yellowBlink.Play());
        cameraSequence.Join(pokeball.GetChild(0).DOShakeRotation(.5f, 30, 8, 70, true));

        cameraSequence.AppendInterval(.8f);
        cameraSequence.Append(cam.DOMoveZ(.0f, finalZoomDuration).SetEase(Ease.InExpo));


        //Particle
        cameraSequence.AppendCallback(() => yellowBlink.Play());
        cameraSequence.Join(pokeball.GetChild(0).DOShakeRotation(.5f, 20, 8, 70, true));

        cameraSequence.AppendInterval(.8f);
        cameraSequence.Append(cam.DOMoveZ(-.2f, finalZoomDuration).SetEase(Ease.InExpo));

        //Particle
        cameraSequence.AppendCallback(() => yellowBlink.Play());
        cameraSequence.Join(pokeball.GetChild(0).DOShakeRotation(.5f, 10, 8, 70, true));

        cameraSequence.AppendInterval(.8f);

        //Particle
        cameraSequence.AppendCallback(() => blueBlink.Play());
        cameraSequence.AppendCallback(() => finalCircle.Play());
        cameraSequence.AppendCallback(() => stars.Play());
        cameraSequence.AppendCallback(() => secondCamera.transform.DOShakePosition(.2f, .1f, 7, 90, false, true));

        cameraSequence.Append(pokeball.GetChild(0).DOPunchRotation(new Vector3(-10, 0, 0), .5f, 8, 1));



    }

    void Debug()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(pokemon.position + hitOffset, .2f);
    }
}
