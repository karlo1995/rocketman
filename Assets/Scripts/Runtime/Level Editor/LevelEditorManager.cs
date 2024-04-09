using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace SayDesign.LevelEditor
{
    public class LevelEditorManager : SerializedMonoBehaviour
    {
        [ShowInInspector] [ReadOnly] private static SpriteRenderer _platform;
        [ShowInInspector] [ReadOnly] private static Image _background;

        public static void CachePlatform()
        {
            _platform = GameObject.Find("Platform").GetComponent<SpriteRenderer>();
        }

        public static void CacheBackground()
        {
            _background = GameObject.Find("Background").GetComponent<Image>();
        }

        public static void SetBackgroundSprite(Sprite sprite)
        {
            if (_background == null) return;
            _background.sprite = sprite;
        }

        public static Sprite GetBackgroundSprite()
        {
            if (_background == null) return null;
            return _background.sprite;
        }

        public static void SetPlatformSprite(Sprite sprite)
        {
            if (_platform == null) return;
            _platform.sprite = sprite;
        }

        public static Sprite GetPlatformSprite()
        {
            if (_platform == null) return null;
            return _platform.sprite;
        }

        public static Vector2 GetPlatformPosition()
        {
            if (_platform == null) return Vector2.zero;
            return _platform.transform.position;
        }

        public static void ResetPlatformPosition()
        {
            if (_platform == null) return;
            _platform.transform.position = Vector2.zero;
        }

        public static void MovePlatform(Vector2 moveVector)
        {
            if (_platform == null) return;
            _platform.transform.position = new Vector3(Mathf.Round(_platform.transform.position.x * 100) / 100f, Mathf.Round(_platform.transform.position.y * 100) / 100f) + new Vector3(Mathf.Round(moveVector.x * 100) / 100f, Mathf.Round(moveVector.y * 100) / 100f);
        }
    }

    public enum PlatformMovement
    {
        None, OneDimensional, TwoDimensional
    }
}
