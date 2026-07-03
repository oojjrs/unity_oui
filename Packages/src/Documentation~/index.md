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

`MyButton`은 Unity `Button` 컴포넌트 위에서 클릭 콜백, 호버 콜백, 프레스 콜백, 더블 클릭 콜백, 쿨다운, 애니메이션 트리거, pointer/focus 기반 hover 사운드와 클릭 사운드 재생을 처리합니다. 버튼의 이미지 참조는 `MyImage` 기준으로 맞춰 `Sprite` 갱신이 다른 이미지 래퍼와 같은 경로를 사용합니다.

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

전역 UI 사운드는 `MyControl.Audio`에 연결할 수 있고, 개별 버튼은 인스펙터의 사운드 오버라이드로 별도 `AudioSource`를 사용할 수 있습니다. focus hover 사운드는 click 또는 pointer hover 사운드와 같은 프레임에 겹치면 재생하지 않습니다.

## 값 표시

`MyText`, `MyImage`, `MyPortrait`는 UGUI `Text`와 `Image` 갱신을 간단한 프로퍼티로 감쌉니다. `MyImage.SetNativeSizeSprite(sprite, nativeSizeScale)`는 `sprite`를 설정한 뒤 `SetNativeSize()`를 호출하고 `nativeSizeScale` 배율을 적용합니다. `1f`는 100%, `0.5f`는 50%, `2f`는 200% 크기입니다. 단순히 값을 읽거나 크기 조정 없이 교체할 때는 기존 `Sprite`, `OverrideSprite` 프로퍼티를 사용합니다.

`MyBar`, `MySlider`, `MyToggle`, `MySelector`, `MySwapper`는 값 기반 UI를 갱신할 때 사용합니다. 각 컴포넌트는 필요한 초기화 또는 변경 콜백 인터페이스를 함께 제공합니다.

## 라디오

`MyRadio`와 `MyRadioGroup`은 Unity `Toggle`, `ToggleGroup`, `Selectable`에 기대지 않고 라디오 버튼과 토글 묶음을 구성합니다. `MyRadio`는 `IsOn`과 `IsInteractable`을 Inspector에서 설정할 수 있으며, off/on 각각의 normal, highlighted, pressed preview, selected, disabled 상태 GameObject를 접을 수 있는 `StateObjects` 묶음으로 받아 직접 켜고 끕니다. 라디오별 아이콘과 라벨은 serialized `MyImage[]`, `MyText[]` 배열 참조로 연결하고, 코드는 setter-only `Sprite`와 `Title`로 연결된 이미지·텍스트 배열 전체를 갱신할 수 있습니다. 배열 값은 어떤 슬롯을 대표값으로 읽을지 안정적인 계약을 만들 수 없으므로 getter를 제공하지 않습니다. `MyRadio.InitializerInterface`는 단독 라디오에서만 적용되며, `MyRadioGroup`의 배열에 포함된 라디오 선택 초기화는 그룹이 맡습니다.

`MyRadioGroup.SelectionMode`는 항상 하나를 선택하는 `Required`, 선택 없음도 허용하는 `Optional`, 각 항목을 독립 토글처럼 다루는 `Multiple`을 제공합니다. `Index`는 현재 선택과 Inspector 초기 선택을 함께 맡으며, `Optional`의 `-1`은 선택 없음입니다. `Multiple`에서는 단일 초기 인덱스를 사용하지 않고 첫 번째 on 라디오의 index를 표시합니다. 그룹의 라디오 목록은 Inspector의 배열로 직접 받으며, 배열 순서가 선택 index 순서입니다. 상위에 라디오 그룹이 있어도 배열에 포함되지 않은 `MyRadio`는 단독 토글처럼 동작합니다. Inspector 변경은 `OnValidate()`로 표시를 맞추고, `OnEnable()`과 `Start()`의 초기화 경로는 플레이 모드에서만 실행합니다. 자식 자동 수집은 그룹의 기본 동작이 아니며, 필요하면 별도 보조 컴포넌트나 Editor 도구에서 제공하는 영역으로 둡니다.

## 탭

`MyTab`은 같은 GameObject의 `MyRadioGroup`과 `MySelector`를 연결해 탭 선택 상태를 갱신합니다. 헤더 `MyRadio`는 `MyRadioGroup`의 라디오 배열에 본문 `MySelector` 값 배열과 같은 인덱스 순서로 연결합니다. 탭은 항상 하나가 선택되는 `Required` 라디오 그룹으로 동작하므로 현재 선택 index는 `MyRadioGroup.Index`가 기준입니다. 코드에서 초기 탭을 정해야 할 때는 같은 GameObject의 컴포넌트가 `MyTab.InitializerInterface.InitialIndex`를 구현하며, 탭이 라디오 그룹 초기화 인터페이스를 내부 연결로 숨깁니다. 탭 진입/이탈 콜백은 선택 사항이며 구현체가 없어도 별도 경고를 출력하지 않습니다. `Required` 라디오 헤더 설정 경고는 플레이 모드에서만 출력합니다. 본문 페이지 동기화는 라디오 그룹 선택 콜백에서 시작됩니다.

`OuiMoveNext()`와 `OuiMovePrevious()`는 현재 선택된 탭 기준으로 다음 또는 이전 interactable 탭을 찾습니다. `allowWrapAround`를 `true`로 넘기면 끝에서 반대편 탭으로 순환합니다.

## 리스트

`MyList`는 값 컬렉션과 프리팹 엔트리를 동기화합니다. 엔트리 컴포넌트는 `MyListEntry<TValue>`를 구현하고, 리스트 소유자는 `MyList.Master<TEntry, TValue>`를 구현합니다.

정렬이 필요하면 `MyList.SorterInterface<TValue>`를, 엔트리 추가 후 처리가 필요하면 `MyList.PostscriptInterface<TEntry, TValue>`를 함께 구현합니다.

## 입력과 모달

`MyInput`은 입력값 초기화, 값 변경, 제출 콜백을 분리해서 연결합니다.

`MyAsker`는 확인 모달과 예/아니오 모달의 열기, 닫기, 결과 콜백을 제공합니다. 추가 데이터는 `MyAsker.MyAskerArguments`로 전달합니다.
