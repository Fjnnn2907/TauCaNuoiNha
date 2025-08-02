using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class DrawingManager : MonoBehaviour
{

    public List<SymbolPattern> patterns;
    public LineRenderer lineRenderer;
    public TMP_Text resultText;
    public SymbolManager symbolManager;
    public ScoreManager scoreManager; // Thêm lại ScoreManager
    
    private List<Vector2> drawnPoints = new List<Vector2>();
    private bool isDrawing = false;
    
    // Thêm các tham số để lọc điểm
    private const float MIN_DISTANCE = 0.1f; // Khoảng cách tối thiểu giữa các điểm
    private Vector2 lastAddedPoint;

    void Start()
    {
        // Tạo 3 mẫu vẽ
        SymbolPattern circle = new SymbolPattern { name = "circle" };
        for (int i = 0; i < 24; i++)
        {
            float angle = i * Mathf.Deg2Rad * (360f / 24f);
            circle.points.Add(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)));
        }

        SymbolPattern zigzag = new SymbolPattern
        {
            name = "zigzag",
            points = new List<Vector2>
            {
                new Vector2(-1f, 1f),
                new Vector2(0.0f, 1f),
                new Vector2(-1f, 0f),
                new Vector2(0f, 0f),
                new Vector2(-1f, -1f)
            }
        };

        SymbolPattern vshape = new SymbolPattern
        {
            name = "vshape",
            points = new List<Vector2>
            {
                new Vector2(-1f, 1f),
                new Vector2(0f, -1f),
                new Vector2(1f, 1f)
            }
        };

        patterns = new List<SymbolPattern> { circle, zigzag, vshape };
        
        // Tạo symbol đầu tiên
        if (symbolManager != null)
        {
            symbolManager.GenerateNewSymbol();
        }
        
        // Bắt đầu minigame với độ khó Easy
        if (scoreManager != null)
        {
            scoreManager.StartMinigame(DifficultyLevel.Easy);
        }
        
        ClearDrawing();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDrawing = true;
            drawnPoints.Clear();
            lineRenderer.positionCount = 0;
        }

        if (Input.GetMouseButton(0) && isDrawing)
        {
            Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            // Chỉ thêm điểm nếu đủ xa điểm cuối cùng
            if (drawnPoints.Count == 0 || Vector2.Distance(point, lastAddedPoint) >= MIN_DISTANCE)
            {
                drawnPoints.Add(point);
                lastAddedPoint = point;
                lineRenderer.positionCount = drawnPoints.Count;
                lineRenderer.SetPosition(drawnPoints.Count - 1, point);
            }
        }

        if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            isDrawing = false;
            
            // Tối ưu hóa điểm trước khi nhận dạng
            var optimizedPoints = OptimizePoints(drawnPoints);
            RecognizeSymbol(optimizedPoints);
        }
    }

    // Hàm tối ưu hóa điểm - loại bỏ điểm thừa và làm mượt
    List<Vector2> OptimizePoints(List<Vector2> points)
    {
        if (points.Count <= 2) return points;

        List<Vector2> optimized = new List<Vector2>();
        optimized.Add(points[0]); // Luôn giữ điểm đầu

        for (int i = 1; i < points.Count - 1; i++)
        {
            Vector2 prev = points[i - 1];
            Vector2 current = points[i];
            Vector2 next = points[i + 1];

            // Tính góc giữa 3 điểm liên tiếp
            Vector2 dir1 = (current - prev).normalized;
            Vector2 dir2 = (next - current).normalized;
            float angle = Vector2.Angle(dir1, dir2);

            // Chỉ giữ điểm nếu góc thay đổi đủ lớn hoặc khoảng cách đủ xa
            float distance = Vector2.Distance(prev, current);
            if (angle > 15f || distance > MIN_DISTANCE * 2f)
            {
                optimized.Add(current);
            }
        }

        optimized.Add(points[points.Count - 1]); // Luôn giữ điểm cuối
        return optimized;
    }

    void RecognizeSymbol(List<Vector2> points)
    {
        string predicted = SimpleRecognizer.Recognize(points, patterns);

        if (predicted == symbolManager.currentSymbol)
        {
            resultText.text = "Đúng +" + (scoreManager?.pointsPerCorrectAnswer ?? 20) + " điểm";
            resultText.color = Color.green;
            
            // Tăng điểm khi nhận dạng đúng
            if (scoreManager != null)
            {
                scoreManager.AddScore();
            }
        }
        else
        {
            resultText.text = $"Sai rồi -" + (scoreManager?.pointsDeductionPerWrong ?? 5) + " điểm";
            resultText.color = Color.red;
            
            // Trừ điểm khi vẽ sai
            if (scoreManager != null)
            {
                scoreManager.DeductScore();
            }
        }

        Invoke(nameof(ResetForNext), 1.5f);
    }
    
    void ResetForNext()
    {
        resultText.text = "";
        resultText.color = Color.white;
        lineRenderer.positionCount = 0;
        
        // Tạo symbol mới
        if (symbolManager != null)
        {
            symbolManager.GenerateNewSymbol();
        }
    }
    
    // Xóa vẽ hiện tại
    public void ClearDrawing()
    {
        drawnPoints.Clear();
        lineRenderer.positionCount = 0;
        resultText.text = "";
        resultText.color = Color.white;
    }

    string FakeRecognizer(List<Vector2> points)
    {
        // 💡 Tạm nhận dạng siêu đơn giản bằng độ dài và hướng
        if (points.Count < 5) return "vshape";

        Vector2 start = points[0];
        Vector2 end = points[points.Count - 1];
        float dx = Mathf.Abs(end.x - start.x);
        float dy = Mathf.Abs(end.y - start.y);

        if (dx < 0.5f && dy > 1f) return "circle";
        if (dx > dy) return "zigzag";
        return "vshape";
    }
}
[System.Serializable]
public class SymbolPattern
{
    public string name;
    public List<Vector2> points = new List<Vector2>();
}
