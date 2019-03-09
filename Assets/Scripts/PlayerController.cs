using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 移動用の簡易スクリプト
// 移動中のパーティクル挙動確認用なので結構適当
public class PlayerController : MonoBehaviour
{
    [Header("移動関連")]
    [SerializeField]
    private Camera m_MainCamera = null;
    [SerializeField, Range(0.1f, 1.0f)]
    private float m_MoveSpeed = 0.1f;
    [SerializeField, Range(0.1f, 1.0f)]
    private float m_CameraRotateSpeed = 0.1f;

    [Header("パーティクル関連"), Space(5.0f)]
    [SerializeField]
    private ParticleHorming m_ParticleHorming = null;
    [SerializeField]
    private RectTransform m_GaugeRectTransform = null;
    [SerializeField]
    private RectTransform m_MeterRectTransform = null;

    private float m_MaxMeterWidth = 0.0f;
    private float m_NowMeterWidth = 0.0f;

    // パーティクル消失時のコールバック
    private void EndParticleCallback()
    {
        m_NowMeterWidth = Mathf.Min(m_MaxMeterWidth, m_NowMeterWidth + 1.0f);
        Vector2 sizeDelta = m_MeterRectTransform.sizeDelta;
        sizeDelta.x = m_NowMeterWidth;
        m_MeterRectTransform.sizeDelta = sizeDelta;
    }

    void Reset()
    {
        m_MoveSpeed = 0.1f;
        m_CameraRotateSpeed = 0.1f;
    }

    void Awake()
    {
        if(m_MainCamera == null)
        {
            Debug.LogError("MainCameraがnullです");
            return;
        }
        m_MainCamera.transform.SetParent(transform, false);

        // パーティクルとUIの設定
        m_MaxMeterWidth = m_GaugeRectTransform.sizeDelta.x;
        m_ParticleHorming.SetEndParticleCallback(EndParticleCallback);
    }

    void Update()
    {
        if(m_MainCamera == null) return;

        // パーティクル発射チェック
        if(Input.GetKeyDown(KeyCode.Z))
        {
            m_ParticleHorming.CreateParticle();
        }

        // 移動入力チェック
        int inputHorizontal = (int)Input.GetAxisRaw("Horizontal");
        int inputVertical = (int)Input.GetAxisRaw("Vertical");
        if(inputHorizontal == 0 && inputVertical == 0) return;

        // 向きの回転
        transform.Rotate(Vector3.up, m_CameraRotateSpeed * inputHorizontal);
        // 移動処理
        Vector3 nextPositon = transform.position;
        nextPositon += transform.forward * m_MoveSpeed * inputVertical;
        transform.position = nextPositon;
    }
}
