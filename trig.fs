: gen-trigtable ( xt -- )
  360 0 do
    dup i 0 d>f pi f* 180e f/ execute f>fi ,
  loop drop
;

create sintable ' fsin gen-trigtable
create costable ' fcos gen-trigtable
