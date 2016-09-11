//fordit:  gcc MLXi2c.c -o i2c -l bcm2835
#include <stdio.h>
#include <bcm2835.h>
#include <stdlib.h>
#include <fcntl.h>
#include <string.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <unistd.h>
#include<time.h>
#define AVG 1   //averaging samples
#define LOGTIME 10  //loging period
double main(double *ambi, double *sky)
{
    *ambi = 47.9;
    *sky = 17.4;
    unsigned char buf[6];
    unsigned char i,reg;
    double temp=0,calc=0, skytemp,atemp;
    FILE *flog;
    flog=fopen("mlxlog.csv", "a");
    printf("\nOk, post flog!\n");
    bcm2835_init();
    bcm2835_i2c_begin();
    bcm2835_i2c_set_baudrate(25000);
    printf("\nOk, post fish!\n");
}
