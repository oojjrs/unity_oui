# OOJJRS' Unity UI Helper

UGUI 기반 UI에서 자주 반복되는 컴포넌트 제어를 `oojjrs.oui` 네임스페이스로 묶은 Unity 패키지입니다.

## 요구 사항

- Unity 6000.4 이상
- `com.unity.ugui` 2.0.0

## 주요 컴포넌트

- `MyButton`: 버튼 클릭, pointer/focus 기반 hover 사운드, 호버, 프레스, 더블 클릭, 쿨다운, 애니메이션 트리거, 사운드 오버라이드를 처리하며 이미지 갱신 표면은 `MyImage` 기준으로 맞춥니다. 기본 disabled 이미지는 숨기고, 옵션을 켜면 이미지를 유지한 채 disabled 색상으로 표시합니다.
- `MyRadio`, `MyRadioGroup`: Unity `Toggle`, `ToggleGroup`, `Selectable`에 기대지 않고 상태별 GameObject 표시와 명시 배열 기반 라디오 선택 UI를 구성하며, 라디오별 이미지와 텍스트는 serialized 배열 참조와 setter-only `Sprite`/`Title` 표면으로 연결합니다. Inspector 미리보기와 플레이 모드 초기화 경계를 분리합니다.
- `MyList`: 값 목록을 프리팹 엔트리로 동기화하고 필요하면 정렬합니다.
- `MyBar` / `MySlider` / `MyToggle` / `MyTab`: 값 변경 UI, 표시 텍스트, 라디오 헤더 기반 탭 선택과 런타임 초기화 콜백을 연결합니다.
- `MyText` / `MyImage` / `MyPortrait`: 기본 텍스트와 이미지 갱신을 감싸고, `MyImage.SetNativeSizeSprite`로 스프라이트 교체 후 native size를 배율까지 지정해 맞출 수 있습니다.
- `MyInput`: 입력값 초기화, 변경, 제출 콜백을 연결합니다.
- `MyAsker`: 확인 또는 예/아니오 모달을 열고 닫는 흐름을 제공합니다.

## 컴포넌트 배치 규칙

런타임 UI 헬퍼 컴포넌트는 하나의 GameObject에 같은 헬퍼 타입을 하나만 두는 것을 기준으로 합니다. 같은 오브젝트의 필수 UGUI 컴포넌트가 필요한 타입은 `RequireComponent`로 의존성을 드러내고, 헬퍼 본체는 `DisallowMultipleComponent`로 중복 부착을 막습니다.

자세한 내용은 `Documentation~/index.md`를 참고합니다.
