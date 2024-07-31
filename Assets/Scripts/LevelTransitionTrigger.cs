using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransitionTrigger : MonoBehaviour
{
    public GameObject currentLevel; // 当前所在场景
    public Vector2Int targetLevelIndex;
    bool loading = false;

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!loading)
        {
            loading = true;
            StartCoroutine(Game.DarkScreen.WaitForLoading(ChangeLevel()));
        }
    }

    IEnumerator ChangeLevel()
    {
        GameObject player = Game.Player.gameObject;
        player.SetActive(false);
        string level = Game.LevelPath[targetLevelIndex];
        var request = Resources.LoadAsync(level);
        yield return request;
        GameObject prefab = request.asset as GameObject;
        Instantiate(prefab);
        player.SetActive(true);
        Destroy(currentLevel);
    }
}
