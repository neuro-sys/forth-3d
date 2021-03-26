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
\ load-obj ( addr0 u -- vaddr iaddr v f )
\
\ addr0 u is the string that contains the file name for the OBJ file.
\
\ vaddr contains the address of an array that contains x, y, z
\ position coordinates per vertex in fixed point format 1.12 (see
\ #fbits).
\
\ iaddr contains the address of an array that contains the integer
\ indeces into the vertex buffer that forms a triagle.
\

require fi.fs

0 value fd              \ file handle
0 value vertices        \ fixed point vertex positions of mesh
0 value indices         \ vertex indices
0 value vcount          \ number of vertices
0 value icount          \ number of indices

\ file related words
: fd>pad ( -- u )       pad 80 fd read-line throw drop ;
: open ( addr u -- fd ) r/o open-file throw ;
: open-file ( addr u )  open to fd ;
: close-file ( -- )     fd close-file throw ;
: rewind ( -- )         0. fd reposition-file throw ;

\ Count the number of vertices and indices in currently open file
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
: allot-buffer ( n -- addr ) here swap cells allot ;

0 value voffset
0 value ioffset
: push-v ( n -- ) vertices voffset cells + ! voffset 1+ to voffset ;
: push-i ( n -- ) indices ioffset cells + ! ioffset 1+ to ioffset ;

\ Parse next float from string delimited by spaces and return the
\ string for next float
: slurp-float ( addr0 u0 -- addr1 u1 ) ( F: f0 -- )
  2dup s"  " search
  if
    2dup 1 /string 2>r          \ save next string
    nip -
    >float 0= throw
    2r>                         \ restore next string skip space
  else
    2drop 2 -
    >float 0= throw
  then
;

\ Parse next integer from string delimited by spaces and return the
\ string for next integer
: slurp-integer ( addr0 u0 -- addr1 u1 n )
  2dup s"  " search
  if
    2dup 1 /string 2>r         \ save next string
    nip -
    0. 2swap >number 2drop drop
    2r> rot                    \ restore next string skip space
  else
    2drop
    0. 2swap >number 2drop drop
  then
;

: slurp-face ( addr u -- ) \ s" 1\2\3 4\5\6 7\8\9 "
  2dup s"  " search
  if
    2dup 2>r
    
    2r> 1 /string rot
  else
  then
;

: add-normal ( addr u -- ) \ s" -1.000000 -1.000000 -1.000000"
  2drop
;

: add-vertex ( addr u -- ) \ s" -1.000000 -2.000000 3.000000"
  slurp-float f>fi push-v
  slurp-float f>fi push-v
  slurp-float f>fi push-v
;

: add-index ( addr u -- ) \ s" 2 3 1"
  slurp-integer push-i
  slurp-integer push-i
  slurp-integer push-i
;

: skip-n ( addr0 u0 n -- addr1 u1 ) /string ;

: gulp ( addr n -- )
  over 2 s" vn" compare 0= if 2 skip-n add-normal exit then
  over 1 s" v"  compare 0= if 2 skip-n add-vertex exit then
  over 1 s" f"  compare 0= if 2 skip-n add-index exit then

  2drop
;

: slurp ( -- ) begin fd>pad ?dup while pad swap gulp repeat ;

: report ( -- )
  cr ." vertex count: " vcount .
  cr ." index count: "  icount .
;

: load-obj ( addr0 u -- vaddr iaddr v f )
  cr 2dup ." Reading file: " type
  open-file count-elements to icount to vcount
  rewind
  vcount allot-buffer to vertices
  icount allot-buffer to indices
  slurp
  close-file
  report
  vertices indices vcount icount
;
