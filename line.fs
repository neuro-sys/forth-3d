require sdl.fs
require zbuffer.fs

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
variable 'zbuffer
: put-pixel
  y0 @ #width * x0 @ + zbuffer + 'zbuffer !
  'zbuffer @ @ currentz @ >
  x0 @ y0 @ pixel-off? invert
  and if
    x0 @ y0 @ put-pixel
    currentz @ 'zbuffer @ !
  then
;

: linev
  x1 @ x0 @ - abs dx !
  y1 @ y0 @ - abs dy !
  dx @ 2 / e !
  x1 @ x0 @ > if 1 else -1 then s !
  begin
    y1 @ y0 @ >
  while
    put-pixel
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
    put-pixel
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
