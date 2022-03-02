# MaWang
2021 하반기 EDGE 게임개발대회

---20220303 / 05:50 ---
1. Pause > SettingPanel 구현 (normal, start, elite)
2. 전체 볼륨 조정을 위한 AudioManager, SoundEffecter, SFXButton 대폭 수정 (임시/임의로 변경한 점이 많으니 추후 이의나 수정 환영)
3. 볼륨 슬라이더 전용 VolumeSlider스크립트 추가
4. SelectableButtonsActive 관련 버그 해결
5. 초반 스토리 스킵버튼 적용
6. 다이얼로그 중 pause 후 resume 시 time이 흐르던 버그 해결

--20220303 / 02:27---
//22_03_02  로 찾아보시면 빠르게 변경사항을 확인할 수 있습니다.

---주요사항---
1. 배경음악이 흘러나옵니다
>>> 배경음악은 AudioManager를 하이어키에 넣음으로써 동작합니다. AudioManager Script가 붙어있어서, 그걸로 조절이 가능합니다.
2. 일부 부분들을 제외하고 효과음이 들어갔습니다.
>>> 효과음은 static class SoundEffecter로 사용이 가능합니다.

---수정된 버그---
1. Normal의 Enemy2 수정 - Phantom의 위치와 특정 map이 호환되지 않던 문제를 수정
2. Elite에서 유물만 보상으로 주어질 때 clearpanel이 정상적으로 작동하지 않던 오류를 수정


---발견한 버그---
1. 전 버전에서는 문제 없었던 것으로 확인 했었는데, 게임 클리어가 되어도 map이 게임 클리어가 되지 않은 상태로 인식합니다.
UIManger 내의 displayClearAward에서 SelectableButtonsActive를 했을 때, BattleFixed branch에서는 클리어가 되었었는데 지금은 안되는 것으로 보입니다.
뭐가 문제인지를 잘 모르겠습니다...



---코드 변경---
1. Player 코드 변경 - 효과음 부분
2. 모든 Artifact들의 코드 변경 - 효과음 부분
3. SpawnManager 코드 변경 - 효과음 부분
4. 모든 AI들의 코드 변경 - 효과음 부분
5. SFX Related 폴더 추가. 내부에 코드들 추가
6. AudioManager 추가. Scene에 넣어서 배경음악을 조정합니다.
7. UIManager 변경 - 효과음과 음악 관련 부분 변경입니다.
8. ClearPanel 변경 - 효과음과 음악관련, 그리고 능력치 상승시 얼마나 상승하는지 보이도록 수정.

---수정---
1. 각 scene에 audioManager추가, uiManager에게 audioManager 부착

---추가---
1. 각종 음악과 효과음 추가


###########################################################################


--20220302 / 15:03---
1. SwitchScene 코드 변경 - 버그 수정
2. EliteGolem AI 생성. Elite 방에서 Map 오류가 계속떠서 테스트 횟수 적음


----20220301 / 23:13----
//22_03_01  로 찾아보시면 빠르게 변경사항을 확인할 수 있습니다.

---주요사항---
1. 적을 모두 물리쳤을 때, 결산 화면이 뜹니다. Normal과 Elite에서 동작하나, Elite에서는 실험횟수가 모자랍니다.
결산화면에서는 움직일 수 없으며, ESC로 메뉴를 띄우는 것만 가능합니다.
2. 결산화면에서 나가게 될 경우, 이어하기를 누르면 결산화면으로 다시 돌아옵니다.
3. 게임 오버 화면이 추가되었습니다. 주인공이 죽게 되면 gameOver화면이 뜨며, gameover화면에서는 타이틀로 돌아가기와 quit만 작동됩니다.
4. Player의 공격 effect가 가끔 적의 아래에서 발동되는 버그를 수정하였습니다.
5. 결산화면에서 유물 보상을 얻을 경우, 유물 보상의 위에 커서를 대면 설명이 나옵니다.

---수정---
1. Canvas 내부에 새로운 UI를 추가하였습니다. ClearAward, Description 그리고 gameover입니다.


---코드 변경---
1. TileManager 코드 변경 => 적 섬멸 완료를 체크하는 코드를 추가
2. LevelManager 코드 변경 => level clear나 game over시 발동하는 코드를 추가
3. UIManger 코드 변경 => gameover와 level clear시 ui를 위한 코드를 대거 추가 / 변경
4. CameraFollow 코드 변경 => gameover시의 처리와 관련된 코드를 변경
5. Player 코드 변경 => gameover시의 처리와 관련된 코드를 추가 / 능력치 관련 코드 추가
6. SaveManager 코드 변경 => level clear시와 관련된 코드를 추가 / 변경
7. SwitchScene 코드 변경 => 새로운 함수를 두개 추가 / map button을 이용할 때 changeScene을 써서 저장이 제대로 되지 않던 오류 수정
8. SaveBase 코드 변경 => stageFlag를 추가

---추가---
1. ImageAnimation 코드를 UI Related 폴더에 추가
2. EnemyVariation에 Enemy99를 각각 추가. 이는 클리어하고 저장하고 껐다 켰을 때 player만 나오도록 하기 위함.
3. ScriptableArtifact 스크립트 추가
4. SceneManagers에 유물 백과사전 역할을 하는 ArtifactDictionary를 추가. NULL인 0번을 제외하고 List의 n번째 유물을 불러올 수 있게 해준다.
5. ClearPanel 스크립트 추가


##############################################################################





----20220226 / 22:56----
1. 새로운 몬스터인 Phantom을 추가하였습니다.
2. Phantom의 AI를 추가하였습니다.
==> Phantom은 골렘보다 훨씬 멀리서부터 플레이어를 확인하고, 느리지만 강력한 광범위 원거리 공격을 시도합니다.
==> Phantom은 플레이어가 너무 가까이 다가온다면 도망을 가려고 시도할 것입니다. 그러나, 마냥 도망치지는 않습니다. 너무 가까이 붙으면 궁지에 몰린 쥐처럼 공격을 시도합니다.




---20220226 변경점---

---주요사항---
1. 몬스터 및 플레이어가 움직이는 방향에 따라서 sprite가 flip됩니다.
2. 몬스터가 플레이어를 향해 걸어오는 AI가 Normal map에서 적용됩니다.
3. 몬스터가 플레이어를 향해 공격을 해옴 - Warning Sign은 아직 미적용상태입니다.

---수정---
1. Artifacts/ArtifactImage 내부의 유물들의 이미지를 모두 변경

2. 필요없는 sprite제거
3. Elite몹의 sprite변경
4. 장애물의 sprite변경

5. NotUsed 폴더 삭제
6. ScriptableFloatVariable 코드 삭제 - ScriptableLocation으로 변경


---코드 변경---
1. Player 코드 변경
2. Characterbase 코드 변경
3. TileManager 코드 변경
4. SpawnManager 코드 변경
5. DirectionChange 코드 변경 - static function 몇 개 더 추가
6. TestArtifact 코드 변경 - 아령에 맞추어 처리
7. Artifact__Potion 코드 변경 - bug fix
8. MonsterController 코드 변경


---추가---
1. 새로운 유물 - 아령이 추가되었습니다. 공격시 맨손의 공격이 나가지만, 획득시 힘+2, 잃어버리면 다시 힘-2인 유물입니다.



###########################################################################################################################################################



---20220224 변경---

---주요사항---
1.Title 씬에서의 UI를 손봤습니다.
2.Title씬 관련 스크립트 파일이 일부 수정되었습니다.

---수정---
1.BlockButton의 이름, 위치 변경 -> UI Related 의 StartLoadGameButton으로
2.StartStoryUI 스크립트를 생성했습니다. (Title씬의 Canvas에 속함)
3.Title 씬에 초기 스토리 설명 기능을 추가했습니다.


---20220220 변경점---

---주요사항---
1. Map관련 UI들을 통해 다음 맵을 선택할 수 있습니다
2. 더불어 저장 관련 코드의 흐름이 변경되었습니다.



---수정---
1. MapUIBtn 스크립트 파일이 추가되었습니다.
2. 클래스 playerSave~를 그냥 Save~로 바꿨습니다.
3. SaveBase에 curRoomRow 추가했습니다.
4. SaveBase의 curRoomNumber, prevRoomNumber의 기본값을 -1로 설정했습니다. (killPlayer)
5. MapUI가 SaveManager의 saving을 접근합니다.
6. SaveManager의 saveRoomEnd, saveRoomClear, saveStageClear를 주석처리
7. curRoomNumber의 수정을 MapUI가 합니다.
8. SaveManager의 savePlayer의 인수를 추가.(bool player, bool map) : 즉 savePlayer가 map도 저장
9. SwitchScene의 호출하는 메소드 변경
10. 또 뭐있지 (직접 질문 환영)

---코드 변경---
1. MapUI
2. MapUIBtn 
3. UIManager 
4. SwitchScene 
5. SaveManager 
6. SaveBase 변경


---20220214 변경점---

---주요사항---
1. 이제 유물이 보이게 되었습니다.
2. 여태까지 만든 map들을 변경하였습니다.


---수정---
1. Tilemap의 팔레트, 현재까지 만든 map, 모든 sprite를 교체
2. Slime을 Monster_Stone으로 변경
3. 변경사항에 맞추어 Tile Data들을 모두 변경
4. Canvas에서 Img_Art1, 2, 3를 추가 - 유물을 위한 image
5. Canvas의 img_item_sel의 hierarchy를 변경
6. 모든 scene에 유물을 test할 수 있는 버튼을 추가



---코드 변경---
1. TileData 코드를 변경
2. UIManager 코드를 변경
3. Player 코드를 변경



---추가---
1. Resources/Artifacts/ArtifactImage 폴더 추가
- 임시 sprite를 추가해놓음.
2. Script에 유물 테스트와 관련된 TestArtifact 코드를 추가








---20220213 변경점---


---주요사항---
1. 공격시 effect 적용
2. 4개의 유물을 추가 - 맨손, 그냥 포션, 사면검, 장창. === 맨손은 맨손, 포션은 체력회복, 사면검은 전범위 공격, 장창은 2칸앞까지 공격
3. 유물을 띄우는 것은 추후 업데이트 할 예정



---수정---
1. Input Action에서 마우스 클릭을 제거했습니다... 테스트 할 때 좀 피곤해져서....
(물약을 테스트할 때 클릭 잘못해서 사용되는 등의 문제)
2. Sprite 내부에 남아있던 animation파일들 삭제하고 필요한 sprite 이동
3. UI에다가 TransitionScreen를 prefab화. 이에 맞추어 현재까지 구현한 모든 Scene에서 transitionScreen을 수정
>>>>>> 무슨 버그인지는 모르겠으나, prefab화 이후에 UI에서 animation 구동이 전혀 안되게 되었음....
>>>>>> Prefab이 아니도록 해도, animation을 새로 만들어도 도저히 수정을 할 수가 없어서 일단은 정지해놓은 상황....
4. Animation폴더에 이펙트 3개를 추가
5. Resources폴더에 Effect폴더를 추가 - effect prefab 2개 추가



---코드 변경---
>>> 변경사항은 2022_02_13으로 찾아보시면 빠르게 확인하실 수 있습니다.
1. Player의 코드를 변경
2. Artifact의 코드를 변경
3. PlayerSaveManager의 코드를 변경
4. UIManager의 코드를 변경
5. SpawnManager의 코드를 변경
6. TileManager의 코드를 변경
7. CharacterBase의 코드를 변경


8. PlayerSaveManager의 코드를 변경
9. BlockButton을 더 직관적이게 변경
