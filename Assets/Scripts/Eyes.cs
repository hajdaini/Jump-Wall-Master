using UnityEngine;
using System.Collections;

public class Eyes : MonoBehaviour 
{
	[HideInInspector] public SpriteRenderer eyesSpriteRenderer;

	[SerializeField] float m_MinBlinkInterval = 2.0f;
	[SerializeField] float m_MaxBlinkInterval = 3.0f;
	[SerializeField] float m_MinBlinkTime = 0.1f;
	[SerializeField] float m_MaxBlinkTime = 0.15f;
	[SerializeField] GameObject m_PupilLeft;
	[SerializeField] GameObject m_PupilRight;
	public GameObject whiteEyes;
	[SerializeField] float deltaX = 10.0f;

	float m_BlinkTimer;
	bool m_Blinking;
	Vector2 m_PupilLeftCentre;
	Vector2 m_PupilRightCentre;

	void Start () 
	{
		eyesSpriteRenderer = GetComponent<SpriteRenderer>();
		m_BlinkTimer = UnityEngine.Random.Range(m_MinBlinkInterval, m_MaxBlinkInterval);
		m_PupilLeftCentre = m_PupilLeft.transform.localPosition;
		m_PupilRightCentre = m_PupilRight.transform.localPosition;
	}
	

	void Update () 
	{	
		m_BlinkTimer -= Time.deltaTime;

		if(m_BlinkTimer < 0.0f)
		{
			m_Blinking = !m_Blinking;

			if(m_Blinking)
			{
				m_BlinkTimer = UnityEngine.Random.Range(m_MinBlinkTime, m_MaxBlinkTime);
				eyesSpriteRenderer.sprite = PlayerCustomize.instance.eyesClosedSprite;

				whiteEyes.SetActive(false);
				m_PupilLeft.SetActive(false);
				m_PupilRight.SetActive(false);
			}
			else{
				m_BlinkTimer = UnityEngine.Random.Range(m_MinBlinkInterval, m_MaxBlinkInterval);
				eyesSpriteRenderer.sprite = PlayerCustomize.instance.eyesBorderSprite;
				whiteEyes.SetActive(true);
				m_PupilLeft.SetActive(true);
				m_PupilRight.SetActive(true);
			}
		}

        float posX = transform.parent.GetComponent<Rigidbody2D>().velocity.x;
        float posY = transform.parent.GetComponent<Rigidbody2D>().velocity.y;
        posX = Mathf.Clamp(posX, -deltaX, deltaX);
        posY = Mathf.Clamp(posX, -deltaX, deltaX);
        

        m_PupilLeft.transform.localPosition = m_PupilLeftCentre + (Vector2.right * posX) + (Vector2.up * posY);
        m_PupilRight.transform.localPosition = m_PupilRightCentre + (Vector2.right * posX) + (Vector2.up * posY);
	}
}
