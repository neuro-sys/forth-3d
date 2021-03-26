require fi.fs

0 \ vector3
dup constant v.x 1 cells +
dup constant v.y 1 cells +
dup constant v.z 1 cells +
constant vector3

: v.x v.x + ;
: v.y v.y + ;
: v.z v.z + ;

: v.x@ v.x @ ;
: v.y@ v.y @ ;
: v.z@ v.z @ ;

create v0 vector3 allot
create v1 vector3 allot
create v2 vector3 allot

: v! ( x y z addr -- ) dup >r v.z ! r@ v.y ! r> ! ;
: v@ ( v -- x y z )    dup @ swap dup v.y@ swap v.z@ ;

: vadd ( v0 v1 -- v2 )
  v1 v! v0 v!

  v0 v.x@ v1 v.x@ +
  v0 v.y@ v1 v.y@ +
  v0 v.z@ v1 v.z@ +
;

: vsub ( v0 v1 -- v2 )
  v1 v! v0 v!

  v0 v.x@ v1 v.x@ -
  v0 v.y@ v1 v.y@ -
  v0 v.z@ v1 v.z@ -
;

: vmul ( v0 v1 -- v2 )
  v1 v! v0 v!

  v0 v.x@ v1 v.x@ fimul
  v0 v.y@ v1 v.y@ fimul
  v0 v.z@ v1 v.z@ fimul
;

: vdiv ( v0 v1 -- v2 )
  v1 v! v0 v!

  v0 v.x@ v1 v.x@ fidiv
  v0 v.y@ v1 v.y@ fidiv
  v0 v.z@ v1 v.z@ fidiv
;

\ https://www.fpgarelated.com/showarticle/1347.php
\ TODO: cheat with FPU, fix this!
: vlength ( v0 -- n )
  v0 v!

  v0 v.x@ fi>f fdup f*
  v0 v.y@ fi>f fdup f* 
  v0 v.z@ fi>f fdup f* f+ f+ fsqrt f>fi

  \ v0 v.x@ dup fimul
  \ v0 v.y@ dup fimul
  \ v0 v.z@ dup fimul + +
;

: vdot ( v0 v1 -- n )
  v1 v! v0 v!

  v0 v.x@ v1 v.x@ fimul
  v0 v.y@ v1 v.y@ fimul
  v0 v.z@ v1 v.z@ fimul

  + +
;

: vcross ( v0 v1 -- v2 )
  v1 v! v0 v!

  v0 v.y@ v1 v.z@ fimul
  v0 v.z@ v1 v.y@ fimul -

  v0 v.z@ v1 v.x@ fimul
  v0 v.x@ v1 v.z@ fimul -

  v0 v.x@ v1 v.y@ fimul
  v0 v.y@ v1 v.x@ fimul -
;

: vnormal ( v0 v1 v2 -- v3 )
  v2 v! v1 v! v0 v!

  v1 v@ v0 v@ vsub
  v2 v@ v0 v@ vsub
  vcross
;

: vnormalize ( v0 -- v1 )
  v0 v!
  v0 v@
  v0 v@ vlength dup dup
  vdiv
;

\ 1 i>fi 2 i>fi 3 i>fi
\ 1 i>fi 5 i>fi 7 i>fi vdot fi>i .s

\ \ 1 i>fi 2 i>fi 3 i>fi .s vnormalize .s
\ bye

\ 0e 16384e f/
\ 0e 16384e f/
\ -16384e 16384e f/

\ 54e 16384e f/
\ 4879e 16384e f/
\ -16384e 16384e f/

\ f.s
\ bye
