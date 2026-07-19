# Changelog

## 1.8.1

- `OuiFree()` 또는 비활성화 전까지 유지되는 인자 없는 `MyButton.OuiLock()`을 추가했습니다.
- 인자 없는 Lock은 별도 감시 코루틴을 실행하지 않고 내부 singleton 조건으로 상태만 유지합니다.

## 1.8.0

- `MyButton.LockInterface`와 `OuiLock()` / `OuiFree()`를 추가해 `Button.interactable`과 분리된 행동 잠금을 제공했습니다.
- Lock 중에는 hover, focus, pressed 표현을 유지하면서 클릭, 더블 클릭, 프레스 콜백과 클릭 사운드를 차단합니다.
- `KeepWaiting` 조건이 끝나면 자동으로 해제하고, 수동 해제와 Lock 교체, 조건 구현체 파괴를 안전하게 처리합니다.
- 비활성화 시 Lock을 초기화하고, `MyToggle`의 자식 클릭 전달 경로도 MyButton Lock을 따르도록 했습니다.

## 1.7.17

- `MyCurrentGameObjectDetector`를 추가해 `EventSystem.currentSelectedGameObject` 변경 시 이전 선택과 현재 선택을 콜백으로 전달하도록 했습니다.
- 선택 객체 감지기 사용법을 README와 패키지 문서에 추가했습니다.

## 1.7.16

- `MyButton`, `MyRadio`, `MySelectable`이 시작 전에 이미 EventSystem의 현재 선택인 경우 시작 시 포커스 진입 콜백을 한 번 보충하도록 했습니다.
- 초기 선택 동기화와 실제 `OnSelect`가 겹쳐도 진입 콜백을 중복 호출하지 않고, 동기화 과정에서는 focus hover 사운드를 다시 재생하지 않습니다.

## 1.7.15

- `MyButton`에 serialized `Animator` 참조와 public get-only `Animator` 속성을 추가했습니다.
- 외부에서 읽는 `Animator` 참조와 기존 `OuiPlayAnimation` 계열의 동일 GameObject trigger 경로를 구분해 문서화했습니다.

## 1.7.14

- `MyButton`과 `MyRadio`에 동기식 포커스 진입·이탈 콜백을 제공하는 각 타입의 `FocusInterface`를 추가했습니다.
- 포커스 콜백을 기존 focus hover 사운드의 지연·중복 억제와 분리했습니다.
- `MySelectable`이 interactable 해제나 비활성화에서도 select/deselect 콜백의 짝을 한 번 유지하도록 보강했습니다.

## 1.7.13

- `MyButton`의 기본 disabled 이미지 동작은 기존처럼 숨김으로 유지했습니다.
- `_isImageVisibleWhenDisabled` 옵션을 켜면 disabled 상태에서도 이미지를 유지하고 normal/disable 색상으로 전환하도록 했습니다.
- `MyImage.Color` 래퍼를 추가해 버튼 이미지 색상 처리를 `MyText.Color`와 같은 방식으로 다루도록 했습니다.

## 1.7.12

- `MyButton`과 `MyRadio`의 pointer hover 사운드를 다시 재생하도록 복구했습니다.
- focus hover 사운드는 유지하되, click 또는 pointer hover와 같은 프레임에 겹치면 취소해 중복 재생되지 않도록 했습니다.
- 문서의 hover 사운드 설명을 pointer/focus 양쪽 입력 기준으로 바로잡았습니다.

## 1.7.11

- `MyButton`과 `MyRadio`의 hover 사운드를 focus 진입 기준으로 재생하도록 정리했습니다.
- focus와 click이 같은 프레임에 함께 일어나면 hover 사운드를 취소해 click 사운드와 겹치지 않도록 했습니다.
- `MyToggle`의 별도 hover 사운드 재생을 제거해 `MyButton`과 중복 재생되지 않게 했습니다.

## 1.7.10

- `MyRadio.Sprite`와 `MyRadio.Title`을 배열 참조 전체에 값을 적용하는 setter-only 접근 표면으로 정리했습니다.
- 배열 값은 대표 getter 계약을 만들 수 없으므로 읽기 API를 제공하지 않는다는 설계 의도를 문서화했습니다.

## 1.7.9

- `MyRadioGroup`의 `OnEnable()`과 `Start()` 초기화 경로가 플레이 모드에서만 실행되도록 정리했습니다.
- `MyTab`의 `Required` 라디오 헤더 설정 경고가 플레이 모드에서만 출력되도록 정리했습니다.

## 1.7.8

- `MyRadio`의 이미지와 텍스트 참조를 단일 `MyImage`, `MyText` 대신 `MyImage[]`, `MyText[]` 배열로 바로잡았습니다.
- `MyRadio`의 이미지와 텍스트 배열은 serialized 참조로 유지하고, `Sprite` 설정은 연결된 이미지 배열 전체에 적용되도록 했습니다.

## 1.7.7

- `MyButton`의 이미지 참조를 Unity `Image` 대신 `MyImage` 기준으로 맞추고, 기존 `Sprite` 접근은 `MyImage.Sprite` 경로를 사용하도록 정리했습니다.
- `MyRadio`에 선택지별 `MyImage`, `MyText` 참조를 추가하고, `Sprite`와 `Title` 접근 표면으로 라디오 아이콘과 라벨을 갱신할 수 있게 했습니다.

## 1.7.6

- `MyRadioGroup`의 직접 배열 방식에서 별도 라디오 등록 단계를 제거했습니다.
- `MyRadio`는 상위 그룹이 있어도 자신이 그룹 배열에 포함된 경우에만 그룹 선택 정책에 위임하고, 배열에 없으면 단독 토글처럼 동작합니다.
- 라디오 그룹 직접 배열 방식에 맞춰 패키지 문서와 설계 문서를 갱신했습니다.

## 1.7.5

- `MyTab`이 선택 진입/이탈 콜백 구현체가 없어도 별도 경고를 출력하지 않도록 정리했습니다.
- `MyTab`의 라디오 헤더 모드 경고 문구가 `MyRadioGroup.SelectionModeEnum.Required` 값을 직접 가리키도록 정리했습니다.

## 1.7.4

- `MyRadioGroup`의 라디오 목록을 자식 자동 수집 대신 Inspector의 직접 배열로 받도록 변경했습니다. 배열 순서가 선택 index 순서가 됩니다.
- `MyRadio`는 `MyRadioGroup` 배열에 포함된 경우에만 그룹 선택 정책에 위임하고, 포함된 그룹이 없거나 그룹이 해당 라디오를 처리하지 못하면 단독 토글처럼 동작합니다.
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
