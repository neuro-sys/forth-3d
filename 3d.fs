\ 3d drawing with fixed point arithmetic
\ 
0 warnings !

require sdl.fs
require objload.fs
require fp.fs
require zbuffer.fs
require line.fs
require trig.fs
require vec3.fs
require scanfill.fs

variable vertices
variable faces
0 value vcount
0 value fcount

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

create t0 0.0e f>fi , 0.0e f>fi , -2.5e f>fi , \ translate vector
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

: face>vertex ( f -- x y z )
  cells faces @ + @ 1-         \ face index
  vector3 * vertices @ + v@    \ vertex position
;

create v0 vector3 allot
: v>proj ( v -- x y z )
  v0 v!
  v0 v@ t1 v@ vmul           \ scale vector
  angle xrot
  angle yrot
  angle zrot
  t0 v@ vadd                 \ translate vector
  >proj                      \ project to screen x0 y0 z0
;

create v0 vector3 allot
create v1 vector3 allot
create v2 vector3 allot
0 value x0
0 value y0
0 value z0
0 value x1
0 value y1
0 value z1
0 value x2
0 value y2
0 value z2

: get-average-z z0 z1 z2 + + 3 fidiv ;

: draw-triangle ( v0 v1 v2 -- )
  v2 v! v1 v! v0 v!

  v0 v@ v>proj to z0 to y0 to x0 \ keep z fixed point
  v1 v@ v>proj to z1 to y1 to x1 
  v2 v@ v>proj to z2 to y2 to x2 

  get-average-z currentz !

  x0 y0 
  x1 y1 
  x2 y2 visible? if
    x0 y0
    x1 y1
    x2 y2 scanfill
  then
;

create v0 vector3 allot
create v1 vector3 allot
create v2 vector3 allot
: 3d
  init-sdl

  255 255 255 set-color

  begin
    clear-screen
    clear-zbuffer

    fcount 0 do
      i     face>vertex v0 v!
      i 1 + face>vertex v1 v!
      i 2 + face>vertex v2 v!

      v0 v@ v1 v@ v2 v@ draw-triangle
    3 +loop

    1000 60 / sdl-delay
    \ angle @ 1 + angle !

    flip-screen

    sdl-event sdl-poll-event
    sdl-event c@ SDL_KEYDOWN =
  until

  sdl-quit
;

s" models/torus.obj" load-obj to fcount to vcount faces ! vertices !
3d

bye
