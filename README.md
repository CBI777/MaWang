20220210 - 죄송합니다
어제 너무 피곤했는지 정말 어처구니 없는 실수를 했습니다. 정작 파일을....
오늘까지 작업한 것을 다시 올리겠습니다.

20220210 변경점
---추가---
1. Stage1_Normal과 Stage1_Elite의 resources 내부의 모든 character들에게 hud를 위한 canvas를 추가
2. player도 canvas로 방향을 알아야하기 때문에 canvas를 추가
3. sprite에 방향을 나타내기 위한 임시 sprite인 direction을 추가



---코드변경---
1. CaracterBase 변경
2. player 변경
3. 테스트 때문인지는 몰라도, player의 공격범위가 전후좌우가 아니라 z를 바꾸길래 수정했습니다.
4. TileManager update 다시 주석처리



////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////



20220209 변경점
제 실수로 인한 나비효과로 만들어진 버그의 원인을 찾아내느라 시간이 많이 지나서 올립니다. 죄송합니다.
---제거---
1. 기존에 썼었던 transition관련 애니메이션 - scene 내에 있었던 transition 제거
2. Scenes폴더 내부의 temporary 제거.
3. Scene Related 폴더 내부의 LevelLoader제거



---추가---
1. Scenes폴더는 필요 Scene들로 채웠음.
	1) Title은 종료/이어하기/새로하기를 넣고
	2) Start는 각 stage에서 출발을 하기 위한 공간으로 할 예정이다. 즉, Stage1_Start는 처음에 시작을 하는 마왕 아들의 방이다.
	3) 현재 구현상 Stage1_Normal, Stage1_Elite, Title, Stage1_Start 밖에는 작동을 하지 않는다. 다른 Scene은 전부 비어있음.

2. UIManager / InputManager을 prefab으로 빼둠.
3. Save Related 폴더 신설. 안에 Save관련 4개의 script 넣어둠. Map관련은 후에 상의가 필요
4. Scene Related 폴더 내부에 필요한 두 개의 script를 넣어둠. BlockButton은 임시로 만들어둔 것.
5. 필요한 tag를 추가하였음. 그리고 해당 tag들을 필요한 prefab에 넣어두었음.
6. Canvas에 Loading용 TransitionScreen을 추가.
7. Sprite에 Knight_temp와 그에 해당하는 Knight_Walk, Knight_Walk_Animation을 추가.
8. 7번에서 추가한 animation을 위해서 Animation 폴더를 세분화
9. 모든 Canvas에 TransitionScreen을 추가.
10. Stage1_Normal에 Stage1_Elite로 넘어가는 버튼을 추가(디버그용)



---변경---
1. Project Setting에서 script의 발동순서 변경 - 이는 Save/Load를 위하여 Awake와 Start의 타이밍을 맞춰주기 위한 조치다.
2. Stage1_Elite 변경
3. Build Setting에서 모든 Scene을 넣어줌



---코드 변경---
1. Save Related 폴더
 - PlayerSaveBase추가
 - PlayerSaveManager추가

2. Scene Related 폴더
 - SwitchScene추가
 - LevelManager추가

3. Character Related 폴더
 - Artifact추가 => 지금은 기능이 없는 그냥 기반용 코드

4. Player코드 수정
5. CharacterBase코드 수정
6. TileManager코드 수정
7. SpawnManager코드 수정
 - 특이사항 : 기존에 Stage1_Normal scene에서 아마 모든 object들이 제대로 tile안에 들어가지 않는 문제가 있었을 것 같은데,
   사실 이게 눈에 보이는 그래픽하고 내부에서 처리하는 좌표하고 서로 달라서 0.5f씩 더한 값을 실제 그림이 그려지는 좌표로 하고 처리를 하는 식으로 코드가 짜져있었는데, 
   제가 마지막으로 push했던 프로젝트에서는 왜그런지 모르겠는데 그 부분이 쏙 빠져있어서 생겼던 오류였습니다.
   이 문제를 고치시려고 Resources안에 있는 Normal의 Enemy 좌표들에 모두 0.5씩을 더해 놓으셨더군요.
   정말 죄송합니다.
8. MonsterController코드 수정
9. UIManager코드 수정
 - 특이사항 : 원래는 gold나 스탯도 변경을 할 수 있게 UIManager를 만들었으나, 그러한 부분은 괜히 건드리지 않고,
    테스트를 위해서
	1) Start부분에 간단하게 지금이 어디인지를 설명해줄 수 있도록 하는 부분을 넣어둠.
	2) 더 간편한 접근을 위해서 LevelManager를 인스펙터로 넣을 수 있도록 조정.
		==> 이 UIManager의 변경사항은 중요한게 아니니, 후에 다른 UI도 변경을 하는 코드를 짜실 때 아예 삭제하고 새롭게 짜셔도 무방.



---주의 사항---
1. Save코드 때문에 Title Scene에서 시작을 안하면 예기치 않은 문제가 발생할수도 있습니다
