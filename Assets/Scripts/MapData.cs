using UnityEngine;

namespace System
{
    [CreateAssetMenu]
    public class MapData : ScriptableObject
    {
        [Header("맵 프리팹")]
        public GameObject mapPrefab;
        [Header("플레이어 처음 생성 위치")]
        public Vector2 playerPosition;
        [Header("글로벌 라이트 설정")]
        public Color globalLight;
        [Header("게임 플레이 시간")] public float playTime;
        [Header("보스전의 유무")] public bool hasBossBattle;
    }
}
