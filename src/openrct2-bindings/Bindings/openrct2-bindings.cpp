#include <stdio.h>
#include <cstdarg>

#include "openrct2-bindings.h"


// Write to output log with a custom prefix and line ending.
void dll_log(const char* format, ...)
{
    printf("(me) ");

    va_list args;
    va_start(args, format);
    vprintf(format, args);
    va_end(args);

    printf("\n");
}
