# UnityOui 작업 메모리

## 현재 요약

- 마지막 확인 2026-11-13T19:12:58+09:00 · 근거 MEMORY-CLEANUP-001. MyInput 쓰기 API는 `3d2528d`에 반영됐고 [#29](https://github.com/oojjrs/unity_oui/issues/29)는 닫혔다. Escape 취소 뒤 `IsTexting`의 실제 입력 검수는 [#17](https://github.com/oojjrs/unity_oui/issues/17)에 남아 있다.
- MyText의 Inspector 말줄임 옵션은 `6244527`에 반영됐다. 실제 Unity 표시 확인은 [#30](https://github.com/oojjrs/unity_oui/issues/30)에서 검수 중이다.
- 기존 Unity 검수 항목은 [#20 라디오 탐색](https://github.com/oojjrs/unity_oui/issues/20), [#21 포커스 자동 스크롤](https://github.com/oojjrs/unity_oui/issues/21), [#22 목록 업데이트 콜백](https://github.com/oojjrs/unity_oui/issues/22), [#26 슬라이더 화살표 드래그](https://github.com/oojjrs/unity_oui/issues/26)다.

## 입력·텍스트

- **MyInput** — 자체 `_isFocused`는 선택 상태를 추적한다. Escape 취소로 `wasCanceled`가 설정되면 `IsTexting`에서 제외하며, `InputField.isFocused`로 대체하는 안은 사용자가 철회했다. EndEdit는 기존 UnityEvent 연결에서 `wasCanceled`를 전달한다. 자동 이벤트 등록과 BeginInput/CancelInput 추가는 InputField와 책임이 겹쳐 철회됐다. `IsReadOnly`는 `InputField.readOnly`에 위임하고 `IsInteractable`은 CanvasGroup이 반영되는 기존 getter와 `InputField.interactable` setter를 사용한다. [선택 상태](codex://threads/01a0cd0c-ce48-7563-8cff-72ddf32cae55) · [EndEdit](codex://threads/01a0a2cc-4634-7641-ab27-315baccab268) · [쓰기 API](codex://threads/01a1cf84-cfe9-7792-ab84-c0ff0bf23515)
- **MyText** — 말줄임은 기본 꺼짐 Inspector 옵션이다. `Text`와 `EscapedText` 대입 시 현재 폭에서 계산하고, Auto Width보다 우선하며 Auto Height에는 최종 표시 문자열을 사용한다. 폭이나 옵션을 바꿔 다시 계산하려면 원문을 다시 대입한다. `Text.text` 직접 변경 감지와 사전 크기 계산 API는 이전 논의에서 제외됐다. [말줄임](codex://threads/01a1cf95-94c1-7be1-aa1b-6ceb79af048d) · [자동 크기 결정](codex://threads/01a013f8-ec92-7ac1-b355-65742a7e4048)
- **텍스트 참조와 이스케이프** — Inspector에서 공통 타입을 받기 위해 `MyTextBase`를 사용한다. TMP 패키지 의존성 없이 참조할 수 있어야 한다. `EscapedText`는 사용자 입력에 적용하며, 시스템이 의도한 rich text를 조합할 때는 필요한 부분에 `MyText.Escape`를 적용한다. Rich Text가 꺼진 상태에서 `EscapedText`를 쓰면 매번 경고한다. [공통 타입](codex://threads/01a0f19b-36e9-7c23-b888-6132210baee2) · [이스케이프 결정](codex://threads/01a09396-f6fd-74f2-ba4f-9ffbd6fe88ef)

## 목록·표시

- **MyReel** — 가변 높이 항목을 추가하는 시점에 실제 크기를 동기 확정하고 스크롤 위치를 계산한다. 초기 추정·보정과 ContentSizeFitter 중첩은 높이 확정을 지연시키므로 명시적 `ResolveSize`로 대체했다. 직계 자식 resolver의 경계를 부모 좌표로 합치고, resolver 없는 중간 객체는 자동 탐색하지 않는다. 일반 `MyList.Entry`에 높이 인터페이스를 강제하지 않으며, 스크롤바가 목록 폭을 바꿔 대량 재계산을 일으키지 않도록 한다. [초기 구현](codex://threads/01a012a0-3c1b-7c13-9f2a-897e61fd6b26) · [Entry 높이 정정](codex://threads/01a0142f-b93b-7001-bab3-fc9509f4b497) · [최종 크기 계약](codex://threads/01a01efd-56bb-7453-8073-3a7d04052f4d)
- **MyScrollRect** — 선택된 content 자손을 viewport 안에 두는 독립 컨트롤이다. 입력 장치 전환 시 강제로 맞출 수 있도록 `OuiFocus`를 제공한다. [결정](codex://threads/01a0d739-bb71-75f1-99dc-ad82d88733e2)
- **MySlider** — float 초기화 API를 보존하고 int 전용 초기화·콜백·텍스트 인터페이스를 제공한다. 화살표 이동 기본값은 정수 1, 실수 범위의 5%다. 자식 화살표 버튼이 부모 드래그로 처리되면 첫 클릭이 경계값으로 이동할 수 있다. [정수 API](codex://threads/01a129e6-3e57-7380-b9c4-568aa1cdbc3b) · [드래그 수정](codex://threads/01a147ef-fb29-77a3-ad6e-5e3709c6c2db)
- **MyBar** — Text와 DeltaImage는 선택 참조다. 일반 게이지는 FillAmount, Sliced 이미지 게이지는 Resize를 선택할 수 있다. 별도 마스크 구성이 필요한 RectMask2D 안은 제외했다. [결정](codex://threads/01a14c6e-ca28-7423-a364-8072cb88ab4a)
- **MyView·MyTooltip** — MyView의 `Prefab`은 설정 원본이고 `Instance`는 현재 생성물이다. 동일 프리팹 재생성은 `OuiRecreate`로 요청한다. MyTooltip은 대상 밖에 배치하고 raycast를 막지 않아야 포인터 이탈이 반복되지 않는다. 패키지에 Localization 의존성을 추가하지 않는다. [MyView](codex://threads/01a0e0e1-2758-7040-9f6d-c98ca341a699) · [MyTooltip](codex://threads/01a028f0-eb1d-7a01-b657-79206c1ea5cd)
- **MyImage** — 배율을 받는 `SetNativeSizeSprite`를 유지하고, OverrideSprite의 native-size 기능은 사용자 수정으로 제거됐다. [결정](codex://threads/019e8b8c-0f23-7630-a652-8fda9dbfdb7e)

## 선택·버튼

- **MyRadio·MyRadioGroup** — 독립 입력 처리와 상태별 오브젝트를 사용한다. 그룹은 명시적 배열을 받으며 멤버십은 배열 포함 여부로 결정한다. 배열 전체에 쓰는 속성에는 대표값 getter를 두지 않는다. 초기 선택 적용은 알림을 보내지 않아 네트워크 요청으로 이어지지 않는다. [라디오 설계](codex://threads/019ed57b-fa0f-7b41-82c3-4c7fd3b1ba46) · [명시 배열](codex://threads/019ee9db-ba61-7b20-9d60-a2394b7eb489) · [배열 쓰기](codex://threads/019eef1d-e643-7fe0-b278-47814dba4be8) · [초기 알림](codex://threads/01a00e0a-a5a4-76c1-b7c8-1da578d40045)
- **MyRadioNavigation·MyTab** — 라디오 탐색은 UGUI의 위치 기반 탐색을 따르며 그룹 사이의 Button도 후보에 포함된다. Previous 문제는 직전 칸 탐색 문제였고 끝 순환은 별도 동작이다. MyTab의 선택 상태는 MyRadioGroup이 소유하며 그룹 콜백으로 MySelector를 동기화한다. 외부 초기화는 MyTab Initializer를 사용한다. [탐색 정정](codex://threads/01a0d654-36f5-7002-949b-babc687623d7) · [탭 설계](codex://threads/019ee540-221d-7931-8e3a-50d436976b5f)
- **MyButton** — 즉시 Press와 0.2초 Hold는 별도 동작이다. Lock은 표시를 그대로 두고 행동 콜백만 막으며 비활성화 시 해제된다. 포인터 hover와 패드 focus에서 소리를 내되 같은 프레임의 중복 재생을 막는다. 비활성화 시 이미지를 숨기는 것이 기본이며, 이미지 유지 옵션은 현재 색상에 맞춰 동작해야 하므로 원래 색상을 고정 캐시하지 않는다. [Press/Hold](codex://threads/019fce8b-885c-7932-a389-2d261fb5989a) · [Lock](codex://threads/019f7598-bd09-73d3-91c9-282e46ea393e) · [사운드](codex://threads/019f27ad-5c6b-7da0-ae35-b05165d3399e) · [비활성 이미지](codex://threads/019f3c08-1c49-7231-ad66-283b5775c733)

## 기록 정리

- **MEMORY-CLEANUP-001** · 2026-11-13T19:12:58+09:00 · Codex/root, host local. 사용자는 다음 작업에 필요하지 않은 기록까지 보존하는 방식을 정정했다. Codex는 후속 작업에 영향을 주는 결정·철회 이유·미해결 검수만 주제별로 남겼다. 현재 상태는 Git과 연결 이슈에서 확인했다. 개별 발언 시각은 확인되지 않았다. 턴 `01a1d45e-e687-7281-a20a-2da14d00572e`, 사용자 항목 `01a1d463-87a6-7e10-a33c-ae70e24ee2b0`. [요청 스레드](codex://threads/01a1d45e-7588-75e1-aa55-82035af91d49)
- 이전 `CURRENT-*`, `MYTEXT-ELLIPSIS-*`, `HIST-*` 기록을 다시 확인할 때는 [정리 전 메모리](https://github.com/oojjrs/unity_oui/blob/6244527fdcbdb9b8e9ed2072e8f4560a728a71bc/CodexWorkMemory.md)를 참조한다. 당시 Codex가 보고한 작업·검증은 해당 대화 시점의 기록이다.
