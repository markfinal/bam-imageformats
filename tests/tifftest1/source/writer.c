/*
Copyright (c) 2010-2019, Mark Final
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
#include "writer.h"
#include "checkerboard.h"

#include "tiffio.h"

#include <stdlib.h>
#include <string.h>

int
TestWriter()
{
    int width = 128;
    int height = 128;
    int samplesperpixel = 4;
    char *image = malloc(width * height * samplesperpixel);
    if (NULL == image)
    {
        return -1;
    }
    size_t linebytes = samplesperpixel * width;
    unsigned char *buf = NULL;
    TIFF *tif = TIFFOpen("new.tif", "w");
    if (NULL == tif)
    {
        return -1;
    }
    TIFFSetField(tif, TIFFTAG_IMAGEWIDTH, width);
    TIFFSetField(tif, TIFFTAG_IMAGELENGTH, height);
    TIFFSetField(tif, TIFFTAG_SAMPLESPERPIXEL, samplesperpixel);
    TIFFSetField(tif, TIFFTAG_BITSPERSAMPLE, 8);
    TIFFSetField(tif, TIFFTAG_ORIENTATION, ORIENTATION_TOPLEFT);
    TIFFSetField(tif, TIFFTAG_PLANARCONFIG, PLANARCONFIG_CONTIG);
    TIFFSetField(tif, TIFFTAG_PHOTOMETRIC, PHOTOMETRIC_RGB);
    if (TIFFScanlineSize(tif) == linebytes)
    {
        buf = _TIFFmalloc(linebytes);
    }
    else
    {
        buf = _TIFFmalloc(TIFFScanlineSize(tif));
    }
    if (NULL == buf)
    {
        return -1;
    }
    TIFFSetField(tif, TIFFTAG_ROWSPERSTRIP, TIFFDefaultStripSize(tif, linebytes));
    createCheckerboardImage(image, width, height, samplesperpixel);
    unsigned int row;
    for (row = 0; row < height; ++row)
    {
        memcpy(buf, &image[(height - row - 1) * linebytes], linebytes);
        if (TIFFWriteScanline(tif, buf, row, 0) < 0)
        {
            break;
        }
    }
    TIFFClose(tif);
    _TIFFfree(buf);
    free(image);
    return 0;
}
