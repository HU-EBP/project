using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PuzzleCheck : MonoBehaviour
{
    // Vars for the interaction/scene
    public int PuzzleInt;
    [SerializeField] private GameObject PlayerCharacter;
    [SerializeField] private GameObject TargetObject;
    [SerializeField] private int PuzzleSceneInt;
    [SerializeField] private float activationDistance = 2f;
    [SerializeField] private bool isActive = false;
    private int PuzzleCompletedValue;
    private bool PuzzleCompleted = false;
    private bool PuzzleDone = false;

    // Vars for the movable object
    [SerializeField] private Transform objectToMove;
    [SerializeField] private float moveDuration = 1.0f;
    [SerializeField] private Vector3 moveTo;
    private bool isMoving = false;
    private float startTime;
    private Vector3 startPosition;

    void Start()
    {
        // Get value to check if a puzzle was done (if the player came from the PuzzleScene), remove pref
        if (PlayerPrefs.HasKey("DidPuzzle"))
        {
            PuzzleDone = PlayerPrefs.GetInt("DidPuzzle") == 1;
            PlayerPrefs.DeleteKey("DidPuzzle");
        }

        // Check if puzzle was done (if the player came from the PuzzleScene), teleport player if it was
        if (PuzzleDone) { PlayerCharacter.transform.position = transform.position; }

        // Get value to check if puzzle completed
        if (PlayerPrefs.HasKey("Puzzle" + PuzzleInt + "Completed")) { PuzzleCompletedValue = PlayerPrefs.GetInt("Puzzle" + PuzzleInt + "Completed"); }
        if (PlayerPrefs.HasKey("Puzzle" + (PuzzleInt + 1) + "Completed"))
        {
            if (PlayerPrefs.GetInt("Puzzle" + (PuzzleInt + 1) + "Completed") == 1) { PuzzleCompleted = false; }
            else { PuzzleCompleted = PuzzleCompletedValue == 1; }
        }
        else { PuzzleCompleted = PuzzleCompletedValue == 1; }

        // Check if the puzzle was completed, move object if it was
        if (PuzzleCompleted)
        {
            isMoving = true;
            startTime = Time.time;
            startPosition = objectToMove.localPosition;
        }
    }

    private void Update()
    {
        // Check for distance between interactiepunt and player, show E-key if player is close enough
        if (!isActive && Vector3.Distance(TargetObject.transform.position, PlayerCharacter.transform.position) <= activationDistance)
        {
            TargetObject.gameObject.SetActive(true);
            isActive = true;
        }

        // Hide E-key if player moves away again
        else if (isActive && Vector3.Distance(TargetObject.transform.position, PlayerCharacter.transform.position) > activationDistance)
        {
            TargetObject.gameObject.SetActive(false);
            isActive = false;
        }

        // If the player presses E while the E-key is showing, go to puzzle
        if (isActive && Input.GetKeyDown(KeyCode.E))
        {
            PlayerPrefs.SetInt("PrevScene", (SceneManager.GetActiveScene().buildIndex));
            PlayerPrefs.SetInt("PuzzleNum", PuzzleInt);
            SceneManager.LoadScene(PuzzleSceneInt);
        }

        // Move the specified platform
        if (isMoving)
        {
            float t = (Time.time - startTime) / moveDuration;
            objectToMove.localPosition = Vector3.Lerp(startPosition, moveTo, t);
            if (t >= 1.0f) { isMoving = false; }
        }
    }
}