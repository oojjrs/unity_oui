# OOJJRS' Unity UI Helper

`OOJJRS' Unity UI Helper`는 Unity UGUI 위에서 반복적으로 필요한 UI 제어 코드를 작게 묶어 둔 런타임 패키지입니다. 모든 공개 타입은 `oojjrs.oui` 네임스페이스에 있습니다.

## 설치

Package Manager에서 Git URL을 추가합니다.

```text
https://github.com/oojjrs/unity_oui.git?path=/Packages/src
```

로컬 개발용 프로젝트에서는 `Packages/manifest.json`의 `com.oojjrs.oui` 항목이 `file:src`를 가리킵니다.

## 컴포넌트 배치 규칙

패키지의 런타임 헬퍼 컴포넌트는 한 GameObject에 같은 타입을 하나만 두는 것을 기준으로 합니다. 같은 오브젝트에 필수 UGUI 컴포넌트가 있어야 하는 타입은 `RequireComponent`로 의존성을 표시하고, 헬퍼 컴포넌트 본체는 `DisallowMultipleComponent`로 중복 부착을 막습니다.

## 버튼

`MyButton`은 Unity `Button` 컴포넌트 위에서 클릭 콜백, 호버 콜백, 프레스 콜백, 더블 클릭 콜백, 쿨다운, 애니메이션 트리거, 사운드 재생을 처리합니다.

콜백을 받을 컴포넌트는 필요한 인터페이스를 구현합니다.

```csharp
using oojjrs.oui;
using UnityEngine;

public sealed class StartButton : MonoBehaviour, MyButton.CallbackInterface
{
    public void OnClick()
    {
        Debug.Log("Start");
    }
}
```

전역 UI 사운드는 `MyControl.Audio`에 연결할 수 있고, 개별 버튼은 인스펙터의 사운드 오버라이드로 별도 `AudioSource`를 사용할 수 있습니다.

## 값 표시

`MyText`, `MyImage`, `MyPortrait`는 UGUI `Text`와 `Image` 갱신을 간단한 프로퍼티로 감쌉니다. `MyImage.SetNativeSizeSprite(sprite, nativeSizeScale)`는 `sprite`를 설정한 뒤 `SetNativeSize()`를 호출하고 `nativeSizeScale` 배율을 적용합니다. `1f`는 100%, `0.5f`는 50%, `2f`는 200% 크기입니다. 단순히 값을 읽거나 크기 조정 없이 교체할 때는 기존 `Sprite`, `OverrideSprite` 프로퍼티를 사용합니다.

`MyBar`, `MySlider`, `MyToggle`, `MySelector`, `MySwapper`는 값 기반 UI를 갱신할 때 사용합니다. 각 컴포넌트는 필요한 초기화 또는 변경 콜백 인터페이스를 함께 제공합니다.

## 탭

`MyTab`은 헤더 버튼 목록과 `MySelector`를 연결해 탭 선택 상태를 갱신합니다. 헤더 버튼에는 `MyTabHeaderButton`을 붙이고, 각 버튼은 부모 `MyTab`의 헤더 배열 기준 인덱스로 선택됩니다.

`OuiMoveNext()`와 `OuiMovePrevious()`는 현재 선택된 탭 기준으로 다음 또는 이전 interactable 탭을 찾습니다. `allowWrapAround`를 `true`로 넘기면 끝에서 반대편 탭으로 순환합니다.

## 리스트

`MyList`는 값 컬렉션과 프리팹 엔트리를 동기화합니다. 엔트리 컴포넌트는 `MyListEntry<TValue>`를 구현하고, 리스트 소유자는 `MyList.Master<TEntry, TValue>`를 구현합니다.

정렬이 필요하면 `MyList.SorterInterface<TValue>`를, 엔트리 추가 후 처리가 필요하면 `MyList.PostscriptInterface<TEntry, TValue>`를 함께 구현합니다.

## 입력과 모달

`MyInput`은 입력값 초기화, 값 변경, 제출 콜백을 분리해서 연결합니다.

`MyAsker`는 확인 모달과 예/아니오 모달의 열기, 닫기, 결과 콜백을 제공합니다. 추가 데이터는 `MyAsker.MyAskerArguments`로 전달합니다.
