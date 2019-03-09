using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

// 3D空間にあるオブジェクトから座標変換を通して指定のCanvasUIに向かってパーティクルを飛ばすスクリプト
public class ParticleHorming : MonoBehaviour
{
    [SerializeField, Header("メインカメラ")]
    private Camera m_Camera = null;
    [SerializeField, Header("座標変換用のCanvas")]
    private RectTransform m_RaycasterCameraCanvasRectTransform = null;
    [SerializeField, Header("放出する3DObjectTransform")]
    private List<Transform> m_Emit3DTransformList = null;
    [SerializeField, Header("パーティクルが収束するUIの位置")]
    private RectTransform m_TargetTransform = null;
    [SerializeField]
    private ParticleSystem m_ParticleSystem = null;
    [SerializeField, Range(0, 10)]
    private int m_EmitParticleCount = 5;
    [SerializeField, Range(0.0f, 10.0f), Header("パーティクルの反動力")]
    private float m_ReactionDistance = 5.0f;

    // パーティクル更新用
    private int m_ActiveParticleCount = 0;
    private ParticleSystem.Particle[] m_ParticleList = null;

    // パーティクル消失時のコールバック
    private UnityAction m_EndParticleCallback = null;

    // アクティブパーティクルの更新
    private void UpdateActiveParticleNum()
    {
        m_ActiveParticleCount = m_ParticleSystem.GetParticles(m_ParticleList);
    }

    void Awake()
    {
        m_ParticleList = new ParticleSystem.Particle[m_ParticleSystem.main.maxParticles];
        UpdateActiveParticleNum();
    }

    void LateUpdate()
    {
        int preActiveCount = m_ActiveParticleCount;
        UpdateActiveParticleNum();
        // もし前回よりアクティブなパーティクルが減っていてコールバックが設定されていたらコールバックを呼ぶ
        if(preActiveCount > m_ActiveParticleCount && m_EndParticleCallback != null)
        {
            int removeSize = preActiveCount - m_ActiveParticleCount;
            for(int i = 0; i < removeSize; ++i)
            {
                m_EndParticleCallback();
            }
        }

        // 何もない場合はここで終了
        if(m_ActiveParticleCount <= 0) return;

        // 更新
        for(int i = 0; i < m_ActiveParticleCount; ++i)
        {
			float rate = (1.0f - m_ParticleList[i].remainingLifetime / m_ParticleList[i].startLifetime);
			rate = Mathf.Pow(rate, m_ReactionDistance);　// 指数関数を加えることにより、収束する勢いを変更できるようにしてる
			m_ParticleList[i].position = Vector3.Lerp(m_ParticleList[i].position, m_TargetTransform.position, rate);
        }
        m_ParticleSystem.SetParticles(m_ParticleList, m_ActiveParticleCount);
    }

    // パーティクル消失時のコールバック設定
    public void SetEndParticleCallback(UnityAction callback)
    {
        m_EndParticleCallback = callback;
    }

#if UNITY_EDITOR
    [ContextMenu("CreateParticle")]
#endif
    public void CreateParticle()
    {
        // 3D空間座標からカメラスクリーン上の座標に変換する
        Vector3 basePos = m_Emit3DTransformList.GetRandom().position;   // GetRandom()についてはListExtensionsを参照
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(m_Camera, basePos);
        Vector3 screenPos3D = m_Camera.WorldToScreenPoint(basePos);
        Debug.Log(screenPos);
        Debug.Log(screenPos3D);

        // カメラスクリーン座標をキャンバス上のローカル座標に変換する
        Vector2 cameraCanvasPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_RaycasterCameraCanvasRectTransform, screenPos, m_Camera, out cameraCanvasPos);

        // 座標確認
        Debug.LogFormat("rectPos{0}", cameraCanvasPos);

        // ParticleSystemを放出位置に移動させてEmit
        m_ParticleSystem.transform.localPosition = cameraCanvasPos;
        m_ParticleSystem.Emit(m_EmitParticleCount);
    }
}
