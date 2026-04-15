using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sword : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject slashAnimPrefab;
    [SerializeField] private Transform slashAnimSpawnPoint;
    [SerializeField] private float swordAttackCD = .5f;
    [SerializeField] private WeaponInfo weaponInfo;

    private Transform weaponCollider;
    private Animator myAnimator;

    private GameObject slashAnim;

    private void Awake() {
        myAnimator = GetComponent<Animator>();
    }

    private void Start() {
        weaponCollider = PlayerController.Instance.GetWeaponCollider();
        slashAnimSpawnPoint = GameObject.Find("SlashSpawnPoint").transform;
    }

    private void Update() {
        MouseFollowWithOffset();
    }

    public WeaponInfo GetWeaponInfo() {
        return weaponInfo;
    }

    public void Attack() {
        myAnimator.SetTrigger("Attack");
        weaponCollider.gameObject.SetActive(true);
        slashAnim = Instantiate(slashAnimPrefab, slashAnimSpawnPoint.position, Quaternion.identity);
        slashAnim.transform.parent = this.transform.parent;
    }

    public void DoneAttackingAnimEvent() {
        weaponCollider.gameObject.SetActive(false);
    }


    public void SwingUpFlipAnimEvent() {
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(-180, 0, 0);

        if (PlayerController.Instance.FacingLeft) { 
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void SwingDownFlipAnimEvent() {
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        if (PlayerController.Instance.FacingLeft)
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

   private void MouseFollowWithOffset()
{
    if (Camera.main == null || PlayerController.Instance == null) return;

    Vector2 mousePos = Mouse.current.position.ReadValue();
    Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(PlayerController.Instance.transform.position);

    float angle = Mathf.Atan2(mousePos.y - playerScreenPoint.y, mousePos.x - playerScreenPoint.x) * Mathf.Rad2Deg;

    if (mousePos.x < playerScreenPoint.x)
    {
        ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, -180, angle);
        weaponCollider.transform.rotation = Quaternion.Euler(0, -180, 0);
    }
    else
    {
        ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, 0, angle);
        weaponCollider.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}

}
