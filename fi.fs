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
: f>fi   ( f -- fi )
  fdup f>d drop #fbits * 1.0e fmod #fbits 0 d>f f* f>d drop + ;
: i3>fi3 ( a b c - a b c ) i>fi rot i>fi rot i>fi rot ;
: fi3>i3 ( a b c - a b c ) fi>i rot fi>i rot fi>i rot ;
: fifloor ( n0 - n1 )      #fbitmask' and ;
: ficeil  ( n0 - n1 )      #fbits/2 + fifloor ;

