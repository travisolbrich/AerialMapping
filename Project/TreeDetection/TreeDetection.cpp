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

/** Function Headers */
void detectAndDisplay( Mat frame );

/** Global variables */
string tree_cascade_name = "treecascade.xml";
CascadeClassifier tree_cascade;
string window_name = "Capture - Tree Detection";
RNG rng( 12345 );

extern "C" _declspec( dllexport ) void entry( const char *s )
{

  if( !tree_cascade.load( tree_cascade_name ) ){ printf( "--(!)Error loading\n" ); return; };
  Mat frame = imread( s, 1 );

  do
  {
    if( !frame.empty() )
    {
      detectAndDisplay( frame );
    }
    else
    {
      printf( " --(!) No captured frame -- Break!" ); break;
    }

    int c = waitKey( 10 );
    if( ( char ) c == 'c' ) { break; }
  } while( false );
  return;
}

void detectAndDisplay( Mat frame )
{
  std::vector<Rect> faces;
  Mat frame_gray;

  cvtColor( frame, frame_gray, CV_BGR2GRAY );
  equalizeHist( frame_gray, frame_gray );

  //-- Detect faces
  tree_cascade.detectMultiScale( frame_gray, faces, 1.2, 4, 0 | CV_HAAR_SCALE_IMAGE, Size( 10, 60 ) );
  for( size_t i = 0; i < faces.size(); i++ )
  {
    int x = faces[i].x;
    int y = faces[i].y;
    int w = faces[i].width;
    int h = faces[i].height;
    rectangle( frame, Point( x, y ), Point( x + w, y + h ), Scalar( 255, 0, 255 ) );
  }


  //imwrite( "A:\\test.jpg", frame );
  imshow( "Image", frame );
}