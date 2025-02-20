using UnityEngine;

public class CameraSlide : MonoBehaviour
{
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        ShopUI.Instance.CustomShopOpen += OpenSlide;
    }

    private void OpenSlide(bool shopOpen)
    {
        _animator.SetBool("ShopOpened", shopOpen);
    }
}
