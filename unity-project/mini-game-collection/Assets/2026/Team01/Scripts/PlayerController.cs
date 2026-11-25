using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGameCollection.Games2026.Team01
{
    public class PlayerController : MiniGameBehaviour
    {
        [field: SerializeField] public PlayerID PlayerID { get; private set; }
        [field: SerializeField] public float PlayerSpeed { get; private set;  }
        [field: SerializeField] public float Accel { get; private set;  }
        [field: SerializeField] public float Decel { get; private set;  }

        [field: SerializeField] public float SpriteSize { get; private set; }

        public List<GameObject> currentTask;

        private float AxisX;
        private float AxisY;

        private Animator animator;
        private Rigidbody2D rb2d;
        private Transform hand;
        
        void Awake()
        {
            hand = transform.Find("Hand");
            animator = GetComponentInChildren<Animator>();
            rb2d = GetComponent<Rigidbody2D>();
            Debug.Log(animator.ToString() + PlayerID);
        }
        void Update()
        {
            // Get Input
            AxisX = ArcadeInput.Players[(int)PlayerID].AxisX;
            AxisY = ArcadeInput.Players[(int)PlayerID].AxisY;
            //Debug.Log($"Player: {PlayerID}\nX: {axisX}\nY: {axisY}");


            // Always scale the player back to (1, 1, 1)
            Vector3 targetMove = new Vector3(SpriteSize, SpriteSize, SpriteSize);
            if (transform.localScale != targetMove) { transform.localScale = Vector3.Lerp(transform.localScale, targetMove, 10f * Time.deltaTime); }
        }
        void FixedUpdate()
        {
            // Change Movement
            Vector2 movement = new Vector2(AxisX, AxisY);
            movement.Normalize();

            Vector2 velocity = rb2d.velocity;
            float targetX = movement.x * PlayerSpeed;
            float rateX;
            float targetY = movement.y * PlayerSpeed;
            float rateY;
            // Check if the player is moving at ALL
            if (Mathf.Abs(AxisX) > 0.01f || Mathf.Abs(AxisY) > 0.01f)
            {
                rateX = Accel;
                rateY = Accel;
                if (Mathf.Sign(targetX) != Mathf.Sign(velocity.x) && Mathf.Abs(velocity.x) > 0.01f)
                {
                    rateX = Decel;
                }
                if (Mathf.Sign(targetY) != Mathf.Sign(velocity.y) && Mathf.Abs(velocity.y) > 0.01f)
                {
                    rateY = Decel;
                }
            }
            else
            {
                rateX = Decel;
                rateY = Decel;
            }
            velocity.x = Mathf.MoveTowards(velocity.x, targetX, rateX * Time.fixedDeltaTime);
            velocity.y = Mathf.MoveTowards(velocity.y, targetY, rateY * Time.fixedDeltaTime);
            rb2d.velocity = velocity;

            // Check for if player has a tool and wants to drop it
            if (ArcadeInput.Players[(int)PlayerID].Action2.Pressed && hand.GetComponentInChildren<ToolScript>())
            {
                ToolScript toolHeld = GetComponentInChildren<ToolScript>();
                toolHeld.Drop();
            }
            // Check for if player has a tool and wants to swing it
            if (ArcadeInput.Players[(int)PlayerID].Action1.Pressed && hand.GetComponentInChildren<ToolScript>())
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    Swing();
                }
            }
        }
        protected override void OnEnable()
        {
            
        }
        public void Swing()
        {
            Debug.Log($"Swing here for {PlayerID}");

            // Animation

            animator.SetTrigger("Swing");
        }
    }
}
