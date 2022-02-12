# MaWang
2021 하반기 EDGE 게임개발대회

2022_02_12_AM 변경점

---변경---
1. Title Scene의 Canvas가 scale with screen size가 아니어서 transition image가 제대로 안나왔던 점 수정
2. Stage1_Normal, Stage1_Elite, Stage1_Start에다가 player에게 damage를 주는 버튼을 추가. 이 버튼은 player의 hp가 0이 되어도 게임오버를 하진 않음. 순수 테스트용
3. Player들에게 UIManager가 제대로 안 들어가있어서 inspector에서 넣어줌
4. Stage1_Normal, Stage1_Elite, Stage1_Start에 있던 다음 씬으로 넘어가는 버튼들이 수행하던 함수를 잠시 변경. 플레이어의 hp가 바뀌는 것을 확인하기 위함.



---제거---
1. Samples 폴더의 제거
2. MapSaveBase 제거 - 필요없다고 판단



---코드변경---
1. UIManager변경 - hp관련하여 함수를 하나 더 만듦
2. Characterbase 변경 - Player에서 override를 위하여 hpDamage와 hpHeal을 virtual로 변경하였음.
3. Player변경
	- Player내부에서 UIManager를 참조할 때 쓰는 것도 UIManager라서 혼동이 올 수 있다고 판단하였고, 실제로 컴파일러도 헷갈려함.
	  따라서, 소문자로 변경하였습니다.
	- override를 통하여 hp에 변동이 있을 때 이를 hp바에 적용시키는 코드를 override하였습니다.
	- test를 위한 testhpDamage를 넣었습니다. 이건 후에 지워야합니다.
4. SwitchScene 변경
	- 플레이어의 hp가 유지되는 것을 보기 위해서 임시적인 함수를 생성
5. PlayerSaveManager 변경
	- 변동사항은 2022_02_11로 찾아보시면 편합니다.




>>>>>>>>>>>>>
MapUI가 시작때에 disable된 상태에서 시작되면 바깥에서 map을 access할 때 어떻게 할지를 생각해야할 것 같습니다.
 └ MapUI 스크립트 자체는 UIManager가 가지고 있기 때문에 크게 상관 없을 것 같습니다.
