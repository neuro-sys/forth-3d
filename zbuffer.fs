require sdl.fs
require fi.fs

variable zbuffer-on? false zbuffer-on? !
create zbuffer #width #height * allot
variable currentz

#fbits negate #fbits * constant min-fp
: clear-zbuffer zbuffer #width #height * min-fp fill ;
