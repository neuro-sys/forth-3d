require sdl.fs
require fp.fs
require zbuffer.fs

variable y
variable x1
variable x2
variable n
variable 'zbuffer

: hline ( y x1 x2 -- )
  x2 ! x1 ! y !

  x1 @ x2 @ > if x1 @ x2 @ swap x2 ! x1 ! then

  x1 @ y @ pixel-off?
  x2 @ y @ pixel-off? or if rdrop exit then

  x2 @ x1 @ - n !
  x1 @ y @ get-pixel-addr ( addr0 -- )
  dup n @ 4 * + swap ( addr1 addr2 -- )
  ?do
    color i 3 cmove
    255 i 3 + !
  4 +loop
;

\ all values are in 8.8 fixed point format
variable x0 \ top
variable y0 \
variable x1 \ left
variable y1
variable x2 \ right
variable y2

variable dxdy-left
variable dxdy-right
variable dxdy-left-bottom
variable cur-left-x
variable cur-right-x
variable cur-left-bottom-x
variable scanline
: scan-edges
  x1 @ x0 @ - y1 @ y0 @ - dup 0= if 1 else fidiv then dxdy-left !
  x2 @ x0 @ - y2 @ y0 @ - dup 0= if 1 else fidiv then dxdy-right !
  x2 @ x1 @ - y2 @ y1 @ - dup 0= if 1 else fidiv then dxdy-left-bottom !

  x0 @ fifloor cur-left-x !
  x0 @ fifloor cur-right-x !
  x1 @ fifloor cur-left-bottom-x !

  \ scan top half
  y0 @ scanline !
  begin
    scanline @ y1 @ <
  while
    scanline @ fi>i cur-left-x @ fi>i cur-right-x @ fi>i
    hline

    cur-left-x @ dxdy-left @ + cur-left-x !
    cur-right-x @ dxdy-right @ + cur-right-x !
    scanline @ 1 i>fi + scanline !
  repeat

  \ scan bottom half
  y1 @ scanline !
  begin
    scanline @ y2 @ <
  while
    scanline @ fi>i cur-left-bottom-x @ fi>i cur-right-x @ fi>i
    hline

    cur-left-bottom-x @ dxdy-left-bottom @ + cur-left-bottom-x !
    cur-right-x @ dxdy-right @ + cur-right-x !
    scanline @ 1 i>fi + scanline !
  repeat
;

: scanfill ( x0 y0 x1 y1 x2 y2 -- )
  y2 ! x2 ! y1 ! x1 ! y0 ! x0 !

  \ sort left right, top down
  y0 @ y1 @ > if
    x0 @ x1 @ swap x1 ! x0 !
    y0 @ y1 @ swap y1 ! y0 !
  then
  y1 @ y2 @ > if
    x1 @ x2 @ swap x2 ! x1 !
    y1 @ y2 @ swap y2 ! y1 !
  then
  y0 @ y1 @ > if
    x0 @ x1 @ swap x1 ! x0 !
    y0 @ y1 @ swap y1 ! y0 !
  then

  scan-edges
;

\ variable ox #width 2/  ox !
\ variable oy #height 2/ oy !
\ : test
\   init-sdl
\   hex 0x00 0x80 0x80 decimal set-color

\   ox @      i>fi oy @      i>fi \ top
\   ox @ 30 + i>fi oy @ 90 + i>fi \ left
\   ox @ 39 - i>fi oy @ 180 + i>fi \ right

\   hex 0x80 0x80 0x80 decimal set-color
\   ox @      i>fi oy @      i>fi \ top
\   ox @ 30 - i>fi oy @ 90 + i>fi \ left
\   ox @ 39 + i>fi oy @ 180 + i>fi \ right
\   scanfill

\   flip-screen
\   wait-key
\   sdl-quit
\ ;

\ test bye
