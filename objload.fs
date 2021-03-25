\ This file loads a WaveFront OBJ file (see cube.obj for sample) into
\ memory.
\
\ # WaveFront OBJ File Format
\
\ The file contains a text representation of a 3D scene. A minimal
\ file looks like the following:
\ 
\ v -1.000000 -1.000000 1.000000
\ v -1.000000 1.000000 1.000000
\ v -1.000000 -1.000000 -1.000000
\ f 2 3 1
\
\ The line that starts with letter "v" denotes the 3 floats denoting
\ the vertex positions X, Y and Z.
\
\ The letter "f" denotes the index positions (starting from 1) into
\ the vertex list forming a triangle.
\
\ # How to use
\
\ load-obj ( addr0 u -- vaddr faddr v f )
\
\ addr0 u is the string that contains the file name for the OBJ file.
\
\ vaddr contains the address of an array that contains x, y, z
\ coordinates per vertex in fixed point format 1.15 (see #fbits).
\
\ faddr contains the address of an array that contains the integer
\ offsets into the vertex buffer that forms a triagle.
\

require fp.fs

0 warnings !

variable fd              \ file handle
variable vertices        \ fixed point vertex positions of mesh
variable faces           \ vertex indices
variable vcount          \ number of vertices
variable fcount          \ number of faces

\ file related words
: fd>pad ( -- u )       pad 80 fd @ read-line throw drop ;
: open ( addr u -- fd ) r/o open-file throw ;
: open-file ( addr u )  open fd ! ;
: close-file ( -- )     fd @ close-file throw ;
: rewind ( -- )         0 0 fd @ reposition-file throw ;

\ Count the number of vertices and faces in currently open file
: count-elements ( -- v f )
  0 0 \ vertex face counters
  begin
    fd>pad
  while
    pad c@ [char] v = if swap 3 + swap then
    pad c@ [char] f = if 3 + then
  repeat
;

\ reserve space in Dictionary area
: allot-buffer ( n -- addr ) here swap cell * allot ;

variable voffset 0 voffset !
variable foffset 0 foffset !
: push-v ( n -- ) vertices @ voffset @ cells + ! voffset @ 1+ voffset ! ;
: push-f ( n -- ) faces @ foffset @ cells + ! foffset @ 1+ foffset ! ;

\ Parse next float from string delimited by spaces and return the
\ string for next float
: slurp-float ( addr0 u0 -- addr1 u1 ) ( F: f0 -- )
  2dup s"  " search
  if
    2dup 2>r                    \ save next string
    swap drop -
    >float invert throw
    2r> 1 /string               \ restore next string skip space
  else
    2drop 2 -
    >float invert throw
  then
;

\ Parse next integer from string delimited by spaces and return the
\ string for next integer
: slurp-integer ( addr0 u0 -- addr1 u1 n )
  2dup s"  " search
  if
    2dup 2>r                   \ save next string
    swap drop -
    0 0 2swap >number 2drop drop
    2r> 1 /string rot          \ restore next string skip space
  else
    2drop
    0 0 2swap >number 2drop drop
  then
;

: add-vertex ( addr u -- ) \ s" -1.000000 -2.000000 3.000000"
  slurp-float f>fi push-v
  slurp-float f>fi push-v
  slurp-float f>fi push-v
;

: add-face ( addr u -- ) \ s" 2 3 1"
  slurp-integer push-f
  slurp-integer push-f
  slurp-integer push-f
;

: skip-letter ( addr0 u0 -- addr1 u1 ) 2 - swap 2 + swap ;

: gulp ( n -- ) \ parse the pad area, n is pad count
  pad c@
  case
    [char] v of pad swap skip-letter add-vertex endof
    [char] f of pad swap skip-letter add-face endof
    drop
  endcase
;

: slurp ( -- ) begin fd>pad ?dup while gulp repeat ;

: load-obj ( addr0 u -- vaddr faddr v f )
  open-file count-elements fcount ! vcount !
  rewind
  vcount @ allot-buffer vertices !
  fcount @ allot-buffer faces !
  slurp
  close-file
  vertices @ faces @ vcount @ fcount @
;
