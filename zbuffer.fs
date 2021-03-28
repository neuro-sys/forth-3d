[undefined] zbuffer.fs [if]

vocabulary zbuffer.fs also zbuffer.fs definitions

require sdl.fs
require fi.fs

also sdl.fs
also fi.fs

variable zbuffer-on? false zbuffer-on? !

create zbuffer #width #height * cells allot

0 value zbuffer-current

: get-zbuffer-pixel ( x y -- n )
  zbuffer -rot #width cells * swap cells + + @
;

: set-zbuffer-pixel ( n x y -- )
  zbuffer -rot #width cells * swap cells + + !
;

#fbits negate #fbits * constant #max-fp

: clear-zbuffer
  zbuffer #width #height * cells + zbuffer do
    #max-fp i !
  cell +loop
;

previous definitions

[then]
