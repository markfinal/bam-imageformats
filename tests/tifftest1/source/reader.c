/*
Copyright (c) 2010-2016, Mark Final
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this
  list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice,
  this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution.

* Neither the name of BuildAMation nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#include "reader.h"
#include "checkerboard.h"

#include "tiffio.h"

#include <string.h>
#include <stdlib.h>

int
TestReader()
{
    int width = 0;
    int height = 0;
    int samplesperpixel = 0;
    TIFF *tif = TIFFOpen("new.tif", "r");
    if (NULL == tif)
    {
        return -1;
    }
    TIFFGetField(tif, TIFFTAG_IMAGEWIDTH, &width);
    TIFFGetField(tif, TIFFTAG_IMAGELENGTH, &height);
    TIFFGetField(tif, TIFFTAG_SAMPLESPERPIXEL, &samplesperpixel);

    uint32 *image = _TIFFmalloc(width * height * samplesperpixel);
    if (NULL == image)
    {
        return -1;
    }

    if (1 != TIFFReadRGBAImage(tif, width, height, image, 0))
    {
        return -2;
    }

    char *compare = _TIFFmalloc(width * height * samplesperpixel);
    if (NULL == compare)
    {
        return -1;
    }
    createCheckerboardImage(compare, width, height, samplesperpixel);
    if (0 != memcmp(image, compare, width * height * samplesperpixel))
    {
        return -2;
    }

    free(compare);
    free(image);
    TIFFClose(tif);
    return 0;
}
