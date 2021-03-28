\ Polygon based 3D renderer.
\
\ TODO:
\
\ + Use vocabularies for modules
\ + Add z-buffering
\ - Add z-sorting
\ - Get rid of FPU completely
\   - string>fi
\   - fisqrt
\ - Use matrices

require sdl.fs
require objload.fs
require fp.fs
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
  y1 y0 + fpmul

  x2 x1 -
  y2 y1 + fpmul

  x0 x2 -
  y0 y2 + fpmul

  + + 0 <=
;

#width  2 / i>fp constant #width-half
#height 2 / i>fp constant #height-half
#width  2 / i>fp constant #dist
  
: >p  ( z p -- p1 )  #dist fpmul swap fpdiv ;

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

create t0 0.0e f>fp , 0.0e f>fp , -3.5e f>fp , \ translate vector
create t1 1.0e f>fp , 1.0e f>fp , 1.0e f>fp ,  \ scale vector
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

  costable a cells + @ x fpmul
  sintable a cells + @ y fpmul -

  sintable a cells + @ x fpmul
  costable a cells + @ y fpmul +

  z
;

\ x = z sin β + x cos β
\ y = y
\ z = z cos β − x sin β
: yrot ( x y z a -- x y z )
  360 mod to a to z to y to x

  sintable a cells + @ z fpmul
  costable a cells + @ x fpmul +

  y

  costable a cells + @ z fpmul
  sintable a cells + @ x fpmul -
;

\ x = x
\ y = y cos β − z sin β
\ z = y sin β + z cos β
: xrot ( x y z a -- x y z )
  360 mod to a to z to y to x

  x

  costable a cells + @ y fpmul
  sintable a cells + @ z fpmul -

  sintable a cells + @ y fpmul
  costable a cells + @ z fpmul +
;

0 value face
: face>vertices ( n -- v0 v1 v2 n0 n1 n2 )
  face-at to face

  face face.v0.p + @ 1- position-at v@
  face face.v1.p + @ 1- position-at v@
  face face.v2.p + @ 1- position-at v@

  face face.v0.n + @ 1- normal-at v@
  face face.v1.n + @ 1- normal-at v@
  face face.v2.n + @ 1- normal-at v@
;

\ TODO: Use matrices
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

: get-average-z z0 z1 z2 + + 3 fpdiv ;

: draw-triangle ( v0 v1 v2 n0 n1 n2 -- )
  n2 v! n1 v! n0 v!
  v2 v! v1 v! v0 v!

  v0 v@ v>proj to z0 to y0 to x0 \ keep z fixed point
  v1 v@ v>proj to z1 to y1 to x1 
  v2 v@ v>proj to z2 to y2 to x2 

  0 i>fp 0 i>fp -1 i>fp
  n0 v@ v>rottrans vnormalize
  \ cheating with FPU for facos
  vdot fp>f facos 3.141592e fswap f- 3.141592e f/ 10e f* f>fp

  256 i>fp fpmul fp>i
  dup dup set-color


  x0 y0 
  x1 y1 
  x2 y2 visible? if
    wireframe? if
      x0 fp>i y0 fp>i x1 fp>i y1 fp>i line
      x1 fp>i y1 fp>i x2 fp>i y2 fp>i line
      x2 fp>i y2 fp>i x0 fp>i y0 fp>i line
    else
      x0 y0 z0
      x1 y1 z1
      x2 y2 z2 scanfill
    then
  then

;

: 3d
  s" models/torus.obj" load-obj
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
    clear-zbuffer

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
