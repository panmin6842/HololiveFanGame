using UnityEngine;

public class CardMove : MonoBehaviour
{
    private CardBasicAttack cardBasicAttack;
    private GameObject player;

    private float time = 0f;
    private float curveSpeed = 2f;

    private Vector3 p0, p1, p2; //p0은 시작 지점 p1은 곡선의 정점 오른쪽으로 가면 최대로 오른쪽으로 가는 지점 p3는 최종 목적지

    private float maxLeanAngle = 60f;
    private float leanSoothSpeed = 10f;

    private float currentRollAngle = 0f; // 현재 카드가 기울어진 각도
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        cardBasicAttack = player.GetComponent<CardBasicAttack>();
    }

    public void InitializeCurve(Vector3 start, Vector3 control, Vector3 end)
    {
        p0 = start;
        p1 = control;
        p2 = end;

        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (time < 1f)
        {
            time += Time.deltaTime * curveSpeed;

            //베지에 곡선 공식 대입
            
            Vector3 m1 = Vector3.Lerp(p0, p1, time);
            Vector3 m2 = Vector3.Lerp(p1, p2, time);
            Vector3 currentPos = Vector3.Lerp(m1, m2, time);

            //현재 프레임 및 아주 미세한 다음 프레임의 위치 계산 (방향성 정밀 측정용)
            float nextTime = Mathf.Min(time + 0.01f, 1f);
            Vector3 nm1 = Vector3.Lerp(p0, p1, nextTime);
            Vector3 nm2 = Vector3.Lerp(p1, p2, nextTime);
            Vector3 nextPos = Vector3.Lerp(nm1, nm2, nextTime);

            Vector3 moveDirection = (nextPos - currentPos).normalized;

            transform.position = currentPos;

            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

                //휘어지는 방향 계산
                //시작점에서 도착지점 직선을 기준으로 현재 얼마나 휘어지는지 확인
                Vector3 straightLine = (p2 - p1).normalized;
                Vector3 currentDeviation = (currentPos - p0).normalized;

                //우측 커버인지 좌측 커브인지 확인
                float curveSign = Vector3.Cross(straightLine, currentDeviation).y; //외적 계산

                //베지에 곡선 중간일때 가장 많이 눕도록 가중치 부여
                float curveWeight = Mathf.Sin(time * Mathf.PI);

                //최종 목표 기울기
                float targetRollAngle = curveSign * maxLeanAngle * curveWeight;

                currentRollAngle = Mathf.Lerp(currentRollAngle, targetRollAngle, Time.deltaTime * leanSoothSpeed);
                transform.rotation = targetRotation * Quaternion.Euler(0, 0, currentRollAngle);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
