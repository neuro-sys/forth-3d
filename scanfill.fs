[undefined] scanfill.fs [if]
vocabulary scanfill.fs also scanfill.fs definitions

require sdl.fs
require fi.fs
require zbuffer.fs

also sdl.fs
also fi.fs
also zbuffer.fs

0 value y
0 value x1
0 value x2
0 value n0
0 value 'zbuffer

: hline ( y x1 x2 -- )
  to x2 to x1 to y

  x1 x2 > if x1 x2 swap to x2 to x1 then

  x2 x1 - to n0

  n0 0=
  x2 y pixel-off? or 0= if 
    x2 x1 do i y put-pixel loop \ Optimize this bit, inline ASM?
  then
;

\ all values are in 8.8 fixed point format
0 value x0 \ top
0 value y0
0 value x1 \ left
0 value y1
0 value x2 \ right
0 value y2

0 value dxdy-left
0 value dxdy-right
0 value dxdy-left-bottom
0 value cur-left-x
0 value cur-right-x
0 value cur-left-bottom-x
0 value scanline
: scan-edges
  x1 x0 - y1 y0 - dup 0= if 2drop 1 else fidiv then to dxdy-left
  x2 x0 - y2 y0 - dup 0= if 2drop 1 else fidiv then to dxdy-right
  x2 x1 - y2 y1 - dup 0= if 2drop 1 else fidiv then to dxdy-left-bottom

  x0 to cur-left-x
  x0 to cur-right-x
  x1 to cur-left-bottom-x

  \ scan top half
  y0 to scanline
  begin
    scanline y1 <
  while
    scanline fi>i cur-left-x fi>i cur-right-x fi>i
    hline

    cur-left-x dxdy-left + to cur-left-x
    cur-right-x dxdy-right + to cur-right-x
    scanline 1 i>fi + to scanline
  repeat

  \ scan bottom half
  y1 to scanline
  begin
    scanline y2 <
  while
    scanline fi>i cur-left-bottom-x fi>i cur-right-x fi>i
    hline

    cur-left-bottom-x dxdy-left-bottom + to cur-left-bottom-x
    cur-right-x dxdy-right + to cur-right-x
    scanline 1 i>fi + to scanline
  repeat
;

: scanfill ( x0 y0 x1 y1 x2 y2 -- )
  ficeil to y2
  ficeil to x2
  ficeil to y1
  ficeil to x1
  ficeil to y0
  ficeil to x0

  \ sort left right, top down
  y0 y1 > if
    x0 x1 swap to x1 to x0
    y0 y1 swap to y1 to y0
  then
  y1 y2 > if
    x1 x2 swap to x2 to x1
    y1 y2 swap to y2 to y1
  then
  y0 y1 > if
    x0 x1 swap to x1 to x0
    y0 y1 swap to y1 to y0
  then

  scan-edges
;

\ #width 2/ value ox
\ #height 2/ value oy
\ : test
\   init-sdl
\   hex 0x00 0x80 0x80 decimal set-color

\   ox      i>fi oy      i>fi \ top
\   ox 30 + i>fi oy 90 + i>fi \ left
\   ox 39 - i>fi oy 180 + i>fi \ right

\   hex 0x80 0x80 0x80 decimal set-color
\   ox      i>fi oy      i>fi \ top
\   ox 30 - i>fi oy 90 + i>fi \ left
\   ox 39 + i>fi oy 180 + i>fi \ right
\   scanfill

\   flip-screen
\   wait-key
\   sdl-quit
\ ;

\ test bye

previous definitions

[then]
