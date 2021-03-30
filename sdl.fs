[undefined] sdl.fs [if]

c-library sdl
s" SDL" add-lib

\c #include <SDL/SDL.h>

c-function sdl-init             SDL_Init            n -- n
c-function sdl-set-video-mode	SDL_SetVideoMode	n n n n -- a
c-function sdl-flip             SDL_Flip            a -- n
c-function sdl-quit             SDL_Quit            -- void
c-function sdl-delay            SDL_Delay           n -- void
c-function sdl-poll-event       SDL_PollEvent       a -- void

end-c-library

vocabulary sdl.fs also sdl.fs definitions

$00000000	constant SDL_SWSURFACE
$80000000   constant SDL_FULLSCREEN
$00000020	constant SDL_INIT_VIDEO
$0000FFFF	constant SDL_INIT_EVERYTHING
$00000002	constant SDL_KEYDOWN

32          constant sdl-pixels-offset
24          constant sdl-event-type-size

1024 constant #width
768 constant #height

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

: get-pixel-addr ( x y -- addr )
  pixels @ -rot #stride * swap 4 * + +
;

: set-pixel ( addr -- )
  dup color     c@ swap c!
  dup color 1 + c@ swap 1 + c!
      color 2 + c@ swap 2 + c!
;

: put-pixel ( x y -- )
  get-pixel-addr set-pixel
;

: pixel-off? ( x y -- t )
  dup #height >= swap 0 < or swap
  dup #width >= swap 0 < or
  or
;

: clear-screen ( -- )
  #stride #height * pixels @ + pixels @ do
    0 i c!
  loop
;

: init-sdl
  SDL_INIT_EVERYTHING sdl-init
  0<> if ." Error sdl-init" exit then

  #width #height 32 SDL_SWSURFACE sdl-set-video-mode
  dup 0< if ." Error sdl-set-video-mode" exit then surface !

  \ save screen buffer address
  surface @ sdl-pixels-offset + @ pixels !
;

: flip-screen surface @ sdl-flip throw ;

previous definitions

[then]
