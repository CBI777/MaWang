# MaWang
2021 하반기 EDGE 게임개발대회

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
