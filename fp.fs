\ This file provide fixed-point number related words. It is useful
\ when FPU is not available, or too slow compared to fixed point when
\ it provides good enough precision.

[undefined] fp.fs [if]

vocabulary fp.fs also fp.fs definitions

\ fixed point format
1 14 lshift      value #fp.bit           \ 4.14 for 16-bit words
#fp.bit 1-       constant #fp.mask
#fp.mask invert  constant #fbitmask'
#fp.bit 1 rshift constant #fp.bit/2

\ fixed point 1.15 format
: i>fp   ( n -- fp )       #fp.bit * ;
: fp>i   ( fp -- n )       #fp.bit / ;
: fpmul  ( n0 n1 - n2 )    * #fp.bit / ;
: fpdiv  ( n0 n1 - n2 )    swap #fp.bit * swap / ;
: i.d>fp ( d d -- fp )     swap i>fp + ;
: f>fp   ( f -- fp )                 \ 1.23
  fdup ftrunc                        \ 1.23 1.00
  fdup frot                          \ 1.00 1.00 1.23
  fswap f-                           \ 1.00 0.23
  #fp.bit 0 d>f f* f>d drop          \ scale fractional part
  f>d drop #fp.bit * + ;             \ scale integer part and add
: i3>fp3 ( a b c - a b c ) i>fp rot i>fp rot i>fp rot ;
: fp3>i3 ( a b c - a b c ) fp>i rot fp>i rot fp>i rot ;
: fpfloor ( n0 - n1 )      #fbitmask' and ;
: fpceil  ( n0 - n1 )      #fp.bit/2 + fpfloor ;

: fp>f    ( fp -- ) ( F: f -- ) 0 swap d>f 0 #fp.bit d>f f/ ;

previous definitions

[then]
