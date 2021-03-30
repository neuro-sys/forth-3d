\ This file provide fixed-point number related words. It is useful
\ when FPU is not available, or too slow compared to fixed point when
\ it provides good enough precision.

\ http://www.forth.org/fd/FD-V04N1.pdf

[undefined] fp.fs [if]

vocabulary fp.fs also fp.fs definitions

\ fixed point format
14                              constant point      \ fractional point
1 point lshift                  constant fp-bitmask
fp-bitmask 1-                   constant fp-fmask   \ fractional bit mask
fp-fmask invert                 constant fp-imask   \ integer bit mask
fp-bitmask 1 rshift             constant fp.bit/2   \ 0.5 in fp
fp-bitmask negate fp-bitmask *  constant max-fp     \ maximum value
fp-bitmask negate fp-bitmask *  constant min-fp     \ minimum value
10 point lshift                 constant fp10

\ fixed point 1.15 format
: i>fp   ( n -- fp )            point lshift ;
: fp>i   ( fp -- n )            point rshift ;
: fp*    ( n0 n1 -- n2 )        fp-bitmask */ ;
: fp/    ( n0 n1 -- n2 )        fp-bitmask swap */ ;
: if>fp  ( n0 n1 -- fp )        i>fp fp10 fp/ swap i>fp + ;
: i3>fp3 ( a b c - a b c )      i>fp rot i>fp rot i>fp rot ;
: fp3>i3 ( a b c - a b c )      fp>i rot fp>i rot fp>i rot ;
: fpfloor ( n0 - n1 )           fp-imask and ;
: fpceil  ( n0 - n1 )           fp.bit/2 + fpfloor ;

\ FPU words TODO: remove
: f>fp   ( f -- fp )                 \ 1.23
  fdup ftrunc                        \ 1.23 1.00
  fdup frot                          \ 1.00 1.00 1.23
  fswap f-                           \ 1.00 0.23
  fp-bitmask 0 d>f f* f>d drop          \ scale fractional part
  f>d drop fp-bitmask * + ;             \ scale integer part and add
: fp>f    ( fp -- ) ( F: f -- ) 0 swap d>f 0 fp-bitmask d>f f/ ;

previous definitions

[then]
