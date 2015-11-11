// This is the main DLL file.

#include "stdafx.h"

#include "TreeDetection.h"

#include "stdafx.h"
#include "opencv2/objdetect/objdetect.hpp"
#include "opencv2/highgui/highgui.hpp"
#include "opencv2/imgproc/imgproc.hpp"

#include <iostream>
#include <stdio.h>

using namespace std;
using namespace cv;

/*
This is the function to be called to enter the dll
*/
extern "C" _declspec( dllexport ) void Entry( const char *s )
{

}

/*
From: https://lost-contact.mit.edu/afs/cs.stanford.edu/package/matlab-r2009b/matlab/r2009b/toolbox/images/images/private/libsrc/neighborhood/neighborhood.cpp
ind_to_sub
Convert from linear index to array coordinates.  This is similar
to MATLAB's IND2SUB function, except that here it's zero-based
instead of one-based.  This algorithm used here is adapted from
ind2sub.m.

Inputs
======
p        - zero-based linear index
num_dims - number of dimensions
cumprod  - cumulative product of image dimensions

Output
======
coords   - array of array coordinates
*/
void ind_to_sub( const int &p, const int &num_dims, const int *cumprod, int *coords )
{
  int idx = p;

  for( int j_up = 0; j_up < num_dims; j_up++ )
  {
    int j = num_dims - 1 - j_up;

    coords[j] = idx / cumprod[j];
    idx = idx % cumprod[j];
  }
}

/*
From: https://lost-contact.mit.edu/afs/cs.stanford.edu/package/matlab-r2009b/matlab/r2009b/toolbox/images/images/private/libsrc/neighborhood/neighborhood.cpp
sub_to_ind
Convert from array coordinates to linear index.  This is similar
to MATLAB's SUB2IND function, except that here it's zero-based
instead of one-based.  The algorithm used here is adapted from
sub2ind.m.

Inputs
======
coords    - array of array coordinates
size      - image size (linear index has to be computed with
            respect to a known image size)
cumprod   - cumulative product of image size
num_dims  - number of dimensions

Return
======
zero-based linear index
*/
int sub_to_ind( const int * coords, const int * cumprod, const int & num_dims )
{
  int index = 0;

  for( int k = 0; k < num_dims; k++ )
  {
    index += coords[k] * cumprod[k];
  }

  return index;
}

/*
This function calculates the shannon entropy for each entropy in a specified kernel
This is function is a modified version from this one: http://stackoverflow.com/a/20398494

Inputs
======
gray_src - The gray source image for the calculation to be performed
roi_rect - A rectangle representing the region of interest (area to perform entropy calculations on)

Output
======
local_entropy_image - The finished version will be outputted to this variable
*/
void GetLocalEntroyImage( const IplImage *gray_src, CvRect roi_rect, IplImage*local_entroy_image )
{
  //1.Define nerghbood model,here it's 9*9
  int neighbood_dim = 2;
  int neighbood_size[] = { 9, 9 };

  //2.Pad gray_src
  Mat gray_src_mat = cvarrToMat( gray_src );
  Mat pad_mat;
  int left = ( neighbood_size[0] - 1 ) / 2;
  int right = left;
  int top = ( neighbood_size[1] - 1 ) / 2;
  int bottom = top;
  copyMakeBorder( gray_src_mat, pad_mat, top, bottom, left, right, BORDER_REPLICATE, 0 );
  IplImage *pad_src = &IplImage( pad_mat );

  //Here, implement a histogram by ourself, each bin calcalates the gray value frequency
  int hist_count[256] = { 0 };
  int neighbood_num = 1;
  for( int i = 0; i < neighbood_dim; i++ )
  {
    neighbood_num *= neighbood_size[i];
  }
  //neighbood_corrds_array is a neighbors_num-by-neighbood_dim array containing relative offsets
  int *neighbood_corrds_array = ( int* ) malloc( sizeof( int ) *neighbood_num*neighbood_dim );
  //Contains the cumulative product of the image_size array;used in the sub_to_ind and ind_to_sub calculations.
  int *cumprod;
  cumprod = ( int * ) malloc( neighbood_dim * sizeof( *cumprod ) );
  cumprod[0] = 1;
  for( int i = 1; i < neighbood_dim; i++ ){
    cumprod[i] = cumprod[i - 1] * neighbood_size[i - 1];
  }
  int *image_cumprod = ( int* ) malloc( 2 * sizeof( *image_cumprod ) );
  image_cumprod[0] = 1;
  image_cumprod[1] = pad_src->width;
  //initialize neighbood_corrds_array
  int p;
  int q;
  int *coords;
  for( p = 0; p < neighbood_num; p++ )
  {
    coords = neighbood_corrds_array + p * neighbood_dim;
    ind_to_sub( p, neighbood_dim, cumprod, coords );
    //ind_to_sub( p, neighbood_dim, neighbood_size, cumprod, coords );
    for( q = 0; q < neighbood_dim; q++ )
    {
      coords[q] -= ( neighbood_size[q] - 1 ) / 2;
    }
  }
  //initlalize neighbood_offset in use of neighbood_corrds_array
  int *neighbood_offset = ( int* ) malloc( sizeof( int ) *neighbood_num );
  int *elem;
  for( int i = 0; i < neighbood_num; i++ )
  {
    elem = neighbood_corrds_array + i * neighbood_dim;
    neighbood_offset[i] = sub_to_ind( elem, image_cumprod, 2 );
  }

  //4.calculate entroy for pixel
  uchar *array = ( uchar* ) pad_src->imageData;
  //here,use entroy_table to avoid frequency log function which cost losts of time
  float entroy_table[82];
  const float log2 = log( 2.0f );
  entroy_table[0] = 0.0;
  float frequency = 0;
  for( int i = 1; i < 82; i++ )
  {
    frequency = ( float ) i / 81;
    entroy_table[i] = frequency * ( log( frequency ) / log2 );
  }
  int neighbood_index;
  int max_index = pad_src->width * pad_src->height;
  float temp;
  float entropy;
  int current_index = 0;
  int current_index_in_origin = 0;
  for( int y = roi_rect.y; y < roi_rect.height; y++ )
  {
    current_index = y * pad_src->width;
    current_index_in_origin = ( y - 4 ) * gray_src->width;
    for( int x = roi_rect.x; x < roi_rect.width; x++, current_index++, current_index_in_origin++ )
    {
      for( int j = 0; j < neighbood_num; j++ ){
        int offset = neighbood_offset[j];
        neighbood_index = current_index + neighbood_offset[j];
        hist_count[array[neighbood_index]]++;
      }
      //get entroy
      entropy = 0;
      for( int k = 0; k < 256; k++ )
      {
        if( hist_count[k] != 0 )
        {
          int frequency = hist_count[k];
          entropy -= entroy_table[hist_count[k]];
          hist_count[k] = 0;
        }
      }
      ( ( float* ) local_entroy_image->imageData )[current_index_in_origin] = entropy;
    }
  }
  free( neighbood_corrds_array );
  free( cumprod );
  free( image_cumprod );
  free( neighbood_offset );
}
