#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include <string.h>

#define ROWS 20
#define COLS 80

int buffer[ROWS][COLS];

void setPixel(int x, int y)
{
    buffer[y][x] = '#';
}

void line(int x0, int y0, int x1, int y1) {
 
  int dx = abs(x1-x0), sx = x0<x1 ? 1 : -1;
  int dy = abs(y1-y0), sy = y0<y1 ? 1 : -1; 
  int err = (dx>dy ? dx : -dy)/2, e2;
 
  for(;;){
    setPixel(x0,y0);
    if (x0==x1 && y0==y1) break;
    e2 = err;
    if (e2 >-dx) { err -= dy; x0 += sx; }
    if (e2 < dy) { err += dx; y0 += sy; }
  }
}

void flip()
{
    int y, x;

    for (y = 0; y < ROWS; y++) {
        for (x = 0; x < COLS; x++) {
            putchar(buffer[y][x]);
        }
        putchar('\n');
    }
}

int main()
{
    memset(buffer, ' ', sizeof(buffer));

    line(3, 3, 9, 7);

    flip();

    return 0;
}
