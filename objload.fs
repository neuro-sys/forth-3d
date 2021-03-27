\ This file loads a WaveFront OBJ file (see cube.obj for sample) into
\ memory.
\
\ # WaveFront OBJ File Format
\
\ The file contains a text representation of a 3D scene. A minimal and
\ supported file format looks like the following:
\ 
\ v -1.000000 -1.000000 1.000000
\ v -1.000000 1.000000 1.000000
\ v -1.000000 -1.000000 -1.000000
\ vn -1.000000 0.000000 0.000000
\ vn -1.000000 0.000000 0.000000
\ vn -1.000000 0.000000 0.000000
\ f 2//1 3//1 1//1
\
\ The line that starts with letter "v" denotes the position vector X,
\ Y and Z.
\
\ The letters "vn" denote the 3 floats denoting the normal vector X, Y
\ and Z.
\
\ The letter "f" denotes the face indices (starting from 1) into the
\ vertex list forming a triangle, and normal indices respectively.
\
\ # How to use
\
\ load-obj ( addr0 u -- paddr naddr iaddr p n i )
\
\ addr0 u is the string that contains the file name for the OBJ file.
\
\ paddr is the array of vertex positions.
\
\ naddr is the array of normal positions.
\
\ iaddr is the array of vertex and normal indices interleaved. See
\ face-cells for layout
\
\ p, n and i are the count of elements for each of the arrays
\ respectively.

require fi.fs

0
dup constant face.v0.p cell +
dup constant face.v0.n cell +
dup constant face.v1.p cell +
dup constant face.v1.n cell +
dup constant face.v2.p cell +
dup constant face.v2.n cell +
constant face-cells

0
dup constant position.x cell +
dup constant position.y cell +
dup constant position.z cell +
constant position-cells

0
dup constant normal.x cell +
dup constant normal.y cell +
dup constant normal.z cell +
constant normal-cells

0 value fd              \ file handle
0 value positions       \ array of vertex positions
0 value normals         \ array of vertex normals
0 value faces           \ array of face-cells
0 value pcount          \ number of x, y, z positions
0 value ncount          \ number of x, y, z normals
0 value fcount          \ number of offset faces

\ file related words
: fd>pad       ( -- u ) pad 80 fd read-line throw drop ;
: open ( addr u -- fd ) r/o open-file throw ;
: open-obj   ( addr u ) open to fd ;
: close-obj      ( -- ) fd close-file throw ;
: rewind         ( -- ) 0. fd reposition-file throw ;

\ Count the number of positions and faces in currently open file
0 value position
0 value normal
0 value index
: count-elements ( -- position normal index )
  0 to position
  0 to normal
  0 to index
  begin
    fd>pad
  while
    pad 3 s" vn " compare 0= if normal 1+ to normal then
    pad 2 s" v "  compare 0= if position 1+ to position then
    pad 2 s" f "  compare 0= if index 1+ to index then
  repeat
  position to pcount
  normal to ncount
  index to fcount
;

0 value poffset
0 value noffset
0 value foffset

: position-at ( n -- adr ) position-cells * positions + ;
: normal-at   ( n -- adr ) normal-cells   * normals + ;
: face-at     ( n -- adr ) face-cells     * faces + ;

: next-p ( -- addr )     poffset position-at poffset 1+ to poffset ;
: next-n ( -- addr )     noffset normal-at   noffset 1+ to noffset ;
: next-f ( -- addr )     foffset face-at     foffset 1+ to foffset ;

\ Parse next float from string delimited by spaces and return the
\ string for next float
: next-float ( addr0 u0 -- addr1 u1 ) ( F: f0 -- )
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

\ skip until next space or /
: skip-space-or/ ( addr0 u0 -- addr1 u1 )
  begin
    2dup drop c@ >r
    1 /string
    dup 0=
    r@ [char] / = or
    r> 32 = or
  until
;

\ Parse next integer from string delimited by space or "/" and return
\ the string for next integer
: next-face-int ( addr0 u0 -- addr1 u1 n )
  2dup skip-space-or/ dup 0<>
  if
    2dup 2>r         \ save next string
    nip -
    0. 2swap >number 2drop drop
    2r> rot          \ restore next string skip space
  else
    2drop
    0. 2swap >number 2drop drop
  then
;

: add-normal ( addr u -- ) \ s" -1.000000 -1.000000 -1.000000"
  next-float f>fi >r
  next-float f>fi >r
  next-float f>fi >r

  next-n
  r> over normal.z + !
  r> over normal.y + !
  r> swap normal.x + !
;

: add-vertex ( addr u -- ) \ s" -1.000000 -2.000000 3.000000"
  next-float f>fi >r
  next-float f>fi >r
  next-float f>fi >r

  next-p
  r> over position.z + !
  r> over position.y + !
  r> swap position.x + !
;

: add-face ( addr u -- ) \ s" 1//2 3//4 5//6"
  next-f >r
  next-face-int r@ face.v0.p + !
  next-face-int drop
  next-face-int r@ face.v0.n + !
  next-face-int r@ face.v1.p + !
  next-face-int drop
  next-face-int r@ face.v1.n + !
  next-face-int r@ face.v2.p + !
  next-face-int drop
  next-face-int r> face.v2.n + !
;

: gulp ( addr n -- )
  over 3 s" vn " compare 0= if 3 /string add-normal exit then
  over 2 s" v "  compare 0= if 2 /string add-vertex exit then
  over 2 s" f "  compare 0= if 2 /string add-face   exit then

  2drop
;

: slurp ( -- ) begin fd>pad ?dup while pad swap gulp repeat ;

: allocate-buffers ( -- )
  here pcount position-cells * allot to positions
  here ncount normal-cells   * allot to normals
  here fcount face-cells     * allot to faces

  positions pcount position-cells * erase
  normals   ncount normal-cells   * erase
  faces     fcount face-cells     * erase
;

: load-obj ( addr0 u -- paddr naddr iaddr p n i )
  open-obj count-elements rewind allocate-buffers slurp close-obj
  positions normals faces pcount ncount fcount
;

\ s" models/cube.obj" load-obj
\ to fcount to ncount to pcount
\ to faces to normals to positions
\ cr ." Positions: " positions pcount position-cells * dump
\ cr ." Normals: " normals   ncount normal-cells   * dump
\ cr ." Faces: " faces     fcount face-cells     * dump

\ bye
\ s" 1/2/3 4/5/6 7/8/9"
\ cr 2dup type
\ cr skip-space-or/ 2dup type .s
\ cr skip-space-or/ 2dup type .s
\ cr skip-space-or/ 2dup type .s
\ cr skip-space-or/ 2dup type .s
\ cr skip-space-or/ 2dup type .s
\ cr skip-space-or/ 2dup type .s
\ cr skip-space-or/ 2dup type .s
\ cr skip-space-or/ 2dup type .s
\ cr skip-space-or/ 2dup type .s

\ s" 1//3 4//6 7//9"
\ cr next-face-int . 2dup type .s
\ cr next-face-int . 2dup type .s
\ cr next-face-int . 2dup type .s
\ cr next-face-int . 2dup type .s
\ cr next-face-int . 2dup type .s
\ cr next-face-int . 2dup type .s
\ cr next-face-int . 2dup type .s
\ cr next-face-int . 2dup type .s
\ cr next-face-int . .s

\ bye
