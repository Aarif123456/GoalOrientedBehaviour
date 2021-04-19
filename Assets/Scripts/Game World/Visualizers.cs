using UnityEngine;

public sealed class Visualizers : MonoBehaviour
{
    public GameObject capsuleCastPrefab;
    
    private static Visualizers _instance;
    
    public static Visualizers Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("Game").GetComponent<Visualizers>();
            }
            
            return _instance;
        }
    }
    
    public GameObject CreateCapsuleCastVisualizer(Vector3 start, Vector3 direction, float length)
    {
        GameObject capsuleCastVisualizer = Instantiate(capsuleCastPrefab) as GameObject;
        capsuleCastVisualizer.transform.position = start;
        
        Transform top = capsuleCastVisualizer.transform.Find("Top");
        Transform middle = capsuleCastVisualizer.transform.Find("Middle");
        Transform bottom = capsuleCastVisualizer.transform.Find("Bottom");
        Transform end = capsuleCastVisualizer.transform.Find("End");
        
        top.localPosition = length * direction / 2 + Vector3.up * 0.5f;
        middle.localPosition = length * direction / 2;
        bottom.localPosition = length * direction / 2 - Vector3.up * 0.5f;
        end.localPosition = length * direction;
        
        Vector3 scale = top.localScale;
        scale.y *= length;
        top.localScale = scale;
    
        scale = middle.localScale;
        scale.x *= length;
        middle.localScale = scale;
        
        scale = bottom.localScale;
        scale.y *= length;
        bottom.localScale = scale;
        
        top.LookAt(top.position + direction);
        top.transform.Rotate(Vector3.right, 90);
        
        middle.LookAt(middle.position + direction);
        middle.transform.Rotate(Vector3.up, 90);
        
        bottom.LookAt(bottom.position + direction);
        bottom.transform.Rotate(Vector3.right, 90);
        
        end.LookAt(end.position + direction);
        
        return capsuleCastVisualizer;
    }
}