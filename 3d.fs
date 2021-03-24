\ 3d drawing with fixed point arithmetic
\ 
0 warnings !

require sdl.fs
require objload.fs

variable vertices
variable faces
variable vcount
variable fcount

vocabulary f8
f8 definitions

\ fixed point format
1 15 lshift constant #fbits

\ fixed point 1.15 format
: i>fi   ( d -- f8 )     #fbits * ;
: fi>i   ( f8 -- d )     #fbits / ;
: fimul  ( n0 n1 - n2 )  * #fbits / ;
: fidiv  ( n0 n1 - n2 )  / #fbits * ;
: i.d>fi ( d d -- fi )   swap i>fi + ;
: f>fi   ( f -- fi )
  fdup f>d drop #fbits * 1.0e fmod #fbits 0 d>f f* f>d drop + ;
\ end fi

vocabulary vec
vec definitions
also f8

0 \ vector3
dup constant v.x 1 cells +
dup constant v.y 1 cells +
dup constant v.z 1 cells +
constant vector3

: v.x v.x + ;
: v.y v.y + ;
: v.z v.z + ;

: v.x@ v.x @ ;
: v.y@ v.y @ ;
: v.z@ v.z @ ;

create v0 vector3 allot
create v1 vector3 allot

: savev0 ( v0 -- ) v0 v.z ! v0 v.y ! v0 v.x ! ;
: savev1 ( v1 -- ) v1 v.z ! v1 v.y ! v1 v.x ! ;
: savev01 ( v0 v1 -- ) savev1 savev0 ;

: vadd ( v0 v1 -- v2 )
  savev01

  v0 v.x@ v1 v.x@ +
  v0 v.y@ v1 v.y@ +
  v0 v.z@ v1 v.z@ +
;

: vsub ( v0 v1 -- v2 )
  savev01

  v0 v.x@ v1 v.x@ -
  v0 v.y@ v1 v.y@ -
  v0 v.z@ v1 v.z@ -
;

: vmul ( v0 v1 -- v2 )
  savev01

  v0 v.x@ v1 v.x@ fimul
  v0 v.y@ v1 v.y@ fimul
  v0 v.z@ v1 v.z@ fimul
;

: vdiv ( v0 v1 -- v2 )
  savev01

  v0 v.x@ v1 v.x@ fidiv
  v0 v.y@ v1 v.y@ fidiv
  v0 v.z@ v1 v.z@ fidiv
;

: vlen ( v0 -- n )
  savev0

  v0 v.x@ dup fimul
  v0 v.y@ dup fimul
  v0 v.z@ dup fimul + +

  s>f fsqrt f>s
;

: vdot ( v0 v1 -- n )
  savev01

  v0 v.x@ v1 v.x@ fimul
  v0 v.y@ v1 v.y@ fimul
  v0 v.z@ v1 v.z@ fimul

  + +
;

: vcross ( v0 v1 -- v2 )
  savev01

  v0 v.y@ v1 v.z@ fimul
  v0 v.z@ v1 v.y@ fimul -

  v0 v.z@ v1 v.x@ fimul
  v0 v.x@ v1 v.z@ fimul -

  v0 v.x@ v1 v.y@ fimul
  v0 v.y@ v1 v.x@ fimul -
;

: v*     ( addr -- x y z ) dup @ swap dup 1 cells + @ swap 2 cells + @ ;
: i3>fi3 ( a b c - a b c ) i>fi rot i>fi rot i>fi rot ;
: fi3>i3 ( a b c - a b c ) fi>i rot fi>i rot fi>i rot ;

\ end vec

\ TODO: precalc this
: gen-trigtable
  360 0 do
    dup i 0 d>f pi f* 180e f/ execute f>fi ,
  loop drop
;

create sintable ' fsin gen-trigtable
create costable ' fcos gen-trigtable

#width 2 /  i>fi constant #width-half
#height 2 / i>fi constant #height-half
#width 2 /  i>fi constant #dist

: pixel-off? ( x y -- t )
  dup #height >= swap 0 < or swap
  dup #width >= swap 0 < or
  or
;

variable x0
variable y0
variable x1
variable y1
variable dx
variable dy
variable e
variable s
: linev
  x1 @ x0 @ - abs dx !
  y1 @ y0 @ - abs dy !
  dx @ 2 / e !
  x1 @ x0 @ > if 1 else -1 then s !
  begin
    y1 @ y0 @ >
  while
    x0 @ y0 @ 2dup pixel-off? if 2drop else put-pixel then
    e @ dx @ + e !
    e @ dy @ >= if
      e @ dy @ - e !
      x0 @ s @ + x0 !
    then
    y0 @ 1+ y0 !
  repeat
;

: lineh
  x1 @ x0 @ - abs dx !
  y1 @ y0 @ - abs dy !
  dy @ 2 / e !
  y1 @ y0 @ > if 1 else -1 then s !
  begin
    x1 @ x0 @ >
  while
    x0 @ y0 @ 2dup pixel-off? if 2drop else put-pixel then
    e @ dy @ + e !
    e @ dx @ >= if
      e @ dx @ - e !
      y0 @ s @ + y0 !
    then
    x0 @ 1+ x0 !
  repeat
;

: line ( x0 y0 x1 y1 -- )
  y1 ! x1 ! y0 ! x0 !

  x1 @ x0 @ - dx !
  y1 @ y0 @ - dy !

  dx @ abs dy @ abs > if \ horizontal
    dx @ 0 > if \ right
      lineh
    else \ left
      x0 @ x1 @ swap x1 ! x0 !
      y0 @ y1 @ swap y1 ! y0 !
      lineh
    then
  else \ vertical
    dy @ 0 > if \ down
      linev
    else \ up
      x0 @ x1 @ swap x1 ! x0 !
      y0 @ y1 @ swap y1 ! y0 !
      linev
    then
  then
;

  
create xs 1 ,
: rnd ( n -- u )
  xs @
  dup 7 lshift xor
  dup 9 rshift xor
  dup 8 lshift xor
  dup xs !
  abs swap mod
;

: >p  ( z p -- p1 )  #dist fimul swap fidiv ;

variable x
variable y
variable z
: 3d>2d ( x y z -- px py )
  z ! y ! x !

  z @ 0= if 0 0 rdrop then
  z @ x @ >p #width-half + fi>i
  z @ y @ >p #height-half + fi>i
;

create t0 0.0e f>fi , 0.0e f>fi , -2.0e f>fi , \ translate vector
create t1 1.0e f>fi , 1.0e f>fi , 1.0e f>fi , \ scale vector
variable angle 0 angle !

variable x
variable y
variable z
variable a
\ x = x cos β − y sin β
\ y = x sin β + y cos β
\ z = z
: zrot ( x y z a -- x y z )
  360 mod a ! z ! y ! x !

  costable a @ cells + @ x @ fimul
  sintable a @ cells + @ y @ fimul -

  sintable a @ cells + @ x @ fimul
  costable a @ cells + @ y @ fimul +

  z @
;

\ x = z sin β + x cos β
\ y = y
\ z = z cos β − x sin β
: yrot ( x y z a -- x y z )
  360 mod a ! z ! y ! x !

  sintable a @ cells + @ z @ fimul
  costable a @ cells + @ x @ fimul +

  y @

  costable a @ cells + @ z @ fimul
  sintable a @ cells + @ x @ fimul -
;

\ x = x
\ y = y cos β − z sin β
\ z = y sin β + z cos β
: xrot ( x y z a -- x y z )
  360 mod a ! z ! y ! x !

  x @

  costable a @ cells + @ y @ fimul
  sintable a @ cells + @ z @ fimul -

  sintable a @ cells + @ y @ fimul
  costable a @ cells + @ z @ fimul +
;


: face-vertex>2d ( i -- x y )
  cells faces @ + @ 1-         \ face index
  vector3 * vertices @ + v*    \ vertex position
  t1 v* vmul                 \ scale vector
  angle @ xrot
  angle @ yrot
  angle @ zrot
  t0 v* vadd                 \ translate vector
  3d>2d                      \ project to screen x0 y0
;

: draw-edge ( i j -- )
  face-vertex>2d rot
  face-vertex>2d line
;


: 3d
  255 255 255 set-color

  begin
    clear-screen

    fcount @ 0 do
      i     i 1 + draw-edge
      i 1 + i 2 + draw-edge
      i 2 + i 0 + draw-edge
    3 +loop

    1000 60 / sdl-delay
    angle @ 1+ angle !

    flip-screen

    sdl-event sdl-poll-event
    sdl-event c@ SDL_KEYDOWN =
  until

  sdl-quit
;

variable ox
variable oy
: line-test
  255 255 255 set-color

  #width 2/ ox !
  #height 2/ oy !
  
  ox @ oy @ ox @ 20 + oy @ 50 - line \ sector 0
  ox @ oy @ ox @ 50 + oy @ 20 + line \ sector 1
  ox @ oy @ ox @ 50 + oy @ 20 - line \ sector 2
  ox @ oy @ ox @ 20 + oy @ 50 + line \ sector 3
  ox @ oy @ ox @ 20 - oy @ 50 - line \ sector 4
  ox @ oy @ ox @ 50 - oy @ 20 + line \ sector 5
  ox @ oy @ ox @ 50 - oy @ 20 - line \ sector 6
  ox @ oy @ ox @ 20 - oy @ 50 + line \ sector 7

  flip-screen
  wait-key
;

s" cube.obj" load-obj fcount ! vcount ! faces ! vertices !
3d


bye
