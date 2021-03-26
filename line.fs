require sdl.fs
require zbuffer.fs

0 value x0
0 value y0
0 value x1
0 value y1
0 value dx
0 value dy
0 value e
0 value s
: line-put-pixel
  x0 y0 pixel-off? 0= if
    x0 y0 put-pixel
  then
;

: linev
  dx abs to dx
  dy abs to dy
  dx 1 rshift to e \ needed?
  x1 x0 > if 1 else -1 then to s
  begin
    y1 y0 >
  while
    line-put-pixel
    e dx + to e
    e dy >= if
      e dy - to e
      x0 s + to x0
    then
    y0 1+ to y0
    \ yield
  repeat
;

: lineh
  dx abs to dx
  dy abs to dy
  dy 1 rshift to e \ needed?
  y1 y0 > if 1 else -1 then to s
  begin
    x1 x0 >
  while
    line-put-pixel
    e dy + to e
    e dx >= if
      e dx - to e
      y0 s + to y0
      \ yield
    then
    x0 1+ to x0
  repeat
;

: line ( x0 y0 x1 y1 -- )
  to y1 to x1 to y0 to x0

  x1 x0 - to dx
  y1 y0 - to dy

  dx abs dy abs > if \ horizontal
    dx 0 > if \ right
      lineh
    else \ left
      x0 x1 swap to x1 to x0
      y0 y1 swap to y1 to y0
      lineh
    then
  else \ vertical
    dy 0 > if \ down
      linev
    else \ up
      x0 x1 swap to x1 to x0
      y0 y1 swap to y1 to y0
      linev
    then
  then
;

#width 2/ value ox
#height 2/ value oy
: line-test
  init-sdl

  255 255 255 set-color

  ox oy ox 20 + oy 50 - line \ sector 0
  ox oy ox 50 + oy 20 + line \ sector 1
  ox oy ox 50 + oy 20 - line \ sector 2
  ox oy ox 20 + oy 50 + line \ sector 3
  ox oy ox 20 - oy 50 - line \ sector 4
  ox oy ox 50 - oy 20 + line \ sector 5
  ox oy ox 50 - oy 20 - line \ sector 6
  ox oy ox 20 - oy 50 + line \ sector 7

  flip-screen
  wait-key
  sdl-quit
;
