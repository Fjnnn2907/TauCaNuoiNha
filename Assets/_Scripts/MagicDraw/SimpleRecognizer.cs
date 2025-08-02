using System.Collections.Generic;
using UnityEngine;

public static class SimpleRecognizer
{
    const int SAMPLE_COUNT = 32;
    const float TOLERANCE = 0.25f; // càng nhỏ càng nghiêm, càng lớn càng dễ đúng
    const float MIN_POINT_DISTANCE = 0.05f; // Khoảng cách tối thiểu giữa các điểm

    public static string Recognize(List<Vector2> input, List<SymbolPattern> patterns)
    {
        if (input.Count < 2) return "unknown";

        // Lọc điểm thừa trước khi xử lý
        var filteredInput = FilterRedundantPoints(input);
        var processedInput = NormalizePoints(filteredInput);

        string bestMatch = "unknown";
        float bestScore = float.MaxValue;

        foreach (var pattern in patterns)
        {
            // Lọc điểm thừa cho pattern cũng
            var filteredPattern = FilterRedundantPoints(pattern.points);
            var processedPattern = NormalizePoints(filteredPattern);
            float score = Compare(processedInput, processedPattern);

            if (score < bestScore)
            {
                bestScore = score;
                bestMatch = pattern.name;
            }
        }

        return bestScore < TOLERANCE ? bestMatch : "unknown";
    }

    // Hàm lọc điểm thừa
    static List<Vector2> FilterRedundantPoints(List<Vector2> points)
    {
        if (points.Count <= 2) return points;

        List<Vector2> filtered = new List<Vector2>();
        filtered.Add(points[0]);

        for (int i = 1; i < points.Count; i++)
        {
            float distance = Vector2.Distance(points[i], filtered[filtered.Count - 1]);
            if (distance >= MIN_POINT_DISTANCE)
            {
                filtered.Add(points[i]);
            }
        }

        return filtered;
    }

    static List<Vector2> NormalizePoints(List<Vector2> points)
    {
        var resampled = Resample(points, SAMPLE_COUNT);
        var rotated = RotateToZero(resampled);
        var scaled = ScaleToBox(rotated);
        var centered = TranslateToOrigin(scaled);
        return centered;
    }

    static float Compare(List<Vector2> a, List<Vector2> b)
    {
        float posScore = 0f;
        for (int i = 0; i < SAMPLE_COUNT; i++)
            posScore += Vector2.Distance(a[i], b[i]);

        float dirScore = CompareDirection(a, b) * SAMPLE_COUNT;

        // Thêm gấp khúc
        int cornersA = CountDirectionChanges(a);
        int cornersB = CountDirectionChanges(b);
        float cornerDiff = Mathf.Abs(cornersA - cornersB) / 10f; // normalize nhỏ

        return (posScore + dirScore + cornerDiff) / (2 * SAMPLE_COUNT); // hoặc chia 2.5 để bớt nhẹ
    }

    static float CompareDirection(List<Vector2> a, List<Vector2> b)
    {
        float sum = 0f;
        for (int i = 0; i < a.Count - 1; i++)
        {
            Vector2 dirA = (a[i + 1] - a[i]).normalized;
            Vector2 dirB = (b[i + 1] - b[i]).normalized;
            float angleDiff = Vector2.Angle(dirA, dirB) / 180f; // 0 to 1
            sum += angleDiff;
        }
        return sum / (a.Count - 1);
    }
    
    static int CountDirectionChanges(List<Vector2> points)
    {
        int count = 0;
        for (int i = 1; i < points.Count - 1; i++)
        {
            Vector2 dir1 = (points[i] - points[i - 1]).normalized;
            Vector2 dir2 = (points[i + 1] - points[i]).normalized;

            float angle = Vector2.Angle(dir1, dir2);
            if (angle > 30f) count++;
        }
        return count;
    }
    
    static List<Vector2> Resample(List<Vector2> points, int count)
    {
        if (points.Count <= 1) return points;

        float totalLength = 0f;
        for (int i = 1; i < points.Count; i++)
            totalLength += Vector2.Distance(points[i - 1], points[i]);

        if (totalLength < 0.01f) return points; // Tránh chia cho 0

        float interval = totalLength / (count - 1);
        List<Vector2> newPoints = new List<Vector2> { points[0] };
        float d = 0f;

        for (int i = 1; i < points.Count; i++)
        {
            float dist = Vector2.Distance(points[i - 1], points[i]);
            if (d + dist >= interval)
            {
                float t = (interval - d) / dist;
                Vector2 newPoint = Vector2.Lerp(points[i - 1], points[i], t);
                newPoints.Add(newPoint);
                points.Insert(i, newPoint);
                d = 0f;
            }
            else
            {
                d += dist;
            }
        }

        while (newPoints.Count < count)
            newPoints.Add(newPoints[newPoints.Count - 1]);

        return newPoints;
    }

    static List<Vector2> RotateToZero(List<Vector2> points)
    {
        Vector2 centroid = GetCentroid(points);
        Vector2 first = points[0];
        float angle = Mathf.Atan2(first.y - centroid.y, first.x - centroid.x);
        return RotateBy(points, -angle);
    }

    static List<Vector2> RotateBy(List<Vector2> points, float angle)
    {
        List<Vector2> rotated = new List<Vector2>();
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        Vector2 centroid = GetCentroid(points);
        foreach (var p in points)
        {
            float dx = p.x - centroid.x;
            float dy = p.y - centroid.y;
            float x = dx * cos - dy * sin + centroid.x;
            float y = dx * sin + dy * cos + centroid.y;
            rotated.Add(new Vector2(x, y));
        }
        return rotated;
    }

    static List<Vector2> ScaleToBox(List<Vector2> points)
    {
        Rect bounds = GetBounds(points);
        List<Vector2> scaled = new List<Vector2>();
        
        // Tránh chia cho 0
        float width = Mathf.Max(bounds.width, 0.01f);
        float height = Mathf.Max(bounds.height, 0.01f);
        
        foreach (var p in points)
        {
            float x = p.x / width;
            float y = p.y / height;
            scaled.Add(new Vector2(x, y));
        }
        return scaled;
    }

    static List<Vector2> TranslateToOrigin(List<Vector2> points)
    {
        Vector2 centroid = GetCentroid(points);
        List<Vector2> result = new List<Vector2>();
        foreach (var p in points)
        {
            result.Add(p - centroid);
        }
        return result;
    }

    static Vector2 GetCentroid(List<Vector2> points)
    {
        Vector2 sum = Vector2.zero;
        foreach (var p in points) sum += p;
        return sum / points.Count;
    }

    static Rect GetBounds(List<Vector2> points)
    {
        float minX = float.MaxValue, minY = float.MaxValue;
        float maxX = float.MinValue, maxY = float.MinValue;

        foreach (var p in points)
        {
            if (p.x < minX) minX = p.x;
            if (p.y < minY) minY = p.y;
            if (p.x > maxX) maxX = p.x;
            if (p.y > maxY) maxY = p.y;
        }

        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }
}
