using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;

public class SaveManager : Singleton<SaveManager>
{
    [SerializeField] private string _fileName = "SaveData.json";
    [SerializeField] private bool isMaHoa = true;

    private FileDataHandler dataHandler;
    private GameData gameData;
    private HashSet<ISaveable> saveables = new();

    protected override void Awake()
    {
        base.Awake();

        dataHandler = new FileDataHandler(Application.persistentDataPath, _fileName, isMaHoa);
    }

    private async void Start()
        => await LoadGameAsync();

    private void OnApplicationQuit()
        => _ = SaveGameAsync();

    // dang ky save
    public void RegisterSaveable(ISaveable saveable)
        => saveables.Add(saveable);

    // huy dang ky
    public void UnregisterSaveable(ISaveable saveable)
        => saveables.Remove(saveable);

    // luu du lieu game
    public async Task SaveGameAsync()
    {
        gameData ??= new GameData();

        foreach (var saveable in saveables)
            saveable.SaveData(ref gameData);

        await dataHandler.SaveDataAsync(gameData);
    }

    // tai du lieu game
    public async Task LoadGameAsync()
    {
        gameData = await dataHandler.LoadDataAsync();

        if (gameData == null)
        {
            Debug.Log("Khong co data tao data moi");
            gameData = new GameData();
        }

        foreach (var saveable in saveables)
            saveable.LoadData(gameData);
    }

    [ContextMenu("===Delete Data===")]
    public void DeleteSave()
        => dataHandler.DeleteData();

    public bool HasSaveData()
    {
        return File.Exists(dataHandler.FullPath);
    }
}