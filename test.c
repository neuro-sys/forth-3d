#include <stdio.h>
#include <stdint.h>

typedef int32_t fixed;

// Square root of integer
unsigned long int_sqrt ( unsigned long s )
{
	unsigned long x0 = s >> 1;				// Initial estimate

	// Sanity check
	if ( x0 )
	{
		unsigned long x1 = ( x0 + s / x0 ) >> 1;	// Update


		while ( x1 < x0 )				// This also checks for cycle
		{
			x0 = x1;
			x1 = ( x0 + s / x0 ) >> 1;
		}
		
		return x0;
	}
	else
	{
		return s;
	}
}

fixed sqrtF2F ( fixed x )
{
    uint32_t t, q, b, r;
    r = x;
    b = 0x40000000;
    q = 0;
    while( b > 0x40 )
    {
        t = q + b;
        if( r >= t )
        {
            r -= t;
            q = t + b; // equivalent to q += 2*b
        }
        r <<= 1;
        b >>= 1;
    }
    q >>= 8;
    return q;
}

#define FP (1 << 16)

typedef unsigned int fp_t;

int main()
{
    fp_t x0 = 4 * FP + (56.0 / 100.0) * (float) FP;

    fp_t res1 = int_sqrt(x0 * FP);
    /* fp_t res2 = sqrtF2F(x0); */

    printf("%f %f\n", (float) res1 / FP);


    return 0;
}
