using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Personaje : MonoBehaviour
{
    //stats de Leif
    public float HP;
    public float Speed;
    public float Damage;

    #region Variables Movimiento

    //variables para el movimiento
    [SerializeField] private Rigidbody rb;
    public float HorizontalInput;

    //variables para el salto
    [SerializeField] private float jumpForce;
    [SerializeField] private float JumpStartTime;
    [SerializeField] private float jumpTime;
    [SerializeField] public bool isJumping;
    [SerializeField] private Transform groundDetector;
    [SerializeField] private Vector3 detectorDimensions;
    [SerializeField] private bool isGrounded;
    [SerializeField] private LayerMask isFloor;

    //Variables para la caida
    [SerializeField] private float globalGravity;
    [SerializeField] private float gravityScale;
    [SerializeField] private float afterJumpScale;
    [SerializeField] private float maxFallSpeed;
    [SerializeField] private float timer = 1f;

    //Variables para agacharse
    [SerializeField] private float normalSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private BoxCollider leifCollider;

    #endregion

    //Variables UI
    public Image healthBar;
    public float barHP;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        leifCollider = GetComponentInChildren<BoxCollider>();
        healthBar = GameObject.Find("Health").GetComponent<Image>();
    }

    void Start()
    {
        normalSpeed = Speed;
    }

    void Update()
    {
        HorizontalInput= Input.GetAxisRaw("Horizontal");
        Jump();
        Crouch();
        healthBar.material.SetFloat("_Health", (barHP - HP) / 100);
    }

    private void FixedUpdate()
    {
        Movement(HorizontalInput);
        Gravity();
        Grounded();
    }

    #region Movement

    private void Movement(float dir)//Toma la variable de direccion y la usa para moverse con velocity del rigidbody.
    {
        var xVel = dir * Speed * 100 * Time.fixedDeltaTime;
        Vector2 targetVelocity = new Vector2(xVel, rb.velocity.y);
        rb.velocity = targetVelocity;
    } 

    private void Jump()//Salto que se hace mas alto contra mas se sostiene apretado el boton.
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            jumpTime = JumpStartTime;

            rb.velocity = Vector2.up * jumpForce;
        }

        if (Input.GetButton("Jump") && isJumping)
        {
            if (jumpTime > 0)
            {
                rb.velocity = Vector2.up * jumpForce;
                jumpTime -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }
    }

    private void Gravity()//Simula gravedad para matener al jugador en el piso y para tener saltos mas realistas.
    {
        if(!isGrounded && !isJumping)
        {
            timer += Time.fixedDeltaTime;
            Vector3 gravity = Mathf.Clamp(globalGravity * afterJumpScale, 9.8f, maxFallSpeed) * Vector3.down;
            rb.AddForce(gravity * timer, ForceMode.Acceleration);
            if (timer >= 1.3f)
            {
                rb.AddForce(gravity * timer, ForceMode.Acceleration);
            }
        }
        else if (isGrounded && !isJumping)
        {
            timer = 1f;
            Vector3 gravity = Mathf.Clamp(globalGravity * gravityScale, 0, maxFallSpeed) * Vector3.down;
            rb.AddForce(gravity, ForceMode.Acceleration);
        }

    }

    private void Crouch()
    {
        if (isGrounded && !isJumping)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                Speed = crouchSpeed;
                leifCollider.center = new Vector3(0, -0.6f, 0);
                leifCollider.size = new Vector3(1, 0.8f, 1);
            }
            else
            {
                Speed = normalSpeed;
                leifCollider.center = new Vector3(0, 0, 0);
                leifCollider.size = new Vector3(1, 2, 1);
            }
        }
    }

    private void Grounded()//Detecta si el player esta parado en el piso o no.
    {
        isGrounded = Physics.CheckBox(groundDetector.position, detectorDimensions, Quaternion.identity, isFloor);
    }

    #endregion

    public void TakeDamage(float damage)//Llama a este script cada vez que recibe da�o de algo.
    {
        HP -= damage;
    }
}
