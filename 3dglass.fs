require sdl.fs


: PLOT ;

VARIABLE TRIGTABLE
TRIGTABLE HERE - ALLOT
0 , 572 , 1144 , 1715 , 2286 , 2856 , 3425 , 3993 ,
4560 , 5126 , 5690 , 6252 , 6813 , 7371 , 7927 , 8481 ,
9032 , 9580 , 10126 , 10668 , 11207 , 11743 , 12275 , 12803 ,
13328 , 13848 , 14364 , 14876 , 15383 , 15886 , 16383 , 16876 ,
17364 , 17846 , 18323 , 18794 , 19260 , 19720 , 20173 , 20621 ,
21062 , 21497 , 21925 , 22347 , 22762 , 23170 , 23571 , 23964 ,
24351 , 24730 , 25101 , 25465 , 25821 , 26169 , 26509 , 26841 ,
27165 , 27481 , 27788 , 28087 , 28377 , 28659 , 28932 , 29196 ,
29451 , 29697 , 29934 , 30162 , 30381 , 30591 , 30791 , 30982 ,
31163 , 31335 , 31498 , 31650 , 31794 , 31927 , 32051 , 32165 ,
32269 , 32364 , 32448 , 32523 , 32587 , 32642 , 32687 , 32722 ,
32747 , 32762 , 32767 ,

32767 CONSTANT TMAX

: COSSIN ( angle ... cos\sin )
  DUP 360 > IF
    360 MOD
  ELSE
    DUP 0< IF
      360 MOD
      360 +
    THEN
  THEN

  2* DUP

  DUP 180 < IF
    NEGATE 180 + TRIGTABLE + @ SWAP
    TRIGTABLE + @
  ELSE
    DUP 360 < IF
       180 - TRIGTABLE + @ NEGATE SWAP
       NEGATE 360 + TRIGTABLE + @
    ELSE
      DUP 540 < IF
        NEGATE 540 + TRIGTABLE + @ NEGATE SWAP
        360 - TRIGTABLE + @ NEGATE
      ELSE
        540 - TRIGTABLE + @ SWAP
        NEGATE 720 + TRIGTABLE + @ NEGATE
      THEN
    THEN
  THEN
;

VARIABLE S

: SCALE3D ( n ... )
  S !
;

VARIABLE X1
VARIABLE X2
VARIABLE X3
VARIABLE Y1
VARIABLE Y2
VARIABLE Y3
VARIABLE Z1
VARIABLE Z2
VARIABLE Z3

: ROTATE3D ( x\y\z ... )
  COSSIN DUP Z2 ! NEGATE Z3 ! Z1 !
  COSSIN DUP Y2 ! NEGATE Y3 ! Y1 ! 
  COSSIN DUP X2 ! NEGATE X3 ! X1 ! 
;

VARIABLE X
VARIABLE Y

: TRANSFORM3D2D ( x\y\z ... x\y )
  >R Y ! DUP
  Z1 @ TMAX */ Y @ Z3 @ TMAX */ + SWAP
  Z2 @ TMAX */ Y @ Z1 @ TMAX */ + SWAP

  DUP
  Y1 @ TMAX */ R@ Y3 @ TMAX */ + X !
  Y2 @ TMAX */ R> Y1 @ TMAX */ + >R 

  DUP
  X1 @ TMAX */ R@ X3 @ TMAX */ + Y !
  X2 @ TMAX */ R> X1 @ TMAX */ + 1000 + >R

  X @ S @ R@ */ 640 +
  Y @ S @ R> */ 512 +
;

: MOVE3D ( x\y\z ... )
  TRANSFORM3D2D
  4 PLOT
;

: DRAW3D ( x\y\z ... )
  TRANSFORM3D2D
  5 PLOT
;

VARIABLE COS
VARIABLE SIN

: DRAW-GLASS

  500 -400 DO
    -400 I -400 MOVE3D
    -400 I 400 DRAW3D
  100 +LOOP

  500 -400 DO
    -400 -400 I MOVE3D
    -400 400 I DRAW3D
  100 +LOOP


  361 0 DO
    I COSSIN SIN ! COS !
    -400 200 COS @ TMAX */ 200 SIN @ TMAX */ MOVE3D
    -375 175 COS @ TMAX */ 175 SIN @ TMAX */ DRAW3D
    -350 30 COS @ TMAX */ 30 SIN @ TMAX */ DRAW3D
    -100 15 COS @ TMAX */ 15 SIN @ TMAX */ DRAW3D
    -50 100 COS @ TMAX */ 100 SIN @ TMAX */ DRAW3D
    0 150 COS @ TMAX */ 150 SIN @ TMAX */ DRAW3D
    50 175 COS @ TMAX */ 175 SIN @ TMAX */ DRAW3D
    150 160 COS @ TMAX */ 160 SIN @ TMAX */ DRAW3D
  18 +LOOP

  -400 200 0 MOVE3D
  
  361 0 DO
    -400 I COSSIN 200 TMAX */ SWAP 200 TMAX */ SWAP DRAW3D
  18 +LOOP

  150 160 0 MOVE3D
  
  361 0 DO
    150 I COSSIN 160 TMAX */ SWAP 160 TMAX */ SWAP DRAW3D
  18 +LOOP


  -100 0 0 MOVE3D
  500 250 0 DRAW3D

  361 0 DO
    500 250 0 MOVE3D
    300 I COSSIN 200 TMAX */ SWAP 200 TMAX */ 200 + SWAP DRAW3D
  24 +LOOP
;

700 SCALE3D
0 0 90 ROTATE3D
