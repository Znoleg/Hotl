using System.Collections;
using UnityEngine;

public class FunctionalityControlOnStun : MonoBehaviour
{
    private StunKillAnimation _stunKillAnim;
    private UIFunctionalityController _controller;

    private void Start()
    {
        _controller = UIFunctionalityController.Instance;
        _stunKillAnim = PlayerCharacter.Instance.PlayerGraphics.StunKillAnimation;
        _stunKillAnim.OnStunKillStart += StunKillAnimation_OnStunKillStart;
        _stunKillAnim.OnStunKillEnd += StunKillAnim_OnStunKillEnd;
    }

    private void StunKillAnim_OnStunKillEnd()
    {
        _controller.SetActive(true);
    }

    private void StunKillAnimation_OnStunKillStart()
    {
        _controller.SetActive(false);
    }

    private void OnDestroy()
    {
        _stunKillAnim.OnStunKillStart -= StunKillAnimation_OnStunKillStart;
        _stunKillAnim.OnStunKillEnd -= StunKillAnim_OnStunKillEnd;
    }
}
