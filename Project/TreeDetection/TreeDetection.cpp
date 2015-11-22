// This is the main DLL file.
#include "stdafx.h"
#ifdef RunPerformanceTest
#include <windows.h>
#endif

#include "TreeDetection.h"

#include "stdafx.h"
#include "opencv2/objdetect/objdetect.hpp"
#include "opencv2/highgui/highgui.hpp"
#include "opencv2/imgproc/imgproc.hpp"

#include <iostream>
#include <stdio.h>
#include <map>
#include <fstream>

using namespace std;
using namespace cv;

void GetEntropyImage( const Mat *graySrc, int neighborhoodSize, Mat *entropyImage, bool bDoNaive = false );

vector<float> EntropyFunctionCache;

int CacheBaseSize = -1;

#ifdef RunPerformanceTest
/*
This is the function can be used to a run a performacne test on
the algorithim

Inputs
======
gridSize - size of the square grid to be generated in with testing
bUseNaive - toggles using naive algorithim or the optimized one

Output
======
ReturnValue - returns the microseconds it took to execute the test
*/
LONGLONG PerfTest(unsigned int gridSize, bool bUseNaive )
{
  LARGE_INTEGER startTime,
                endTime,
                elapsedMicroseconds,
                tickFreq;

  Mat testMat = Mat::zeros( gridSize, gridSize, CV_8U ),
      result;

  //
  // Want the same matrix for each grid size for reproduceability
  //
  srand( 0 );

  for( int x = 0; x < gridSize; x++ )
  {
    for( int y = 0; y < gridSize; y++ )
    {
      testMat.at<uchar>( y, x ) = rand() % 256;
    }
  }
  QueryPerformanceFrequency( &tickFreq );

  QueryPerformanceCounter( &startTime );
  GetEntropyImage( &testMat, 9, &result, bUseNaive );
  QueryPerformanceCounter( &endTime );

  elapsedMicroseconds.QuadPart = endTime.QuadPart - startTime.QuadPart;
  elapsedMicroseconds.QuadPart *= 1000000;
  elapsedMicroseconds.QuadPart /= tickFreq.QuadPart;

  return elapsedMicroseconds.QuadPart;
}
#endif

/*
This is the function to be called to enter the dll

Inputs
======
FilePath - The file for the entropy function to be calculated on

Output
======
*/
extern "C" _declspec( dllexport ) void Entry( const char *filepath )
{
#ifndef RunPerformanceTest
  Mat image = imread( filepath ),
      labImage,
      luminosityImage,
      result;

  std::vector<cv::Mat> channels;

  string outputPath = filepath;
  outputPath = "entropy_" + outputPath;

  //
  // Converts image from RGB to LAB color space, dropping the A and B channels,
  // leaving just the L channel
  //
  cvtColor( image, labImage, CV_RGB2Lab, 0 );
  split( labImage, channels );
  luminosityImage = channels[0];

  GetEntropyImage( &luminosityImage, 9, &result );
  imshow( "derp", result );
#else
  Code to test performance
  int base = 1;
  ofstream myfile;
  myfile.open( "A:\perf.txt" );
  while( base <= 128 )
  {
    LONGLONG resultOpt = PerfTest( base, false );
    LONGLONG resultNaive = PerfTest( base, true );
    myfile << base << "," << resultNaive << "," << resultOpt << "\n";
    base++;
  }
  myfile.close( );*/
#endif
}

/*
This function gets the value at the location of a matrix, it allows for automatic padding of data
Inputs
======
src - The src matrix for the function to be performed on
x - The x coordinates of the pixel
y - The y coordinates of the pixel

Output
======
Return Value - the value of the pixel at the specified location
*/
int GetMatPixelVal( const Mat* src, int x, int y )
{
  int val = src->at<uchar>( 
    borderInterpolate( y, src->rows, BORDER_REFLECT ),
    borderInterpolate( x, src->cols, BORDER_REFLECT )
    );

  return val;
}

/*
This function does the entropy calculation for a given occurence.
It uses a cached value when possible, since the operation is expensive to perform.
Inputs
======
timesOccured     - The number of times this luminosity has been seen for a key pixel
numberOfElements - The size of the neighborhood for the calculation

Output
======
Return Value     - The result of the entropy calculation
*/
float entropyCalculation( int timesOccured, int numberOfElements )
{
  float returnValue,
        frequency;

  //
  // If the size of the neighborhood we are caching changes
  // then we need to empty and recreate the cache
  //
  if( CacheBaseSize != numberOfElements )
  {
    CacheBaseSize = numberOfElements;
    EntropyFunctionCache.clear();
    EntropyFunctionCache.insert( EntropyFunctionCache.end(), numberOfElements + 1, ( float ) -1 );
  }
  if( EntropyFunctionCache[timesOccured] == -1  )
  {
    frequency = ( float ) timesOccured / ( float ) numberOfElements;
    returnValue = frequency * log2f( frequency );
    EntropyFunctionCache[timesOccured] = returnValue;
  }
  else
  {
    returnValue = EntropyFunctionCache[timesOccured];
  }
  return returnValue;
}

/*
This function calculates the shannon entropy for an image with a given kernel size
Inputs
======
graySrc          - The gray source image for the calculation to be performed
neighborhoodSize - An integer representing the size of the grid that will be used for sampling each pixels entropy
                   This value must be positive and odd, and an exception will be thrown if it is not.
bDoNaive         - Toggle to run the naive unoptimized version of this algorithim

Output
======
entropyImage     - Contains a grayscale image representing the entropy of each pixel
*/
void GetEntropyImage( const Mat *graySrc, int neighborhoodSize, Mat *entropyImage, bool bDoNaive )
{
  //
  // 256 comes from 0-255 aka the possible values for a pixel in a grayscale image
  //
  const int NUMBER_OF_BUCKETS = 256;

  int lumionisity,
      buckets[NUMBER_OF_BUCKETS] = { 0 };
  
  float range[] = { 0, 255 },
        pixelFuncVal,
        summation,
        frequency;

  Mat hist,
      entropyMat;

  if( neighborhoodSize <= 0 || !( neighborhoodSize % 2 ) )
  {
    throw invalid_argument("neighborhoodSize");
  }

  //
  // Creates empyty matrix to assign entropy values to
  //
  entropyMat = Mat::zeros( graySrc->rows, graySrc->cols, CV_32F );
  *entropyImage = Mat::zeros( graySrc->rows, graySrc->cols, CV_8UC1 );

  //
  // The following values are used here as:
  // x,y - The current pixel being evaluated where x is the column and y is the row (aka the key pixel)
  // j,q - Used to iterate through a grid around the key pixel, j is the column q is the row
  //
  for( int y = 0; y < graySrc->rows; y++ )
  {
    if( !bDoNaive )
    {
      //
      // If we are not in naive mode we need to manually reset the value of the different buckets
      // when we move from on row to the next
      //
      memset( buckets, 0, sizeof( buckets ) );
    }
    for( int x = 0; x < graySrc->cols; x++ )
    {
      summation = 0;
      if( bDoNaive || x == 0 )
      {
        //
        // This gets the entropy value for all surrounding pixels optimized version uses memoization to
        // improve on this, but it needs to happen for the very first column for each row
        //
        for( long q = y - neighborhoodSize / 2; q <= y + neighborhoodSize / 2; q++ )
        {
          for( long j = x - neighborhoodSize / 2; j <= x + neighborhoodSize / 2; j++ )
          {
            //
            // Here we grab the pixels lumionisity value. 
            // The bin corresponding to this value is incremented.
            // The end result is knowing the number of pixels with a certain value, the number corresponds 
            // with the bin sizes.
            //
            lumionisity = GetMatPixelVal( graySrc, j, q );
            buckets[lumionisity]++;
          }
        }
      }
      else
      {
        long columnSub = x - neighborhoodSize / 2 - 1;
        long columnAdd = x + neighborhoodSize / 2;
        for( long q = y - neighborhoodSize / 2; q <= y + neighborhoodSize / 2; q++ )
        {
          lumionisity = GetMatPixelVal( graySrc, columnSub, q );
          buckets[lumionisity]--;
        }
        for( long q = y - neighborhoodSize / 2; q <= y + neighborhoodSize / 2; q++ )
        {
          lumionisity = GetMatPixelVal( graySrc, columnAdd, q );
          buckets[lumionisity]++;
        }
      }
      for( int ndx = 0; ndx < NUMBER_OF_BUCKETS; ndx++ )
      {
        //
        // Buckets which have no associated values will not influence the value of the calculation
        // and therefore can be skipped.
        //
        if( buckets[ndx] != 0 )
        {
          if( bDoNaive )
          {
            frequency = ( float ) buckets[ndx] / ( float ) ( neighborhoodSize * neighborhoodSize );
            summation -= frequency * log2f( frequency );
          }
          else
          {
            summation -= entropyCalculation( buckets[ndx], ( neighborhoodSize * neighborhoodSize ) );
          }
          if( bDoNaive )
          {
            //
            // Reset the number of pixels that fall in this bucket for the future key pixels
            //
            buckets[ndx] = 0;
          }
        }
      }
      entropyMat.at<float>( y, x ) = summation;
    }
  }
  //
  // The entropy values need to be normalized so the lowest value becomes 0
  // and the highest value is 255. This allows for display of images entropy.
  //
  normalize( entropyMat, *entropyImage, 0, 255, NORM_MINMAX, CV_8UC1 );
}
