using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerUnit;

    public Pokemen Pokemen { get; set; }

    Image image;
    Vector3 orginalPos;
    Color orginalColor;
    private void Awake()
    {
        image = GetComponent<Image>();
        orginalPos = image.transform.localPosition;
        orginalColor = image.color;
    }
    public void Setup(Pokemen pokemen)
    {
        Pokemen = pokemen;
        if (isPlayerUnit )
        {
            image.sprite = Pokemen.Base.BackSprite;
        }
        else
        {
            image.sprite = Pokemen.Base.FrontSprite;
        }
        image.color = orginalColor;
        PlayEnterAnimation();
    }

    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
            image.transform.localPosition = new Vector3(-500f, orginalPos.y);
        else
            image.transform.localPosition = new Vector3(500f, orginalPos.y);

        image.transform.DOLocalMoveX(orginalPos.x, 1f);
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
            sequence.Append(image.transform.DOLocalMoveX(orginalPos.x + 50f, 0.25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(orginalPos.x - 50f, 0.25f));

        sequence.Append(image.transform.DOLocalMoveX(orginalPos.x, 0.25f));
    }

    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, .1f));
        sequence.Append(image.DOColor(orginalColor, .1f));
    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(orginalPos.y - 150f, .5f));
        sequence.Join(image.DOFade(0, .5f));
    }
}
