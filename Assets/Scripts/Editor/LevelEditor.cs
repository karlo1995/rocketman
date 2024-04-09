using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;
using SayDesign.LevelEditor;
using UnityEngine.UI;

namespace SayDesign.Editor
{
    internal class LevelEditor : OdinEditorWindow
    {
        [Title("Configure", HorizontalLine = false)]
        [OnValueChanged("SetPlatformSprite")]
        [SerializeField] private Sprite _platformSprite;

        [OnValueChanged("SetBackgroundSprite")]
        [SerializeField] private Sprite _backgroundSprite;

        [ReadOnly] [SerializeField] private Vector2 _platformPosition;

        [HorizontalGroup("entryrow1", Title = "Entry")]
        [SerializeField] private int _world, _level, _platform;

        [EnumToggleButtons]
        [HideLabel]
        [SerializeField] private PlatformMovement _platformMovement;

        [Button("1.0x", ButtonSizes.Large, ButtonAlignment = 0.5f, Icon = SdfIconType.ArrowUpCircleFill, Stretch = false)]
        private void UpOne()
        {
            LevelEditorManager.MovePlatform(new Vector2(0, 1));
        }

        [Button("0.1x", ButtonSizes.Large, ButtonAlignment = 0.5f, Icon = SdfIconType.ArrowUpCircleFill, Stretch = false)]
        private void UpPointOne()
        {
            LevelEditorManager.MovePlatform(new Vector2(0, 0.1f));
        }

        [ButtonGroup(), Button("1.0x", ButtonSizes.Large, Stretch = false, Icon = SdfIconType.ArrowLeftCircleFill)]
        private void LeftOne()
        {
            LevelEditorManager.MovePlatform(new Vector2(-1, 0));
        }

        [ButtonGroup(), Button("0.1x", ButtonSizes.Large, Stretch = false, Icon = SdfIconType.ArrowLeftCircleFill)]
        private void LeftPointOne()
        {
            LevelEditorManager.MovePlatform(new Vector2(-0.1f, 0));
        }

        [ButtonGroup(), Button("0.1x", ButtonSizes.Large, Stretch = false, Icon = SdfIconType.ArrowRightCircleFill, IconAlignment = IconAlignment.RightOfText)]
        private void RightPointOne()
        {
            LevelEditorManager.MovePlatform(new Vector2(0.1f, 0));
        }

        [ButtonGroup(), Button("1.0x", ButtonSizes.Large, Stretch = false, Icon = SdfIconType.ArrowRightCircleFill, IconAlignment = IconAlignment.RightOfText)]
        private void RightOne()
        {
            LevelEditorManager.MovePlatform(new Vector2(1, 0));
        }

        [Button("0.1x", ButtonSizes.Large, ButtonAlignment = 0.5f, Icon = SdfIconType.ArrowDownCircleFill, Stretch = false)]
        private void DownPointOne()
        {
            LevelEditorManager.MovePlatform(new Vector2(0, -0.1f));
        }

        [Button("1.0x", ButtonSizes.Large, ButtonAlignment = 0.5f, Icon = SdfIconType.ArrowDownCircleFill, Stretch = false)]
        private void DownOne()
        {
            LevelEditorManager.MovePlatform(new Vector2(0, -1));
        }

        [Button("Reset Position", ButtonSizes.Gigantic), GUIColor("red")]
        private void ResetPosition()
        {
            LevelEditorManager.ResetPlatformPosition();
        }

        [Button("Export", ButtonSizes.Gigantic), GUIColor("green")]
        private void Export()
        {
            CSVWriter.WriteCSV(_world, _level, _platform, LevelEditorManager.GetPlatformPosition(), _platformMovement, _platformSprite);
        }

        protected override void Initialize()
        {
            base.Initialize();
            LevelEditorManager.CachePlatform();
            LevelEditorManager.CacheBackground();
            _platformSprite = LevelEditorManager.GetPlatformSprite();
            _backgroundSprite = LevelEditorManager.GetBackgroundSprite();
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            UpdatePlatformPosition();
        }

        private void SetPlatformSprite()
        {
            LevelEditorManager.SetPlatformSprite(_platformSprite);
        }

        private void SetBackgroundSprite()
        {
            LevelEditorManager.SetBackgroundSprite(_backgroundSprite);
        }

        private void UpdatePlatformPosition()
        {
            _platformPosition = LevelEditorManager.GetPlatformPosition();
        }

        [MenuItem("Tools/Say Design/Level Editor")]
        private static void OpenWindow()
        {
            LevelEditor window = GetWindow<LevelEditor>();

            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
        }
    }
}
