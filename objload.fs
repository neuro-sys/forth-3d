\ Load a WaveFront OBJ file into memory
0 warnings !

variable fd              \ file handle
variable vertices        \ fixed point vertex positions of mesh
variable faces           \ vertex indices
variable vcount          \ number of vertices
variable fcount          \ number of faces

\ file words
: fd>pad               pad 80 fd @ read-line throw drop ;
: open ( addr u )      r/o open-file throw ;
: open-file ( addr u ) open fd ! ;
: rewind               0 0 fd @ reposition-file throw ;
: close-file           fd @ close-file ;

: count-elements ( -- v f )
  0 0 \ vertex face counters
  begin
    fd>pad
  while
    pad c@ [char] v = if swap 3 + swap then
    pad c@ [char] f = if 3 + then
  repeat
;

: alloc-buffer ( n -- addr ) here swap cell * allot ;

\ fixed point format
1 15 lshift constant #fbits
: f>fi  ( F: f -- ) ( -- n )
  fdup f>d drop #fbits * 1.0e fmod #fbits 0 d>f f* f>d drop + ;

variable voffset 0 voffset !
variable foffset 0 foffset !
: push-v ( n -- ) vertices @ voffset @ cells + ! voffset @ 1+ voffset ! ;
: push-f ( n -- ) faces @ foffset @ cells + ! foffset @ 1+ foffset ! ;

: v>fi ( faddr -- n ) f@ f>fi ;

: slurp-float ( addr0 u0 -- addr1 u1 ) ( F: f0 -- )
  2dup s"  " search
  if
    2dup >r >r                   \ save next string
    swap drop -
    >float invert throw
    r> r> 1 /string              \ restore next string skip space
  else
    2drop 2 -
    >float invert throw
  then
;

: slurp-integer ( addr0 u0 -- addr1 u1 n )
  2dup s"  " search
  if
    2dup >r >r                   \ save next string
    swap drop -
    0 0 2swap >number 2drop drop
    r> r> 1 /string rot          \ restore next string skip space
  else
    2drop
    0 0 2swap >number 2drop drop
  then
;

: add-vertex ( n -- )
  pad 2 + swap 2 - ( addr u ) \ s" -1.000000 -2.000000 3.000000"
  slurp-float f>fi push-v
  slurp-float f>fi push-v
  slurp-float f>fi push-v
;

: add-face ( n -- )
  pad 2 + swap 2 - ( addr u ) \ s" 2 3 1"
  slurp-integer push-f
  slurp-integer push-f
  slurp-integer push-f
;

: gulp
  pad c@
  case
    [char] v of add-vertex endof
    [char] f of add-face endof
    drop
  endcase
;

: slurp begin fd>pad ?dup while gulp repeat ;

: load-obj ( addr0 u -- vaddr faddr v f )
  open-file count-elements fcount ! vcount !
  rewind
  vcount @ alloc-buffer vertices !
  fcount @ alloc-buffer faces !
  slurp
  close-file
  vertices @ faces @ vcount @ fcount @
;


\ \ test data
\ create vertices1
\ -1.000000e f>fi , -1.000000e f>fi ,  1.000000e f>fi ,
\ -1.000000e f>fi ,  1.000000e f>fi ,  1.000000e f>fi ,
\ -1.000000e f>fi , -1.000000e f>fi , -1.000000e f>fi ,
\ -1.000000e f>fi ,  1.000000e f>fi , -1.000000e f>fi ,
\  1.000000e f>fi , -1.000000e f>fi ,  1.000000e f>fi ,
\  1.000000e f>fi ,  1.000000e f>fi ,  1.000000e f>fi ,
\  1.000000e f>fi , -1.000000e f>fi , -1.000000e f>fi ,
\  1.000000e f>fi ,  1.000000e f>fi , -1.000000e f>fi ,
\ here vertices1 - vector3 / constant #vertices
\ variable vertices vertices1 vertices !
\ variable vcount #vertices vcount !

\ create faces1
\ 2 , 3 , 1 , 
\ 4 , 7 , 3 , 
\ 8 , 5 , 7 , 
\ 6 , 1 , 5 , 
\ 7 , 1 , 3 , 
\ 4 , 6 , 8 , 
\ 2 , 4 , 3 , 
\ 4 , 8 , 7 , 
\ 8 , 6 , 5 , 
\ 6 , 2 , 1 , 
\ 7 , 5 , 1 , 
\ 4 , 2 , 6 , 
\ here faces1 - 1 cells / constant #faces
\ variable faces faces1 faces !
\ variable fcount #faces fcount !
