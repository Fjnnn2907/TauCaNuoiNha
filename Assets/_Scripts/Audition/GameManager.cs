using UnityEngine;

namespace test
{
    public class GameManager : MonoBehaviour
    {
        public AuditionManager ui;
        public DifficultyLevel difficultyLevel;

        void Start()
        {
            ui.SetDifficulty(difficultyLevel);
        }
    }

}

