using BrunoMikoski.AnimationSequencer;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SelectableInteractable : AnimationStepBase
{
    public override string DisplayName => "Selectable Interactable";
    [SerializeField] private Selectable _button;
    [SerializeField] private bool _interactable;
    public override void AddTweenToSequence(Sequence animationSequence)
    {
        animationSequence.AppendCallback(() => _button.interactable = _interactable);
    }

    public override void ResetToInitialState()
    {
        _button.interactable = !_interactable;
    }
}
