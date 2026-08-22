# Changelog

## 1.12.5

- `MyButton`, `MyRadio`, `MySelector`가 hover 진입 상태를 추적해 pointer 이탈, interactable 해제 또는 비활성화 시 `OnHoverExit()`을 한 번만 전달하도록 보강했습니다.

## 1.12.4

- `MySelector.HoverInterface`를 추가해 같은 GameObject의 구현체에 pointer 진입과 이탈을 전달합니다.

## 1.12.3

- `MyTooltip.OpenAutoWidth()` 오버로드를 추가해 현재 또는 전달한 문구의 preferred width와 padding으로 폭을 자동 계산하고, 지정한 최대 폭과 Canvas 폭 안에서 줄바꿈 높이를 다시 맞춥니다.

## 1.12.2

- 화면에 직접 부착되는 UI 컨트롤 14종에 `RectTransform` 요구 조건을 추가하고, EventSystem 보조나 범용 상태 조정 컴포넌트는 기존 부착 범위를 유지합니다.

## 1.12.1

- 외부에서 설정한 현재 `MyText` 내용을 그대로 측정해 툴팁을 여는 `MyTooltip.Open(RectTransform target, float width)` 오버로드를 추가합니다.

## 1.12.0

- `MyTooltip`을 추가해 대상 `RectTransform`, 문구와 폭을 받아 높이를 자동으로 계산하고, 같은 루트 Canvas 안에서 대상 위나 아래에 배치합니다.
- 툴팁 전체의 raycast를 통과시켜 표시 직후 대상의 pointer hover가 끊기지 않도록 합니다.

## 1.11.10

- `MyText`가 자동 너비를 RectTransform에 먼저 적용한 뒤 픽셀 보정된 실제 너비를 기준으로 preferred height를 계산하도록 수정해 텍스트가 영역 밖으로 잘리는 문제를 방지합니다.
- 현재 사용하지 않는 `CalculatePreferredSize()`와 `ResizeToPreferredSize()` 설명을 현재 사용법 문서에서 제거합니다.

## 1.11.9

- 측정 엔트리와 표시 엔트리의 초기 RectTransform 조건을 일치시키고, 항목별 캐시 높이와 표시 인스턴스 높이 차이로 스크롤 중 예외가 발생하지 않도록 수정합니다.

## 1.11.8

- 재귀적으로 확정한 엔트리 높이로 누적 위치와 content 높이를 계산하고 현재 스크롤 위치의 표시 범위를 갱신합니다.
- 화면 밖 엔트리를 풀에 반환해 viewport와 overscan 범위에서 롤링 재사용하며 하단 스크롤 동작을 복구합니다.

## 1.11.7

- `MyReel`을 정렬 없는 `MyList`형 데이터 동기화 구조로 단순화합니다.
- 직계 자식 `SizeResolverInterface`를 가장 깊은 곳부터 재귀적으로 확정하고, container가 자식 bounds를 모두 포함하도록 자기 `RectTransform` 크기를 조정합니다.

## 1.11.6

- `MyListEntry<TValue>`와 `MyReel.SizeResolverInterface`를 상속한 `MyReel.EntryInterface<TValue>` 계약을 추가합니다. 기존 `MyReel` 엔트리는 새 인터페이스와 동기 `ResolveSize()`를 구현해야 합니다.
- 숨은 측정 엔트리 하나를 새 값마다 재사용해 `Value`, `PostscriptInterface.OnAdded()`, `ResolveSize()` 호출 후 루트 `RectTransform.rect.height`를 캐시하고, 모든 높이가 확정된 뒤 스크롤 위치와 표시 범위를 계산하도록 변경합니다. 기존 `PostscriptInterface.OnShown()`은 `OnAdded()`로 대체됩니다.
- `MyReel`의 고정 폭 및 top-anchored `ScrollRect.content` 계약을 추가하고 viewport 폭을 바꾸는 `AutoHideAndExpandViewport` 세로 스크롤바 설정을 거부합니다.

## 1.11.4

- `MyText.CalculatePreferredSize()`를 추가해 RectTransform에 적용하기 전에 자동 너비·높이 옵션을 반영한 목표 크기를 조회할 수 있습니다.
- 너비와 높이를 함께 자동 조정할 때 계산한 너비를 기준으로 높이를 산출해 사전 계산값과 실제 적용값을 일치시킵니다.

## 1.11.3

- `MyReel`이 Unity 레이아웃 시스템에 높이 계산을 위임하지 않고, 엔트리의 `Value` setter가 동기적으로 설정한 `RectTransform.sizeDelta.y`를 사용하도록 변경합니다.
- `MyReel`의 가상화, 엔트리 높이 계약과 하단 스크롤 동작을 패키지 문서와 README에 추가합니다.

## 1.11.2

- `MyText`에 선택적인 자동 너비·높이 조정을 추가해 `Text` 프로퍼티로 문자열을 설정할 때 UGUI Text의 preferred size에 맞춰 RectTransform 크기를 갱신할 수 있습니다.
- `ResizeToPreferredSize()`를 추가해 UGUI Text를 직접 변경한 뒤에도 설정된 자동 조정 축을 수동으로 다시 계산할 수 있습니다.

## 1.11.1

- `MyReel` 엔트리 루트의 `ContentSizeFitter`가 실제 높이를 결정하도록 변경하고, 첫 엔트리의 측정값으로 미측정 항목 높이를 추정해 초기 대량 생성을 방지합니다.
- `ScrollEnum`과 `OuiScrollToBottom()`을 추가하고, viewport 크기 변경 시 폭에 따른 줄바꿈과 높이를 자동으로 다시 측정합니다.

## 1.11.0

- `MyReel`을 추가해 전체 데이터의 표시용 사본은 유지하면서 viewport 주변의 가변 높이 엔트리만 생성하고 재사용합니다.
- 엔트리가 처음 표시될 때 Unity 레이아웃의 preferred height를 측정해 위치와 전체 콘텐츠 높이를 보정하며, 부모 `ScrollRect`가 있으면 스크롤과 viewport를 자동으로 연동합니다.

## 1.10.2

- `MyList`에 선택적인 `_emptyText`를 추가해 `UpdateEntries()` 후 관리 중인 엔트리가 없으면 빈 상태 문구를 표시하고, 엔트리가 있으면 숨깁니다.
- `_emptyText`가 비어 있으면 기존 목록 동작만 유지하며, 이번 입력이 아니라 동기화 후 실제 엔트리 수를 기준으로 표시합니다.

## 1.10.1

- `MySlider`에 선택적인 `_clickAudioSource` 필드를 추가해 좌우 버튼 콜백과 Thumb 포인터 입력마다 클릭음을 재생합니다.
- 슬라이더 값이 최솟값이나 최댓값 경계에 있어 이동하지 않더라도 클릭음은 재생하며, AudioSource가 비어 있으면 기존 이동 동작만 유지합니다.
- Thumb은 왼쪽 포인터를 누를 때 한 번만 재생하고, 트랙 클릭이나 드래그 중에는 반복 재생하지 않습니다.

## 1.10.0

- `MySlider.OnLeftButtonClick()`과 `OnRightButtonClick()`을 추가해 별도 좌우 `Button.onClick`에서 슬라이더 값을 이동할 수 있게 했습니다.
- `OnLeftButtonClick()`과 `OnRightButtonClick()`은 정수 슬라이더에서 5, 연속 슬라이더에서 전체 범위의 5%를 한 칸으로 사용하고 `RightToLeft` 방향을 반영합니다.
- 최솟값과 최댓값 경계에서는 값 변경 콜백을 다시 호출하지 않으며, `Value` 설정 시 `Slider.onValueChanged` 연결 여부와 관계없이 `MySlider` 콜백을 한 번만 전달하도록 정리했습니다.

## 1.9.0

- `MyButton.PressInterface`가 pointer down 즉시 `OnPressStarted()`를 호출하고, 누르는 동안 `OnPressing()`, pointer up에서 `OnPressEnded()`를 호출하도록 입력 계약을 바로잡았습니다.
- 기존 0.2초 지연 프레스 동작은 `HoldInterface`의 `OnHoldStarted()`, `OnHolding()`, `OnHoldEnded()`로 분리했습니다.
- 짧게 누르면 Hold 콜백을 호출하지 않고, Hold가 시작된 입력은 Hold Ended와 Press Ended 순서로 종료하도록 정리했습니다.
- Lock과 비활성화에서 시작된 콜백의 종료 짝을 한 번 보장하고, 오른쪽 pointer up과 선행 down 없는 pointer up을 무시하도록 보강했습니다.

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
