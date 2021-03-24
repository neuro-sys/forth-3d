c-library sdl
s" SDL" add-lib

\c #include <SDL/SDL.h>

$00000000	constant SDL_SWSURFACE
$00000020	constant SDL_INIT_VIDEO
$0000FFFF	constant SDL_INIT_EVERYTHING
$00000002	constant SDL_KEYDOWN

32          constant sdl-pixels-offset
24          constant sdl-event-type-size

c-function sdl-init             SDL_Init            n -- n
c-function sdl-set-video-mode	SDL_SetVideoMode	n n n n -- a
c-function sdl-flip             SDL_Flip            a -- n
c-function sdl-quit             SDL_Quit            -- void
c-function sdl-delay            SDL_Delay           n -- void
c-function sdl-poll-event       SDL_PollEvent       a -- void
c-function sdl-delay            SDL_Delay           n -- void

end-c-library

640 constant #width
480 constant #height

#width 4 * constant #stride

variable color 3 cells allot
variable surface
variable pixels

variable sdl-event sdl-event-type-size allot

: wait-key 
  begin
    sdl-event sdl-poll-event
    sdl-event c@ SDL_KEYDOWN =
  until
;

: set-color ( b g r -- )
  color c!
  color 1 + c!
  color 2 + c!
;

: put-pixel ( x y -- )
  pixels @ -rot #stride * swap 4 * + +

  dup color     c@ swap c!
  dup color 1 + c@ swap 1 + c!
      color 2 + c@ swap 2 + c!
;

: clear-screen ( -- )
  #stride #height * pixels @ + pixels @ do
    0 i c!
  loop
;

SDL_INIT_EVERYTHING sdl-init
0<> [if] ." Error sdl-init" exit [then]

#width #height 32 SDL_SWSURFACE sdl-set-video-mode
dup 0< [if] ." Error sdl-set-video-mode" exit [then] surface !

\ save screen buffer address
surface @ sdl-pixels-offset + @ pixels !

: flip-screen surface @ sdl-flip throw ;