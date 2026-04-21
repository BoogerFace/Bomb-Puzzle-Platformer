using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject fakePlayer;


    private void OnLevelMakerReset()
    {
        fakePlayer.SetActive(true);
    }


    private void OnLevelMakerStart()
    {
        fakePlayer.SetActive(false);
    }
}
