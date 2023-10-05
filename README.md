<h1 align="center">
마왕용사
</h1>

<h5 align="center">
어느날, 눈을 떠보니 마왕이었던 아빠가 죽어있어서 용사를 물리치러 갑니다!
</h5>
<p align="center">
    <img src="https://github.com/CBI777/MaWang/blob/Final/Readme_Img/image.png" width=700>
</p>

-------------------

## About this repository
This repository contains codes and game application of 마왕용사, a game created for EDGE Game Development Contest that was held in the second half of 2021.

This game was created by team of four university students, using Unity with C#.

마왕용사 is a roguelite game, featuring:
  - Battle using CTB system on 2D tile map
  - Stage transition using graph-node system.
--------------------
## Installation
> Download the zip file stored in repository and unzip it. Execute .exe file to start the game.
> > Note that this game only supports Korean for now.

--------------------
## Manual
<pre>
Press <b><ins>WASD</ins></b> to move around.    
</pre>

<pre>
Press <b><ins>1, 2, 3</ins></b> keys on top of qwerty to change the weapon,
Press <b><ins>E</ins></b> to attack.
</pre>

<pre>
Press <b><ins>ESC</ins></b> to open menu for config and map
</pre>

- All enemies shows attack range before executing attack. Use this as a chance to attack them.

-  Every attack and move has a cooldown. Use UIs at the bottom-left corner to see the cooldown.

-  Most of attacks are effected by the 'direction arrow', which is displayed under every characters. Consider direction of character when performing attack.

<p align="center">
    <img src="https://github.com/CBI777/MaWang/blob/Final/Readme_Img/image%20(1).png" width=700>
</p>

- Every weapons have different abilities, and can be changed easily with 1, 2, 3. Manipulate them well to fight off enemies.
- Take rewards after every battle and strengthen yourself to fight the boss.

<p align="center">
    <img src="https://github.com/CBI777/MaWang/blob/Final/Readme_Img/image%20(2).png" width=700>
</p>

--------------------
## Final Fixes
---수정된 버그---
1. Save하고 title로 돌아왔을때 새로하기나 이어하기가 제대로 되지 않던 문제를 수정
2. 각 방에 있었던 level확인과 디버그 버튼들 제거
3. 맵용 디버그 버튼들 제거
4. clearPanel에 맵 이동 버튼 추가<<<
5. Something 수정<<<

---발견한 버그---
---밸런스 패치---
1. 방들의 확률 변경 (상점-5%, 회복-5%)
2. Elite Map1 변경
3. Enemy0 변경
4. Enemy1 변경
5. 작은 골렘들 hp 버프 (검은색->15, 노랑->10)
6. 엘리트 골렘 버프 (체력->35, 근접공격 속도를 0.4초로 변경)
7. 플레이어의 hp보상을 버프 (체력증가량 [2 혹은 4] -> [3 혹은 6])
--------------------
