using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum ActionHud {AddMinion, RemoveMinion}
public enum MinionType {BlueMinion, RedMinion}

public class GameManager : MonoBehaviour , IObservable
{
    private List<IObserver> _observersIa = new();

    [Header("PrefabsReferenceInstance")]
    [SerializeField] private IABlueMinion blueMinionPrefab;
    [SerializeField] private IARedMinion redMinionPrefab;
    
    [Header("MinionsToSpawn")]
    [SerializeField] private List<FSMBase> redMinions;
    [SerializeField] private List<FSMBase> blueMinions;
    
    [Header("LeadersReference")]
    [SerializeField] private IABlueLeader blueLeader;
    [SerializeField] private IARedLeader redLeader;

    [Header("PositionsToSpawnReference")]
    [SerializeField] private List<Transform> redPositionMinionsSpawn;
    [SerializeField] private List<Transform> bluePositionMinionsSpawn;
    [SerializeField] private GameObject redTransformsParent;
    [SerializeField] private GameObject blueTransformsParent;

    private List<Transform> _positionsAroundBlueLeader;
    private List<Transform> _positionsAroundRedLeader;

    [Header("HUD")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button restartGameButton;
    [SerializeField] private Text winMessage;
    [SerializeField] private GameObject redIaManageMinionsBttns;
    [SerializeField] private GameObject blueIaManageMinionsBttns;
    
    #region Getters & Setters

    public List<FSMBase> RedMinions 
    {
        get => redMinions;
        private set => redMinions = value; //set in inspector
    }

    public List<FSMBase> BlueMinions
    {
        get => blueMinions;
        private set => blueMinions = value; //set in inspector
    }

    public IABlueLeader BlueLeader
    {
        get => blueLeader;
        private set => blueLeader = value; //set in inspector
    }

    public IARedLeader RedLeader
    {
        get => redLeader;
        private set => redLeader = value;
    }

    #endregion
    
    private void Start()
    {
        bluePositionMinionsSpawn = blueTransformsParent.GetComponentsInChildren<Transform>().ToList();
        bluePositionMinionsSpawn.Remove(bluePositionMinionsSpawn.First());

        redPositionMinionsSpawn = redTransformsParent.GetComponentsInChildren<Transform>().ToList();
        redPositionMinionsSpawn.Remove(redPositionMinionsSpawn.First());

        _positionsAroundBlueLeader = BlueLeader.GetComponentsInChildren<Transform>().ToList();
        _positionsAroundBlueLeader.Remove(_positionsAroundBlueLeader.First());

        _positionsAroundRedLeader = RedLeader.GetComponentsInChildren<Transform>().ToList();
        _positionsAroundRedLeader.Remove(_positionsAroundRedLeader.First());
    }
    
    private void AddMinion(MinionType typeMinion)
    {
        switch (typeMinion)
        {
            case MinionType.RedMinion  : ModifyAmountMinions(redPositionMinionsSpawn, redMinionPrefab, ActionHud.AddMinion, RedMinions, _positionsAroundRedLeader);    break;
            case MinionType.BlueMinion : ModifyAmountMinions(bluePositionMinionsSpawn, blueMinionPrefab, ActionHud.AddMinion,BlueMinions, _positionsAroundBlueLeader); break;
        }
    }

    private void SubstractMinion(MinionType typeMinion)
    {
        switch (typeMinion)
        {
            case MinionType.RedMinion  : ModifyAmountMinions(redPositionMinionsSpawn, redMinionPrefab, ActionHud.RemoveMinion, RedMinions, _positionsAroundRedLeader);    break;
            case MinionType.BlueMinion : ModifyAmountMinions(bluePositionMinionsSpawn, blueMinionPrefab, ActionHud.RemoveMinion,BlueMinions, _positionsAroundBlueLeader); break;
        }
    }

    private void ModifyAmountMinions(List<Transform> spawnPosMinions ,FSMBase minionType, ActionHud action,List<FSMBase> minionsType, List<Transform> positionsAroundLeader)
    {
        if (action == ActionHud.AddMinion) // Add minions
        {
            for (int i = 0; i < minionsType.Count; i++)
            {
                if (minionsType[i] != null) continue;
                FSMBase refTypeMinion = Instantiate(minionType, spawnPosMinions[i].position, Quaternion.identity);
                refTypeMinion.GetComponent<Boid>().SetLeaderRef(positionsAroundLeader[i]);
                minionsType[i] = refTypeMinion;
                StartCoroutine(EnableMinion(refTypeMinion));
                break;
            }
        }
        else // Remove minions
        {
            for (int i = minionsType.Count - 1; i >= 0 ; i--)
            {
                if (minionsType[i] == null) continue;
                RemoveMinion(minionsType[i]);
                break;
            }
        }
    }

    private IEnumerator EnableMinion(FSMBase minion)
    {
        yield return new WaitForSeconds(0.5f);
        minion.OnNotify(NotifyActionObserver.StartGame);
    }
    
    public void RemoveMinion(FSMBase minion)
    {
        Unsubscribe(minion);
        Destroy(minion.gameObject);
    }

    public void EndGame(string message, FSMBase leaderType)
    {
        winMessage.enabled = true;
        winMessage.text = message + " team win!!";
        
        Unsubscribe(leaderType);
        
        Destroy(leaderType.gameObject);
        
        restartGameButton.gameObject.SetActive(true);
        redIaManageMinionsBttns.SetActive(false);
        blueIaManageMinionsBttns.SetActive(false);
        
        foreach (var item in _observersIa)
        {
            item.OnNotify(NotifyActionObserver.EndGame);
        }
    }
    
    public void StartGame()
    {
        startGameButton.enabled = false;
        startGameButton.gameObject.SetActive(false);
        
        foreach (var item in _observersIa)
        {
            item.OnNotify(NotifyActionObserver.StartGame);
        }
    }

    #region OnButtonsPressed
    
    public void OnButtonAddBlueMinion() => AddMinion(MinionType.BlueMinion);
    public void OnButtonRemoveBlueMinion() => SubstractMinion(MinionType.BlueMinion);
    public void OnButtonAddRedMinion() => AddMinion(MinionType.RedMinion);
    public void OnButtonRemoveRedMinion() => SubstractMinion(MinionType.RedMinion);
    public void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    
    #endregion
    
    public void Subscribe(IObserver observer)
    {
        if (!_observersIa.Contains(observer))
            _observersIa.Add(observer);
    }

    public void Unsubscribe(IObserver observer)
    {
        if (_observersIa.Contains(observer))
        {
            _observersIa.Remove(observer);
        }
    }
}
