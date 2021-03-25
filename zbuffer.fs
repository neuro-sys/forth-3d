require sdl.fs
require fp.fs

create zbuffer #width #height * allot
variable currentz

#fbits negate #fbits * constant min-fp
: clear-zbuffer zbuffer #width #height * min-fp fill ;
