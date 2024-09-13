using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rod : MonoBehaviour
{
    [Header("Rods parameters")]
    [Tooltip("Number of slots on the rod")]
    [SerializeField] public int slots;
    [Tooltip("Distance in between each disk on the rod, default: 0.266")]
    [SerializeField] public float slotsDistance;
    [Tooltip("Id of the rod")]
    [SerializeField] public int id;
    [Tooltip("If the rod is selected or not")]
    [SerializeField] public bool selected = false;

    private GameController gc;


    [Header("Rods materials")]
    [Tooltip("Normal state material")]
    [SerializeField] public Material baseMaterial;
    [Tooltip("Active state material")]
    [SerializeField] public Material activeMaterial;
    [Tooltip("Error state material")]
    [SerializeField] public Material impossibleMaterial;

    public Rod()
    {
        disks = new List<Disk>(); // We initialize the disks on the rod
    }

    private List<Disk> disks;

    void Start()
    {
        gc = GameObject.Find("GameController").GetComponent<GameController>();
        if (gc == null) throw new MissingComponentException("Game Controller not found");
    }

    private void OnMouseEnter()
    {
        gc.HoverRod(this); // If the user is hovering the rod
        
        gameObject.GetComponent<MeshRenderer>().material = activeMaterial; // Change material
    }

    private void OnMouseExit()
    {
        gameObject.GetComponent<MeshRenderer>().material = baseMaterial; // Change material
    }

    private void OnMouseDown()
    {
        // Change the selection status
        select();
    }

    /// Summary: Gets coordinates of the disk on top of the rod
	/// Returns: The top disk position
    public float GetRodTopPosition()
    {
        return (disks.Count - 1) * slotsDistance;
    }

    /// Summary: Add the disk
	/// Returns: True if disk was added, otherwise false
    public bool AddDisk(Disk disk)
    {
        if (disks.Count >= slots) return false;
        disks.Add(disk);
        return true;
    }

    /// Summary: Remove the top disk
	/// Returns: True if disk was removed, otherwise false
    public bool RemoveTopDisk()
    {
        if (disks.Count > 0)
        {
            
            return disks.Remove(disks.FindLast(x => x));
        }

        return false;
    }

    /// Summary: Gets the top disk
    /// Returns: The top disk
    public Disk GetTopDisk()
    {
        return disks.FindLast(x => x);
    }

    /// Summary: Check if rod is empty
    /// Returns: True if rod is empty, otherwise false
    public bool IsRodEmpty()
    {
        if (disks.Count > 0) return false;

        return true;
    }

    /// Summary: Get the size of the disk on top of the rod
    /// Returns: The top disk size
    public int GetTopDiskSize()
    {
        return GetTopDisk().Size;
    }

    /// Summary: Get the X position of the rod
    /// Returns: The X position of the rod
    public float GetPositionX()
    {
        return this.transform.position.x;
    }

    /// Update wether a rod is selected or not
    public void select()
    {
        selected = !selected;
    }

    // Sets the rod red if the move is impossible
    public void impossibleMove()
    {
        gameObject.GetComponent<MeshRenderer>().material = impossibleMaterial;
        
    }

    // Make the disk size accessible to other classes
    public int getRodDisksSize()
    {
        return disks.Count;
    }


}
