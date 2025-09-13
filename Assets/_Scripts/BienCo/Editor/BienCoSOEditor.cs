using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BienCoSO))]
public class BienCoSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BienCoSO bienCo = (BienCoSO)target;

        EditorGUILayout.LabelField("Thông tin cơ bản", EditorStyles.boldLabel);
        bienCo.tenBienCo = EditorGUILayout.TextField("Tên Biến Cố", bienCo.tenBienCo);
        bienCo.moTaBienCo = EditorGUILayout.TextArea(bienCo.moTaBienCo, GUILayout.Height(60));
        bienCo.iconBienCo = (Sprite)EditorGUILayout.ObjectField("Icon", bienCo.iconBienCo, typeof(Sprite), false);
        bienCo.loaiBienCo = (bienCoType)EditorGUILayout.EnumPopup("Loại Biến Cố", bienCo.loaiBienCo);
        bienCo.xacSuatXuatHien = EditorGUILayout.Slider("Tỉ lệ xuất hiện", bienCo.xacSuatXuatHien, 0f, 1f);

        EditorGUILayout.Space();
        bienCo.tenBienCoKey = EditorGUILayout.TextField("Key Tên Biến Cố", bienCo.tenBienCoKey);
        bienCo.moTaBienCoKey = EditorGUILayout.TextField("Key Mô tả Biến Cố", bienCo.moTaBienCoKey);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Dữ liệu theo loại biến cố", EditorStyles.boldLabel);

        switch (bienCo.loaiBienCo)
        {
            case bienCoType.TruTien:
            case bienCoType.CongTien:
                bienCo.giaTriTien = EditorGUILayout.IntField("Giá trị tiền", bienCo.giaTriTien);
                break;

            case bienCoType.MatCanCau:
            case bienCoType.ThemCanCau:
                bienCo.rodData = (FishingRodData)EditorGUILayout.ObjectField("Cần câu", bienCo.rodData, typeof(FishingRodData), false);
                bienCo.soLuongCanCau = EditorGUILayout.IntField("Số lượng cần câu", bienCo.soLuongCanCau);
                break;

            case bienCoType.MatMoiCau:
            case bienCoType.ThemMoiCau:
                EditorGUILayout.LabelField("Danh sách mồi câu", EditorStyles.boldLabel);
                for (int i = 0; i < bienCo.baitEffects.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    bienCo.baitEffects[i].bait = (FishingBaitData)EditorGUILayout.ObjectField(bienCo.baitEffects[i].bait, typeof(FishingBaitData), false);
                    bienCo.baitEffects[i].quantity = EditorGUILayout.IntField(bienCo.baitEffects[i].quantity);
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        bienCo.baitEffects.RemoveAt(i);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("+ Thêm mồi"))
                    bienCo.baitEffects.Add(new BaitEffect());

                break;

            case bienCoType.MatCa:
            case bienCoType.DuocThemCa:
            case bienCoType.BanCa:
                EditorGUILayout.LabelField("Danh sách cá", EditorStyles.boldLabel);
                for (int i = 0; i < bienCo.fishEffects.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    bienCo.fishEffects[i].fish = (FishData)EditorGUILayout.ObjectField(bienCo.fishEffects[i].fish, typeof(FishData), false);
                    bienCo.fishEffects[i].quantity = EditorGUILayout.IntField(bienCo.fishEffects[i].quantity);
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        bienCo.fishEffects.RemoveAt(i);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button("+ Thêm cá"))
                    bienCo.fishEffects.Add(new FishEffect());
                break;

            default:
                EditorGUILayout.HelpBox("Loại biến cố chưa hỗ trợ UI cụ thể.", MessageType.Info);
                break;
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(bienCo);
        }
    }
}
