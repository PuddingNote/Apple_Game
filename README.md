# Apple Game (사과게임)

<img src="https://github.com/user-attachments/assets/09287781-b86d-44c8-8ea6-2ac904f485ae" width="250"></img>
<img src="https://github.com/user-attachments/assets/20501fae-782f-426c-a269-b2ece03a1d26" width="250"></img>
<img src="https://github.com/user-attachments/assets/3d0a8e88-f1b7-4f4a-8f13-09f02585a182" width="250"></img>
</br>


## 1. 게임 메커니즘
  - 사과의 합이 10이 되도록 드래그하여 점수를 획득하는 게임
## 2. 주요 목표
  - 주어진 시간내에 가능한 한 많은 사과를 합쳐 높은 점수를 기록
## 3. 개발 환경
  - Unity, C#
</br>

## 개발 일지
[2024-06-19]
- Project Setting
- Add Apple Prefab
- Add Apple Object Pooling
</br>

[2024-06-26]
- Fix Apple Group
  - Grid Layout Group Error
    - 객체가 비활성화될 때 해당 위치가 비워지는 것이 아닌, 나머지 자식들이 자동으로 재정렬되어서 문제 발생. 비활성화대신 투명하게 제작.
- Add Drag System
  - Add Select System
    - 드래그 범위 내 사과 선택 처리
  - Add Hide System
    - 사과 이미지 투명화, 텍스트 비활성화 후 값 초기화
</br>

[2024-06-27]
- Fix Drag System
  - 드래그 범위 내 사과 선택 처리 수정
    - 사과 선택 효과 때문에 드래그 상시 업데이트로 변경
- Refact Select System
  - 드래그중에 범위 내에 선택된 사과의 색 변환 효과 구현
- Add Score System & UI
- Add Timer System & GaugeBar UI
  - Slider 사용
</br>

[2024-06-28]
- Add TitleScene
- Add GameOver UI
  - GameScene에 EndGroup 제작
- Change Apple Image
- 코드 정리
</br>

[2024-07-01]
- Add Modile Touch System
  - 모바일 터치 기능 추가 완료
- Build Test
  - 윈도우 빌드 테스트 완료
- 생각중인 추가 기능
  - 사과 사라질때 효과 (o)
  - 드래그한 사과의 개수만큼 추가 점수
  - 더이상 합칠 수 있는 사과가 없을때 번호 다시 부여
- 1차 제작 완료
</br>

[2024-07-18]
- 코드 정리
</br>

[2024-11-21]
- Add Dotween Asset
- Fix GameScene
  - 게임 오버시 EndGroup 화면이 위에서 아래로 떨어지는 애니메이션 추가 (Dotween)
- Add Apple Animation
  - 사과들을 없앨때 포물선을 그리며 떨어지는 애니메이션 추가 (Code)
- 코드 정리
</br>

[2024-11-22] - ver0.1.0
- Add Particle System Prefab
  - Add Apple Effect
    - 사과들을 없앨때 이펙트 추가 (Particle System)
- Fix Particle System
  - 조금 더 자연스럽게 수정
- Fix TitleScene
  - TitleScene UI 수정
- Fix GameScene
  - Reset 기능 추가
  - GameOver UI 수정
- 코드 정리
- Build Test
- 생각중인 추가 기능
  - 드래그한 사과의 개수만큼 추가 점수
  - 더이상 합칠 수 있는 사과가 없을때 번호 다시 부여
- 2차 제작 완료
</br>

[2025-02-14] - ver0.1.1
- 이미지 변경
- 뒤로가기 Panel 추가
- 유니티 버전 변경 (2021.3.16f1 -> 2022.3.15f1)
- Build Setting
  - 안드로이드 빌드로 변경 후 테스트 완료
</br>

[2025-02-19] - ver0.1.2
- 뒤로가기 Panel 수정
- 전체 코드 리팩토링
- 추가 점수 기능 구현
- 버그 수정
- Build Setting
  - 빌드 버전 변경
  - 안드로이드 빌드 버전 변경
</br>

[2025-02-21] - ver0.1.3
- 전체 코드 리팩토링
- UI 추가 및 적용
  - 타이머 숫자 UI 적용
- 폰트 추가 및 적용
- Build Setting
  - 빌드 버전 변경
  - 안드로이드 빌드 버전 변경
</br>

[2025-02-21] - ver0.1.4
- UI 추가 및 적용
  - 타이머 숫자 UI 적용
- 폰트 추가 및 적용
- 전체 코드 리팩토링
- Build Setting
  - 빌드 버전 변경
  - 안드로이드 빌드 버전 변경
</br>

[2025-02-22] - ver0.1.5
- 드래그 기능 강화 및 클릭 모드 추가
  - 드래그 판정 수정
  - 클릭 모드 추가
- 모드 저장 기능 추가
- 전체 코드 리팩토링
- Build Setting
  - 빌드 버전 변경
  - 안드로이드 빌드 버전 변경
</br>

[2025-02-22] - ver0.1.6
- 클릭모드 버그 수정
  - 클릭모드에서 사과 점수가 제대로 계산되지 않는 버그 수정
- Build Setting
  - 빌드 버전 변경
  - 안드로이드 빌드 버전 변경
</br>

[2025-03-11] - ver0.1.7
- 드래그모드 정리
- 최고점수기능 추가
- 코드 리팩토링
- Build Setting
  - 빌드 버전 변경
  - 안드로이드 빌드 버전 변경
</br>

[2025-03-12] - ver0.1.8
- 최고점수기능 버그 수정
  - 최고점수가 이상하게 집계되는 버그 수정
- Build Setting
  - 빌드 버전 변경
  - 안드로이드 빌드 버전 변경
</br>

[2025-03-13] - ver0.1.9
- 옵션, 뒤로가기 UI 수정
  - 사운드, 도움말, 크레딧
- 사운드 추가
  - BGM, SFX
- 코드 리팩토링
- Build Setting
  - 빌드 버전 변경
  - 안드로이드 빌드 버전 변경
</br>
