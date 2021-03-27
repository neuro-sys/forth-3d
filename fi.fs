\ This file provide fixed-point number related words. It is useful
\ when FPU is not available, or too slow compared to fixed point when
\ it provides good enough precision.

\ fixed point format
1 14 lshift      value #fbits           \ 4.14 for 16-bit words
#fbits 1-        constant #fbitmask
#fbitmask invert constant #fbitmask'
#fbits 1 rshift  constant #fbits/2

\ fixed point 1.15 format
: i>fi   ( d -- fi )       #fbits * ;
: fi>i   ( fi -- d )       #fbits / ;
: fimul  ( n0 n1 - n2 )    * #fbits / ;
: fidiv  ( n0 n1 - n2 )    swap #fbits * swap / ;
: i.d>fi ( d d -- fi )     swap i>fi + ;
: f>fi   ( f -- fi )                 \ 1.23
  fdup ftrunc                        \ 1.23 1.00
  fdup frot                          \ 1.00 1.00 1.23
  fswap f-                           \ 1.00 0.23
  #fbits 0 d>f f* f>d drop           \ scale fractional part
  f>d drop #fbits * + ;              \ scale integer part and add
: i3>fi3 ( a b c - a b c ) i>fi rot i>fi rot i>fi rot ;
: fi3>i3 ( a b c - a b c ) fi>i rot fi>i rot fi>i rot ;
: fifloor ( n0 - n1 )      #fbitmask' and ;
: ficeil  ( n0 - n1 )      #fbits/2 + fifloor ;

: fi>f    ( fi -- ) ( F: f -- ) 0 swap d>f 0 #fbits d>f f/ ;
