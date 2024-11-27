using System.Collections;
using UnityEngine;

// SoundManager에서 사용하기 위한 코루틴 핸들러
public class CoroutineHandler : MonoBehaviour
{
    private static CoroutineHandler instance;

    void Awake()
    {
        if(!instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void StartStaticCoroutine(IEnumerator coroutine)
    {
        if (!instance)
        {
            // 새로운 GameObject 생성
            GameObject obj = new GameObject("CoroutineHandler");
            // 새 GameObject에 CoroutineHandler 컴포넌트를 추가하고, 이를 instance로 설정
            instance = obj.AddComponent<CoroutineHandler>();
        }
        instance.StartCoroutine(coroutine);
    }
}