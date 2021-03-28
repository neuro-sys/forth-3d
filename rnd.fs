[undefined] rnd.fs [if]

vocabulary rnd.fs also rnd.fs definitions

create xs 1 ,
: rnd ( n -- u )
  xs @
  dup 7 lshift xor
  dup 9 rshift xor
  dup 8 lshift xor
  dup xs !
  abs swap mod
;

previous definitions

[then]
