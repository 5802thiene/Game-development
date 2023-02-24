using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Include the namespace required to use Unity UI and Input System
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // Create public variables for player speed, and for the Text UI game objects
    public float speed;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;

    private float movementX;
    private float movementY;

    private Rigidbody rb;
    private int count;

    public TextMeshProUGUI positionText;
    public TextMeshProUGUI velocityText;
    private Vector3 previousPosition;

    public TextMeshProUGUI closestPickup_Display;
    private LineRenderer lineRenderer;

    // At the start of the game..
    void Start()
    {
        // Assign the Rigidbody component to our private rb variable
        rb = GetComponent<Rigidbody>();

        // Set the count to zero
        count = 0;

        SetCountText();

        // Set the text property of the Win Text UI to an empty string, making the 'You Win' (game over message) blank
        winTextObject.SetActive(false);

        closestPickup_Display.text = "Closest Pickup: ";
        // Check if LineRenderer component is attached to the game object
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            // Add a LineRenderer component to the game object if it is not already attached
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
    }

    void FixedUpdate()
    {
        // Create a Vector3 variable, and assign X and Z to feature the horizontal and vertical float variables above
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);

        rb.AddForce(movement * speed);

        positionText.text = "Position: " + transform.position.ToString();

        float currentSpeed = rb.velocity.magnitude;
        velocityText.text = "Speed: " + currentSpeed.ToString("F0") + " m/s";

        // Find the closest pickup

        float closestDistance = Mathf.Infinity;
        int closestIndex = -1;
        GameObject[] pickups = GameObject.FindGameObjectsWithTag("PickUp");
        for (int i = 0; i < pickups.Length; i++)
        {
            if (pickups[i].activeInHierarchy)
            {
                float distance = Vector3.Distance(
                    rb.transform.position,
                    pickups[i].transform.position
                );
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }
        }

        // Reset colors of all pickups
        for (int i = 0; i < pickups.Length; i++)
        {
            pickups[i].GetComponent<Renderer>().material.color = Color.white;
        }

        // Display closest pickup
        if (closestIndex > -1)
        {
            // Change color of closest pickup to blue
            pickups[closestIndex].GetComponent<Renderer>().material.color = Color.blue;

            // Display distance in text label
            closestPickup_Display.text = "Closest Pickup: " + closestDistance.ToString("F2");
        }

        DrawPathToNearestPickup();
    }

// 	// Draw a line to the closest pickup
//     if (closestIndex > -1)
//     {
//         GameObject closestPickup = pickups[closestIndex];
//         closestPickup.GetComponent<Renderer>().material.color = Color.blue;

//         closestPickup_Display.text = "Closest Pickup: " + closestDistance.ToString("F2");

//         // Draw a line from the player to the closest pickup
//         if (lineRenderer != null)
//         {
//             lineRenderer.enabled = true;
//             lineRenderer.SetPosition(0, transform.position);
//             lineRenderer.SetPosition(1, closestPickup.transform.position);
//             lineRenderer.startWidth = 0.1f;
//  lineRenderer.endWidth = 0.1f;
//         }
//     }
//     else
//     {
//         closestPickup_Display.text = "Closest Pickup: N/A";
//         lineRenderer.enabled = false;
//     }

    void DrawPathToNearestPickup()
    {
        // Find the closest pickup
        float closestDistance = Mathf.Infinity;
        GameObject closestPickup = null;
        GameObject[] pickups = GameObject.FindGameObjectsWithTag("PickUp");
        for (int i = 0; i < pickups.Length; i++)
        {
            if (pickups[i].activeInHierarchy)
            {
                float distance = Vector3.Distance(
                    rb.transform.position,
                    pickups[i].transform.position
                );
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPickup = pickups[i];
                }
            }
        }

        // Draw a line from player to closest pickup
        if (closestPickup != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, closestPickup.transform.position);
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
        }
        else
        {
            lineRenderer.SetPosition(0, Vector3.zero);
            lineRenderer.SetPosition(1, Vector3.zero);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // ..and if the GameObject you intersect has the tag 'Pick Up' assigned to it..
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);

            // Add one to the score variable 'count'
            count = count + 1;

            // Run the 'SetCountText()' function (see below)
            SetCountText();
        }
    }

    void OnMove(InputValue value)
    {
        Vector2 v = value.Get<Vector2>();

        movementX = v.x;
        movementY = v.y;
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();

        if (count >= 12)
        {
            // Set the text value of your 'winText'
            winTextObject.SetActive(true);
        }
    }
}
