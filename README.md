# OOJJRS' Unity UI Helper

`com.oojjrs.oui`는 Unity UGUI에서 반복해서 사용하는 버튼, 선택, 값 표시, 목록, 입력 컴포넌트를 제공하는 패키지입니다.

## 설치

Unity 6000.4 이상에서 Package Manager의 **Add package from git URL**에 다음 주소를 입력합니다.

```text
https://github.com/oojjrs/unity_oui.git?path=/Packages/src
```

## 구성 요소

| 구성 요소 | 종류 | 용도 |
| --- | --- | --- |
| `MyButton`, `MySelectable` | 입력 | 클릭·포커스·프레스·홀드 콜백 연결 |
| `MyRadio`, `MyRadioGroup`, `MyTab` | 선택 | 라디오 선택과 탭 상태 관리 |
| `MyText`, `MyImage`, `MyPortrait` | 표시 | UGUI 텍스트·이미지 갱신 |
| `MyList`, `MyReel` | 목록 | 프리팹 목록과 가상화 목록 관리 |
| `MyBar`, `MySlider`, `MyToggle`, `MySelector`, `MySwapper` | 값 | 값 기반 UI 상태 갱신 |
| `MyInput`, `MyAsker` | 흐름 | 입력 및 확인·예/아니오 모달 처리 |

## 사용 예시

`MyButton`과 콜백 컴포넌트를 같은 GameObject에 추가합니다.

```csharp
using oojjrs.oui;
using UnityEngine;

public sealed class StartButton : MonoBehaviour, MyButton.CallbackInterface
{
    void MyButton.CallbackInterface.OnClick()
    {
        Debug.Log("Start");
    }
}
```

런타임 헬퍼는 `DisallowMultipleComponent`를 사용하므로 같은 GameObject에 동일한 헬퍼 타입을 중복으로 추가할 수 없습니다. 패키지는 `com.unity.ugui` 2.0.0에 의존합니다.

자세한 구성은 [패키지 개요](Packages/src/README.md), 컴포넌트별 사용법은 [상세 문서](Packages/src/Documentation~/index.md)를 참고하세요.
