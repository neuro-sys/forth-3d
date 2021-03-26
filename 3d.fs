\ 3d drawing with fixed point arithmetic
\ 

require sdl.fs
require objload.fs
require fi.fs
require zbuffer.fs
require line.fs
require trig.fs
require vec3.fs
require scanfill.fs

0 value positions       \ array of vertex positions
0 value normals         \ array of vertex normals
0 value faces           \ array of index-cells
0 value pcount          \ number of x, y, z positions
0 value ncount          \ number of x, y, z normals
0 value fcount          \ number of offset faces

false value wireframe?

0 value x0
0 value y0
0 value x1
0 value y1
0 value x2
0 value y2
: visible? ( x0 y0 x1 y1 x2 y2 -- t )
  to y2 to x2 to y1 to x1 to y0 to x0

  x1 x0 -
  y1 y0 + fimul

  x2 x1 -
  y2 y1 + fimul

  x0 x2 -
  y0 y2 + fimul

  + + 0 <=
;

#width 2 /  i>fi constant #width-half
#height 2 / i>fi constant #height-half
#width 2 /  i>fi constant #dist
  
: >p  ( z p -- p1 )  #dist fimul swap fidiv ;

0 value x
0 value y
0 value z
: >proj ( x0 y0 z0 -- x1 y1 z1 )
  to z to y to x

  z 0= if 0 0 rdrop then
  z x >p #width-half +
  z y >p #height-half +
  z
;

create t0 0.0e f>fi , 0.0e f>fi , -3.5e f>fi , \ translate vector
create t1 1.0e f>fi , 1.0e f>fi , 1.0e f>fi , \ scale vector
45 value angle

0 value x
0 value y
0 value z
0 value a
\ x = x cos β − y sin β
\ y = x sin β + y cos β
\ z = z
: zrot ( x y z a -- x y z )
  360 mod to a to z to y to x

  costable a cells + @ x fimul
  sintable a cells + @ y fimul -

  sintable a cells + @ x fimul
  costable a cells + @ y fimul +

  z
;

\ x = z sin β + x cos β
\ y = y
\ z = z cos β − x sin β
: yrot ( x y z a -- x y z )
  360 mod to a to z to y to x

  sintable a cells + @ z fimul
  costable a cells + @ x fimul +

  y

  costable a cells + @ z fimul
  sintable a cells + @ x fimul -
;

\ x = x
\ y = y cos β − z sin β
\ z = y sin β + z cos β
: xrot ( x y z a -- x y z )
  360 mod to a to z to y to x

  x

  costable a cells + @ y fimul
  sintable a cells + @ z fimul -

  sintable a cells + @ y fimul
  costable a cells + @ z fimul +
;

0 value face
create v0 vector3 allot
create v1 vector3 allot
create v2 vector3 allot
create n0 vector3 allot
create n1 vector3 allot
create n2 vector3 allot
: face>vertices ( n -- v0 v1 v2 n0 n1 n2 )
  face-at to face

  face face.v0.p + @ 1- position-at v@ v0 v!
  face face.v1.p + @ 1- position-at v@ v1 v!
  face face.v2.p + @ 1- position-at v@ v2 v!

  face face.v0.n + @ 1- normal-at v@ n0 v!
  face face.v1.n + @ 1- normal-at v@ n1 v!
  face face.v2.n + @ 1- normal-at v@ n2 v!

  v0 v@ v1 v@ v2 v@
  n0 v@ n1 v@ n2 v@
;

: v>rottrans ( v0 -- v1 )
  angle xrot
  angle yrot
  angle zrot
  t0 v@ vadd                 \ translate vector
;

create v0 vector3 allot
: v>proj ( v -- x y z )
  v0 v!
  v0 v@ t1 v@ vmul           \ scale vector
  v>rottrans
  >proj                      \ project to screen x0 y0 z0
;

create v0 vector3 allot
create v1 vector3 allot
create v2 vector3 allot
create n0 vector3 allot
create n1 vector3 allot
create n2 vector3 allot
0 value x0
0 value y0
0 value z0
0 value x1
0 value y1
0 value z1
0 value x2
0 value y2
0 value z2

create a1 vector3 allot
create b1 vector3 allot

: get-abs-max-3 ( a b c -- a )
  abs rot abs rot abs
  max max
;
  
: get-average-z z0 z1 z2 + + 3 fidiv ;

: draw-triangle ( v0 v1 v2 n0 n1 n2 -- )
  n2 v! n1 v! n0 v!
  v2 v! v1 v! v0 v!

  v0 v@ v>proj to z0 to y0 to x0 \ keep z fixed point
  v1 v@ v>proj to z1 to y1 to x1 
  v2 v@ v>proj to z2 to y2 to x2 

  0 i>fi 0 i>fi -1 i>fi
  n0 v@ v>rottrans vnormalize
  vdot fi>f facos 3.141592e fswap f- 3.141592e f/ 10e f* f>fi

  256 i>fi fimul fi>i
  dup dup set-color

  \ n0 v.x@ 255 i>fi fimul fi>i
  \ n0 v.y@ 255 i>fi fimul fi>i
  \ n0 v.z@ 255 i>fi fimul fi>i
  \ set-color

  \ get-average-z currentz !

  x0 y0 
  x1 y1 
  x2 y2 visible? if
    wireframe? if
      x0 fi>i y0 fi>i x1 fi>i y1 fi>i line
      x1 fi>i y1 fi>i x2 fi>i y2 fi>i line
      x2 fi>i y2 fi>i x0 fi>i y0 fi>i line
    else
      x0 y0
      x1 y1
      x2 y2 scanfill
    then
  then

;

create v0 vector3 allot
create v1 vector3 allot
create v2 vector3 allot
create n0 vector3 allot
create n1 vector3 allot
create n2 vector3 allot
: 3d
  s" models/monkey.obj" load-obj
  to fcount
  to ncount
  to pcount
  to faces
  to normals
  to positions

  init-sdl

  255 255 255 set-color

  false to wireframe?

  begin
    clear-screen
    \ clear-zbuffer

    fcount 0 do
      i face>vertices draw-triangle
    loop


    1000 60 / sdl-delay
    angle 1+ to angle

    flip-screen

    sdl-event sdl-poll-event
    sdl-event c@ SDL_KEYDOWN =
  until

  sdl-quit
;

3d

bye
