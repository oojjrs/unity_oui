# Changelog

## 1.7.5

- `MyTab`이 선택 진입/이탈 콜백 구현체가 없어도 별도 경고를 출력하지 않도록 정리했습니다.
- `MyTab`의 라디오 헤더 모드 경고 문구가 `MyRadioGroup.SelectionModeEnum.Required` 값을 직접 가리키도록 정리했습니다.

## 1.7.4

- `MyRadioGroup`의 라디오 목록을 자식 자동 수집 대신 Inspector의 직접 배열로 받도록 변경했습니다. 배열 순서가 선택 index 순서가 됩니다.
- `MyRadio`는 `MyRadioGroup`이 직접 등록한 경우에만 그룹 선택 정책에 위임하고, 등록된 그룹이 없거나 그룹이 해당 라디오를 처리하지 못하면 단독 토글처럼 동작합니다.
- `Assets/MyRadioTest.unity`와 `Assets/MyTabTest.unity`의 라디오 그룹에 명시적인 라디오 배열 참조를 추가했습니다.

## 1.7.3

- `MyTab`이 `MyTabHeaderButton` 대신 같은 GameObject의 `MyRadioGroup`과 `MySelector`를 연결하도록 변경했습니다.
- `MyTabHeaderButton`을 제거해 탭 헤더의 입력과 선택 표시를 `MyRadio`가 직접 담당하도록 정리했습니다.
- `MyTab`에 `ExecuteAlways`와 탭 전용 `InitializerInterface`를 추가하고, 현재 선택 index는 `MyRadioGroup`에 위임하도록 정리했습니다. 탭은 라디오 그룹 초기화 인터페이스를 내부 구현으로 숨기며, 본문 동기화는 라디오 그룹 콜백에서만 시작합니다.

## 1.7.2

- `MyRadioGroup`이 외부 `InitializerInterface` 값을 `Start()`에서 한 번만 읽고, 이미 선택된 index를 다시 선택해도 그룹 콜백을 호출하도록 정리했습니다.
- `MyRadioGroup`의 라디오 목록을 `Awake()`에서 한 번 결정하는 정적 목록으로 바꾸고, 외부에서 목록을 다시 수집하는 public API를 제거했습니다.
- 그룹 안의 `MyRadio`는 자신의 `InitializerInterface`를 적용하지 않고 `MyRadioGroup` 초기화 결과를 따르도록 정리했습니다.
- Inspector에 노출되는 `MyRadioGroup` 툴팁 문구를 한글로 정리했습니다.

## 1.7.1

- `MyRadio`의 상태 표시 GameObject를 `StateObjects` 접힘 묶음으로 정리해 Inspector 길이를 줄였습니다.
- `Assets/MyRadio.prefab` 예제의 상태 표시 참조를 새 `StateObjects` 구조로 옮기고, 테스트 씬 확인 결과를 반영했습니다.

## 1.7.0

- `MyRadio`와 `MyRadioGroup`을 추가해 Unity `Toggle`, `ToggleGroup`, `Selectable`에 기대지 않는 라디오/토글 선택 컨트롤을 제공합니다.
- `MyRadio`는 off/on 각각의 normal, highlighted, pressed preview, selected, disabled 표시 GameObject를 `StateObjects` 접힘 묶음으로 받아 직접 제어합니다.
- `MyRadioGroup.SelectionMode`는 `Required`, `Optional`, `Multiple` 정책을 제공하며, 에디터에서도 초기 선택과 자식 라디오 상태를 동기화합니다.
- `Assets/MyRadioTest.unity` 테스트 씬과 `Assets/MyRadio.prefab` 예제를 추가했습니다.

## 1.6.1

- 런타임 UI 헬퍼 컴포넌트에 `DisallowMultipleComponent`를 적용해 같은 GameObject에 같은 헬퍼를 중복 부착하지 않도록 했습니다.
- 문서에 컴포넌트 배치 규칙과 중복 부착 방지 기준을 추가했습니다.

## 1.6.0

- `MyTab`과 `MyTabHeaderButton`을 추가해 탭 헤더 클릭과 이전/다음 탭 이동을 `MySelector` 선택 상태와 연결할 수 있게 했습니다.
- 탭 이동 시 `allowWrapAround`를 지정하면 끝에서 반대편의 interactable 탭으로 순환할 수 있습니다.
- `MyTab`의 헤더 설정이 비어 있으면 경고를 출력하도록 했습니다.

## 1.5.4

- `overrideSprite`는 표시 이미지만 교체하는 용도에 가깝기 때문에 `SetNativeSizeOverrideSprite` 메서드를 제거했습니다.
- `MyImage` native size 문서를 `SetNativeSizeSprite` 기준으로 정리했습니다.

## 1.5.3

- `MyImage` native size API를 setter에서 `SetNativeSizeSprite`, `SetNativeSizeOverrideSprite` 메서드로 변경했습니다.
- native size 적용 후 `nativeSizeScale` 배율로 크기를 조정할 수 있게 했습니다.

## 1.5.2

- `MyImage`에 스프라이트 설정 후 native size를 함께 맞추는 `NativeSizeSprite`, `NativeSizeOverrideSprite` setter를 추가했습니다.
- 패키지 문서에 `MyImage` native size setter 사용 안내를 추가했습니다.

## 1.5.0

- 패키지 루트를 `Assets`에서 `Packages/src`로 정리했습니다.
- 런타임 스크립트와 asmdef를 `Packages/src/Runtime` 아래로 이동했습니다.
- Git 설치 URL을 `?path=/Packages/src`로 갱신했습니다.
- Unity 최소 버전을 6000.4로 올리고 UGUI 의존성을 명시했습니다.
- README와 Unity Package Manager 문서를 추가했습니다.
