[undefined] zbuffer.fs [if]

vocabulary zbuffer.fs also zbuffer.fs definitions

require sdl.fs
require fi.fs

also sdl.fs
also fi.fs

variable zbuffer-on? false zbuffer-on? !

create zbuffer #width #height * allot

variable currentz

#fbits negate #fbits * constant min-fp

: clear-zbuffer zbuffer #width #height * min-fp fill ;

previous definitions

[then]
