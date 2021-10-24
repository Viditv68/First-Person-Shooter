using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public float moveSpeed;
    public float gravityModifier;
    public float jumpPower;
    public float runSpeed = 12f;
    public CharacterController characterController;
    public Transform camTransform;

    public float mouseSensitivity;
    public bool invertX;
    public bool invertY;

    private Vector3 moveInput;
   
    private bool canJump;
    private bool canDoubleJump; 
    public Transform groundCheckPoint;
    public LayerMask whatIsGrounded;

    public Animator anim;

    //public GameObject bullet;
    public Transform firePoint;

    public Gun activeGun;

    public List<Gun> allGuns = new List<Gun>();
    public List<Gun> UnlockableGuns = new List<Gun>();
    public int currentGun;

    public Transform adsPoint;
    public Transform gunHolder;
    private Vector3 gunStartPos;
    public float adsSpeed;
    private void Awake()
    {
        instance = this;

    }

    private void Start()
    {
        currentGun--;
        SwitchGun();
        gunStartPos = gunHolder.localPosition;
    }
    void Update()
    {
        if (UIController.instance.pauseScreen.activeInHierarchy)
            return;
        //moveInput.x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        //moveInput.z = Input.GetAxisRaw("Vertical") * moveSpeed * Time.deltaTime;

        //store y velocity
        float yStore = moveInput.y;
        Vector3 verticalMove = transform.forward * Input.GetAxis("Vertical");
        Vector3 horizontalMove = transform.right * Input.GetAxis("Horizontal");

        moveInput = horizontalMove + verticalMove;
        moveInput.Normalize();
        if (Input.GetKey(KeyCode.LeftShift))
            moveInput = moveInput * runSpeed;
        else
            moveInput = moveInput * moveSpeed;

        moveInput.y = yStore;
        moveInput.y += Physics.gravity.y * gravityModifier * Time.deltaTime;

        if (characterController.isGrounded)
            moveInput.y = Physics.gravity.y * gravityModifier * Time.deltaTime;

        //jumping
        Jump();

        characterController.Move(moveInput * Time.deltaTime);

        //canera control
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y") * mouseSensitivity);
        if (invertX)
            mouseInput.x = -mouseInput.x;
        if (invertY)
            mouseInput.y = -mouseInput.y;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, mouseInput.x, 0f));

        camTransform.rotation = Quaternion.Euler(camTransform.transform.rotation.eulerAngles + new Vector3(-mouseInput.y, 0f, 0f));


        //Handle SHooting
        FireBullet();

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchGun();
        }

        if(Input.GetMouseButtonDown(1))
        {
            CameraController.instance.ZoomIn(activeGun.zoomAmount);
        }
        if(Input.GetMouseButton(1))
        {
            gunHolder.position = Vector3.MoveTowards(gunHolder.position, adsPoint.position, adsSpeed * Time.deltaTime);
        }
        else
        {
            gunHolder.localPosition = Vector3.MoveTowards(gunHolder.localPosition, gunStartPos, adsSpeed * Time.deltaTime);
        }

        if(Input.GetMouseButtonUp(1))
        {
            CameraController.instance.ZoomOut();
        }


        anim.SetFloat("moveSpeed", moveInput.magnitude);
        anim.SetBool("onGround", canJump);

    }

    private void FireBullet()
    {
        //single shot
        if (Input.GetMouseButtonDown(0) && activeGun.fireCounter <= 0)
        {
            RaycastHit hit;
            if(Physics.Raycast(camTransform.position, camTransform.forward, out hit, 50f))
            {
                if(Vector3.Distance(camTransform.position, hit.point) > 2f)
                    firePoint.LookAt(hit.point);
            }
            else
            {
                firePoint.LookAt(camTransform.position + (camTransform.forward * 30f));
            }

            //Instantiate(bullet, firePoint.position, firePoint.rotation);
            FireShot();
        }

        //shot repeat
        if(Input.GetMouseButton(0) && activeGun.canAutoFire)
        {
            if(activeGun.fireCounter <= 0)
                FireShot();
        }
            
    }

    public void FireShot()
    {
        if (activeGun.currentAmmo > 0)
        {
            activeGun.currentAmmo--;
            Instantiate(activeGun.bullet, firePoint.position, firePoint.rotation);
            activeGun.fireCounter = activeGun.fireRate;

            UIController.instance.ammoText.text = "Ammo: " + activeGun.currentAmmo;
        }
    }
    public void SwitchGun()
    {
        activeGun.gameObject.SetActive(false);
        currentGun++;
        if (currentGun >= allGuns.Count)
            currentGun = 0;
        activeGun = allGuns[currentGun];
        activeGun.gameObject.SetActive(true);
        UIController.instance.ammoText.text = "Ammo: " + activeGun.currentAmmo;
        firePoint.position = activeGun.firePoint.position;
    }

    public void AddGun(string gunToAdd)
    {
        bool gunUnlocked = false;

        if(UnlockableGuns.Count > 0)
        {
            for(int i = 0; i < UnlockableGuns.Count; i++)
            {
                if(UnlockableGuns[i].gunName == gunToAdd)
                {
                    gunUnlocked = true;
                    allGuns.Add(UnlockableGuns[i]);
                    UnlockableGuns.RemoveAt(i);
                    i = UnlockableGuns.Count;
                }
            }
        }

        if(gunUnlocked)
        {
            currentGun = allGuns.Count - 2;
            SwitchGun();
        }
    }


    private void Jump()
    {
        canJump = Physics.OverlapSphere(groundCheckPoint.position, .1f, whatIsGrounded).Length > 0;
        if (canJump)
            canDoubleJump = false;

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            moveInput.y = jumpPower;
            canDoubleJump = true;
        }
        else if (canDoubleJump && Input.GetKeyDown(KeyCode.Space))
        {
            moveInput.y = jumpPower;
            canDoubleJump = false;
        }
    }


}
