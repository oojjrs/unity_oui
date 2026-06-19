# Changelog

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
