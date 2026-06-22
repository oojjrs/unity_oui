# OOJJRS' Unity UI Helper

`com.oojjrs.oui`는 Unity UGUI 기반 프로젝트에서 반복해서 쓰는 UI 컴포넌트 동작을 모아 둔 헬퍼 패키지입니다.

## 설치

Unity Package Manager의 Git URL로 설치합니다.

```text
https://github.com/oojjrs/unity_oui.git?path=/Packages/src
```

이 저장소를 직접 열어 개발할 때는 `Packages/manifest.json`에 로컬 패키지로 연결되어 있습니다.

```json
"com.oojjrs.oui": "file:src"
```

## 포함 기능

- `MyButton`: 클릭, 호버, 프레스, 더블 클릭 콜백과 버튼 사운드 재생을 다루며 이미지 갱신 표면은 `MyImage` 기준으로 맞춥니다.
- `MyRadio`, `MyRadioGroup`: Unity `Toggle`, `ToggleGroup`, `Selectable`에 기대지 않고 상태별 GameObject 표시와 명시 배열 기반 라디오 선택 UI를 구성하며, 라디오별 이미지와 텍스트는 serialized 배열 참조와 setter-only `Sprite`/`Title` 표면으로 연결합니다. Inspector 미리보기와 플레이 모드 초기화 경계를 분리합니다.
- `MyText`, `MyImage`, `MyPortrait`: UGUI 텍스트와 이미지 값을 코드에서 간단히 갱신하고, 필요하면 이미지의 native size를 배율까지 지정해 함께 맞춥니다.
- `MyList`: 프리팹 기반 리스트 엔트리 생성, 제거, 정렬을 관리합니다.
- `MyBar`, `MySlider`, `MyToggle`, `MySelector`, `MySwapper`, `MyTab`: 값 기반 UI 상태와 라디오 헤더 기반 탭 선택/런타임 초기화 상태를 갱신합니다.
- `MyInput`: 입력 초기화, 선택 상태, 제출, 값 변경 콜백을 제공합니다.
- `MyAsker`: 확인 및 예/아니오 형태의 간단한 모달 흐름을 구성합니다.

런타임 헬퍼 컴포넌트는 같은 GameObject에 같은 타입을 중복으로 붙이지 않는 것을 기본 규칙으로 삼고, 이를 `DisallowMultipleComponent`로 표시합니다.

## 패키지 구조

- `Packages/src/package.json`: UPM 패키지 메타데이터입니다.
- `Packages/src/Runtime`: 런타임 컴포넌트와 asmdef가 들어 있습니다.
- `Packages/src/Documentation~`: Unity Package Manager에서 열 수 있는 패키지 문서입니다.
