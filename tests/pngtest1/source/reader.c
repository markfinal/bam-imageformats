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
#include "reader.h"
#include "checkerboard.h"

#include "png.h"

#include <string.h>
#include <stdlib.h>
#include <assert.h>

int
TestReader()
{
    png_structp png_ptr;
    png_infop info_ptr;
    int width;
    int height;
    int color_type;
    int bit_depth;
    png_bytep *row_pointers;
    int y;

    FILE *fp = fopen("new.png", "rb");

    unsigned char header[8];
    if (8 != fread(header, 1, 8, fp))
    {
        fclose(fp);
        return -1;
    }
    if (png_sig_cmp(header, 0, 8) > 0)
    {
        fclose(fp);
        return -1;
    }

    png_ptr = png_create_read_struct(PNG_LIBPNG_VER_STRING, NULL, NULL, NULL);
    info_ptr = png_create_info_struct(png_ptr);

    png_init_io(png_ptr, fp);
    png_set_sig_bytes(png_ptr, 8);
    png_read_info(png_ptr, info_ptr);

    width = png_get_image_width(png_ptr, info_ptr);
    height = png_get_image_height(png_ptr, info_ptr);
    color_type = png_get_color_type(png_ptr, info_ptr);
    bit_depth = png_get_bit_depth(png_ptr, info_ptr);

    png_read_update_info(png_ptr, info_ptr);

    row_pointers = (png_bytep*)malloc(sizeof(png_bytep) * height);

    {
        for (y = 0; y < height; ++y)
        {
            row_pointers[y] = (png_byte*)malloc(png_get_rowbytes(png_ptr, info_ptr));
        }

        png_read_image(png_ptr, row_pointers);
    }

    fclose(fp);

    {
        char *compare;
        const int rowLength = (width * bit_depth * 4) / 8;
        assert(PNG_COLOR_TYPE_RGBA == color_type);
        compare = malloc(width * height * bit_depth * 4);
        if (NULL == compare)
        {
            return -1;
        }
        createCheckerboardImage(compare, width, height);
        for (y = 0; y < height; ++y)
        {
            if (0 != memcmp(row_pointers[y], compare + y * rowLength, rowLength))
            {
                return -2;
            }
        }
        free(compare);
    }

    for (y = 0; y < height; ++y)
    {
        free(row_pointers[y]);
    }
    free(row_pointers);
    return 0;
}
