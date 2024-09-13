using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Disk : MonoBehaviour
{

    [Header("Disk parameters")]
    [Tooltip("Size of the disk")]
    [SerializeField] public int Size;

    [Header("Disk materials")]
    [Tooltip("Default material")]
    [SerializeField] public Material baseMaterial;
    [Tooltip("Active material")]
    [SerializeField] public Material activeMaterial;
    public bool IsUp { get { return isPicked; } }
    public int CurrentRod { get { return currentRod; } }

    private GameController gc;
    
    private float threshold = 0.2f; // Limit for the disk to stop when picked

    private bool isPicked = false;

    private bool isMovingVertically = false;

    private int currentRod = 1;

    // Target for disk movements when picked and placed
    private float targetX;
    private float targetY;
    
    private float translationY = 2f; // Y Value increase when disk is picked

    void Start()
    {
        targetY = this.transform.position.y;

        gc = GameObject.Find("GameController").GetComponent<GameController>();
        if (gc == null) throw new MissingComponentException("Game Controller not found!");
    }

    private void OnMouseDown()
    { 
        gc.SelectDisk(this); //Select a disk on click

    }

    private void OnMouseEnter()
    {
        gameObject.GetComponent<MeshRenderer>().material = activeMaterial; // Change material
    }

    private void OnMouseExit()
    {
        gameObject.GetComponent<MeshRenderer>().material = baseMaterial; // Change material
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
    }

    public void PickUp()
	{
		isPicked = true;
		isMovingVertically = true;
		targetY = translationY;
    }

	public void Place(float toY)
	{
		isPicked = false;
		isMovingVertically = true;
		targetY = toY;
	}

	public void MoveLeft()
	{
		if (currentRod <= 0 || !isPicked || isMovingVertically) return;
        currentRod--;
		targetX = gc.GetRodPositionX(currentRod);
	}

	public void MoveRight()
	{
		if (currentRod >= 2 || !isPicked || isMovingVertically) return;
        currentRod++;
		targetX = gc.GetRodPositionX(currentRod);		
	}

    private void UpdatePosition()
    {
        if (targetY != this.transform.position.y) // The disk needs to move up or down
        {
            if (Mathf.Abs(targetY - this.transform.position.y) < threshold)
            {
                this.transform.Translate(0, targetY - this.transform.position.y, 0);
                isMovingVertically = false;
                return;
            }
            this.transform.Translate(GetDirectionY(targetY) * threshold);
        }

        if (targetX != this.transform.position.x) // The disk needs to move left or right
        {
            if (Mathf.Abs(targetX - this.transform.position.x) < threshold)
            {
                this.transform.Translate(targetX - this.transform.position.x, 0, 0);
                return;
            }
            this.transform.Translate(GetDirectionX(targetX) * threshold);
        }
    }

    private Vector3 GetDirectionX(float targetX)
    {
        if (targetX > this.transform.position.x) return Vector3.right;
        if (targetX < this.transform.position.x) return Vector3.left;
        return Vector3.zero;
    }

    private Vector3 GetDirectionY(float targetY)
    {
        if (targetY > this.transform.position.y) return Vector3.up;
        if (targetY < this.transform.position.y) return Vector3.down;
        return Vector3.zero;
    }
}
