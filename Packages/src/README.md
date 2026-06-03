# OOJJRS' Unity UI Helper

UGUI 기반 UI에서 자주 반복되는 컴포넌트 제어를 `oojjrs.oui` 네임스페이스로 묶은 Unity 패키지입니다.

## 요구 사항

- Unity 6000.4 이상
- `com.unity.ugui` 2.0.0

## 주요 컴포넌트

- `MyButton`: 버튼 클릭, 호버, 프레스, 더블 클릭, 쿨다운, 애니메이션 트리거, 사운드 오버라이드를 처리합니다.
- `MyList`: 값 목록을 프리팹 엔트리로 동기화하고 필요하면 정렬합니다.
- `MyBar` / `MySlider` / `MyToggle`: 값 변경 UI와 표시 텍스트를 연결합니다.
- `MyText` / `MyImage` / `MyPortrait`: 기본 텍스트와 이미지 갱신을 감싸고, `MyImage.SetNativeSizeSprite`와 `MyImage.SetNativeSizeOverrideSprite`로 스프라이트 교체 후 native size를 배율까지 지정해 맞출 수 있습니다.
- `MyInput`: 입력값 초기화, 변경, 제출 콜백을 연결합니다.
- `MyAsker`: 확인 또는 예/아니오 모달을 열고 닫는 흐름을 제공합니다.

자세한 내용은 `Documentation~/index.md`를 참고합니다.
