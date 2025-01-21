using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace System
{
    public class MapManagementSystem : MonoBehaviour
    {
        public static MapManagementSystem instance;
        public MapData[] mapData;
        public int currentMapIndex;
        public Image fader;
        public float fadeSpeed;
        public GameObject player;
        public Light2D globalLight;
        
        private void Awake()
        {
            instance = this;
            currentMapIndex = 0;
        }

        public void ChangeMap(int idx)
        {
            StartCoroutine(FadeInAndOutFlow(idx));
        }

        public void GoToNext()
        {
            StartCoroutine(FadeInAndOutFlow(currentMapIndex + 1));
            SaveIdx(currentMapIndex+1);
        }

        public void GoToHome()
        {
            StartCoroutine(FadeInAndOutFlow(0));
        }

        private IEnumerator FadeInAndOutFlow(int idx)
        {
            var visblity = 0f;
            while (fader.color.a < 0.8f)
            {
                visblity += Time.deltaTime * fadeSpeed;
                fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, visblity);
                yield return null;

            }

            fader.color = new Color(0, 0, 0, 1);
            Destroy(gameObject.transform.GetChild(0).gameObject);
            var newMap = Instantiate(mapData[idx].mapPrefab, new Vector2(0, 0), Quaternion.identity);
            newMap.transform.parent = gameObject.transform;
            //player.transform.position = new Vector2(mapData[idx].playerPosX, mapData[idx].playerPosY);
            globalLight.color = mapData[currentMapIndex].globalLight;
            
            while (fader.color.a > 0)
            {
                visblity -= Time.deltaTime * fadeSpeed;
                fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, visblity);
                yield return null;
                
            }
            fader.color = new Color(0, 0, 0, 0);
        }

        public void SaveIdx(int idx)
        {
            currentMapIndex = idx;
        }
        
    }
}
