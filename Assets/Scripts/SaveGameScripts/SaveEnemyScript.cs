using UnityEngine;

public class SaveEnemyScript : MonoBehaviour, ISaveGame
{
    [SerializeField]
    int enemyId;
    EnemyMovement enemyMovement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void LoadData(GameData data)
    {
        //soundMeterManage.SetCurrentSoundLevel(data.soundMeterLevel);
        //Debug.Log("Loaded slider value with : " + data.soundMeterLevel);

        for (int i = 0; i < data.enemyData.Length; i++)
        {
            if (data.enemyData[i].enemyId == enemyId)
            {
                transform.position = new Vector3(data.enemyData[i].enemyPoz[0], data.enemyData[i].enemyPoz[1], data.enemyData[i].enemyPoz[2]);
                if (enemyMovement != null)
                {
                    enemyMovement.currentPathPoint = data.enemyData[i].currentPathPoint;
                    enemyMovement.CurrentTarget = enemyMovement.pathPoints[enemyMovement.currentPathPoint].gameObject;
                    enemyMovement.agent.SetDestination(enemyMovement.CurrentTarget.transform.position);
                }
            }
        }
    }
    public void SaveData(GameData data)
    {
        EnemyData enemyData = new EnemyData();
        enemyData.enemyId = enemyId;
        enemyData.enemyPoz = new float[3];
        enemyData.enemyPoz[0] = transform.position.x;
        enemyData.enemyPoz[1] = transform.position.y;
        enemyData.enemyPoz[2] = transform.position.z;
        if(enemyMovement != null)
        {
            enemyData.currentPathPoint = enemyMovement.currentPathPoint;
        }
    
        data.AddEnemyData(enemyData);
    }
}
