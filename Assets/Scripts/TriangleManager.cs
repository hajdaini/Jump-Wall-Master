using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TriangleManager : MonoBehaviour
{

    [SerializeField] GameObject triangleObj;
    Transform leftWall, rightWall;



    [Header("Config")]
    [SerializeField] float scale = 1;
    [SerializeField] int minTrianglesNumber;
    [SerializeField] int maxTrianglesNumber;
    [SerializeField] float deltaY;
    [SerializeField] int deltaToIncreaseTriangles;


    int maxTrianglesPossible, numberOfTriangles, currentWallCollision = 1;
    Vector2[] leftCornersPos, rightCornesPos;
    float offsetXLeft, offsetXRight, offsetYLeft, offsetYRight;
    float deltaScale;

    Vector2 leftTopWallPos, leftBottomWallPos, rightTopWallPos, rightBottomWallPos;

    void Start(){
        Init();
    }

    [ContextMenu("Init")]
    public void Init(){
        leftWall = GameManager.instance.leftWall;
        rightWall = GameManager.instance.rightWall;
        leftCornersPos = GetSpriteCorners(leftWall.GetComponent<SpriteRenderer>());
        rightCornesPos = GetSpriteCorners(rightWall.GetComponent<SpriteRenderer>());

        numberOfTriangles = minTrianglesNumber;
        offsetXLeft = (leftWall.localScale.x / 4f) * 0.7f;
        offsetXRight = (rightWall.localScale.x / 4f) * 0.7f;
        deltaScale = (triangleObj.transform.localScale.y * scale /2);

        int indexTmp = 0;
        while(true){
            float y = rightCornesPos[3].y + (deltaY * indexTmp) + deltaScale;
            if(y > rightCornesPos[0].y) { 
                maxTrianglesPossible = indexTmp; 
                break; 
            }
            indexTmp+= 1;
        }
        // Test();
        CreateTriangles(true);
        CreateTriangles(false);
    }

    // test all triangles number at start
    void Test(){
        DeleteTriangles(true);
        DeleteTriangles(false);

        for (int i = 0; i < 30; i++){
            if(numberOfTriangles > maxTrianglesPossible) break;
            GameObject tempObj = null;
                tempObj = Instantiate(triangleObj, new Vector2(rightCornesPos[0].x - offsetXRight, rightCornesPos[3].y + (deltaY * i) + deltaScale), Quaternion.Euler(0,0,180));
                SetScale(tempObj);
                if(tempObj.transform.position.y > rightCornesPos[0].y) Destroy(tempObj);
                else tempObj.transform.SetParent(rightWall);

                tempObj = Instantiate(triangleObj, new Vector2(leftCornersPos[0].x + offsetXLeft, leftCornersPos[0].y + (deltaY * i) + deltaScale), Quaternion.identity);
                SetScale(tempObj);
                if(tempObj.transform.position.y > leftCornersPos[3].y) Destroy(tempObj);
                else tempObj.transform.SetParent(leftWall);
        }
    }


    public void WallTouched(bool isRight){
        CreateTriangles(isRight);
    }


    void CreateTriangles(bool isRight){

        DeleteTriangles(isRight);
        List<int> numberList = Enumerable.Range(1, maxTrianglesPossible).ToList();

        for (int i = 0; i < numberOfTriangles; i++){
            if(numberOfTriangles > maxTrianglesPossible) break;
            int randomIndex = Random.Range(0, numberList.Count);
            GameObject tempObj = null;
            if(isRight){
                tempObj = Instantiate(triangleObj, new Vector2(rightCornesPos[0].x - offsetXRight, rightCornesPos[3].y + (deltaY * randomIndex) + deltaScale), Quaternion.Euler(0,0,180));
                SetScale(tempObj);
                if(tempObj.transform.position.y > rightCornesPos[0].y) Destroy(tempObj);
                else tempObj.transform.SetParent(rightWall);
            }else{
                tempObj = Instantiate(triangleObj, new Vector2(leftCornersPos[0].x + offsetXLeft, leftCornersPos[0].y + (deltaY * randomIndex) + deltaScale), Quaternion.identity);
                SetScale(tempObj);
                if(tempObj.transform.position.y > leftCornersPos[3].y) Destroy(tempObj);
                else tempObj.transform.SetParent(leftWall);
                numberList.Remove(randomIndex);
            }

            currentWallCollision += 1;
            if(currentWallCollision > deltaToIncreaseTriangles * 2) {
                currentWallCollision = 0;
                numberOfTriangles += 1;
                numberOfTriangles = Mathf.Clamp(numberOfTriangles, 0, maxTrianglesNumber);
            } 
        }
    }

    void SetScale(GameObject go){
        foreach (Transform child in go.transform) child.localScale = new Vector2(scale, scale);
    }

    void DeleteTriangles(bool isRight){
        if (isRight) foreach (Transform child in rightWall) Destroy(child.gameObject);
        else foreach (Transform child in leftWall) Destroy(child.gameObject);
    }

    public Vector2[] GetSpriteCorners(SpriteRenderer renderer){
        Vector2 topRight = renderer.transform.TransformPoint(renderer.sprite.bounds.max);
        Vector2 topLeft = renderer.transform.TransformPoint(new Vector2(renderer.sprite.bounds.max.x, renderer.sprite.bounds.min.y));
        Vector2 botLeft = renderer.transform.TransformPoint(renderer.sprite.bounds.min);
        Vector2 botRight = renderer.transform.TransformPoint(new Vector2(renderer.sprite.bounds.min.x, renderer.sprite.bounds.max.y));
        return new Vector2[] { topRight, topLeft, botLeft, botRight };
    }

}
