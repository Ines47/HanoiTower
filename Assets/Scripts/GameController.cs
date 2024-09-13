using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private Disk selectedDisk;
    private Rod selectedRod;

    [Header("Rods and Disks")]
    [Tooltip("Rods used for the game")]
    [SerializeField] public Rod[] Rods;
    [Tooltip("Disks used for the game")]
    [SerializeField] public Disk[] Disks;

    [Header("Texts")]
    [Tooltip("Text for move counter")]
    public TextMeshProUGUI counterText;
    [Tooltip("Text for timer")]
    public TextMeshProUGUI timerText;
    [Tooltip("Text for victory message")]
    public GameObject victoryText;
    [Tooltip("Text for instructions message")]
    public GameObject instructionsText;

    [Header("Audiosources")]
    [Tooltip("Audio for disk selection")]
    public AudioSource clickSound;
    [Tooltip("Audio for disk placement on rod")]
    public AudioSource clickSound2;
    [Tooltip("Audio for when a disk is not placeable on a rod resulting in an error")]
    public AudioSource impossibleSound;
    [Tooltip("Audio for victory when the game is finished")]
    public AudioSource victorySound;

    private int counter; // Moves counter
    private float elapsedTime = 0; // Time elapsed since the start

    // 0 --> Waiting for disk to be clicked
    // 1 --> Disk picked up waiting for rod to be clicked
    // 2 --> Victory, the game is finished everything else is blocked
    private int state = 0; 

    void Start()
    {
        foreach (var disk in Disks)
        {
            Rods[1].AddDisk(disk); // Add disks on the middle pole for initialization
        }
    }


    void Update()
    {
        if(state != 2) // State 2 is for victory
        {
            elapsedTime += Time.deltaTime; // Increment the elapsed time by the time since the last frame

            setTimer(); // Update the timer every frame

            // If no disk has been selected yet
            if (selectedDisk == null)
            {
                return;
            }
            else if (selectedDisk != null && state == 0 && Rods[selectedDisk.CurrentRod].GetTopDisk() == selectedDisk)
            {
                // If a disk has been selected and the state was 0, last condition makes it impossible to select a disk that is not on top, instructions disappears
                instructionsText.SetActive(false);

                pickDisk(); // Pick a disk

                // Transition to state 1
                state = 1;
            }

            // Once a disk is selected check if a rod is selected
            if (selectedRod == null)
            {
                return;
            }
            else if (selectedRod != null && state == 1)
            {
                // If a rod is selected and the state is 1 we continue
                if (selectedRod.id > selectedDisk.CurrentRod)
                {
                    // If the rod selected is on the right of the current rod where our selected disk is
                    moveRight();
                }
                else if (selectedRod.id < selectedDisk.CurrentRod)
                {
                    // If the rod selected is on the left of the current rod where our selected disk is
                    moveLeft();
                }
                else if (selectedRod.id == selectedDisk.CurrentRod && selectedRod.selected)
                {
                    // When the user click the desired rod again it places the disk on the rod
                    placeDisk();
                }
            }
            // We check if the game is finished
            checkVictory();
        }
    }

    private void pickDisk()
    {
        if (selectedDisk.IsUp) return; // If the disk is already picked we can't pick it again
        
        if (Rods[selectedDisk.CurrentRod].RemoveTopDisk())
        {
            selectedDisk.PickUp();
        }
    }

    private void placeDisk()
    {
        if (!selectedDisk.IsUp) return; // If the disk is not picked up we can't place it

        if (!Rods[selectedDisk.CurrentRod].IsRodEmpty())
        {
            // If the rod is not empty we need to check the top disk size
            if (selectedDisk.Size > Rods[selectedDisk.CurrentRod].GetTopDiskSize())
            {
                // Deselect previously selected rod
                selectedRod = null;

                // Play sound
                playErrorSound();

                // Make the rod red to indicate error
                Rods[selectedDisk.CurrentRod].impossibleMove();

                return; // We can't drop a bigger disk on a smaller one
            }

        }

        if (Rods[selectedDisk.CurrentRod].AddDisk(selectedDisk))
        {
            // If we place the disk on the rod
            selectedDisk.Place(Rods[selectedDisk.CurrentRod].GetRodTopPosition());

            // Move counter updated
            counter++;
            setCounter(counter);

            // Placing sound
            clickSound2.Play();

            // We return to state 0
            state = 0;

            // Reset the selected rod and disk
            selectedRod.select();
            selectedDisk = null;
            selectedRod = null;
        }
    }

    private void moveRight()
    {
        selectedDisk.MoveRight();
    }

    private void moveLeft()
    {
        selectedDisk.MoveLeft();
    }

    public void SelectDisk(Disk newSelection)
    {
        if (selectedDisk == null)
        {
            clickSound.Play();
            selectedDisk = newSelection;
            return;
        }

        if (!selectedDisk.IsUp)
        {
            selectedDisk = newSelection;
        }
    }

    public void HoverRod(Rod newSelection)
    {
        selectedRod = newSelection;
        return;
    }

    public float GetRodPositionX(int rod)
    {
        return Rods[rod].GetPositionX();
    }
    
    // Set Move counter
    private void setCounter(int count)
    {
        counterText.text = "Moves: " + count;
    }

    private void setTimer()
    {
        DisplayTime(elapsedTime);
    }

    // Display Time in a proper format
    private void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Check if the game is finished
    private bool checkVictory()
    {
        if (Rods[0].getRodDisksSize() == 5 || Rods[2].getRodDisksSize() == 5)
        {
            // If one of the two rods where the disks were not placed at the beginning contains all 5 disks
            // Activate ending message
            victoryText.SetActive(true);
            victorySound.Play();

            // Put the game in it's final state 
            state = 2;
            return true;
        }

        return false;
    }

    public void playErrorSound()
    {
        impossibleSound.Play();
    }
}
