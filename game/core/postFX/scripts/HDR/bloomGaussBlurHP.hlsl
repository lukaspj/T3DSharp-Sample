//-----------------------------------------------------------------------------
// Copyright (c) 2012 GarageGames, LLC
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------

#include "core/rendering/shaders/postFX/postFx.hlsl"

TORQUE_UNIFORM_SAMPLER2D(inputTex, 0);
uniform float2 oneOverTargetSize;
uniform float gaussMultiplier;
uniform float gaussMean;
uniform float gaussStdDev;

#define PI 3.141592654

float computeGaussianValue( float x, float mean, float std_deviation )
{
    // The gaussian equation is defined as such:
    /*    
      -(x - mean)^2
      -------------
      1.0               2*std_dev^2
      f(x,mean,std_dev) = -------------------- * e^
      sqrt(2*pi*std_dev^2)
      
     */

    float tmp = ( 1.0f / sqrt( 2.0f * PI * std_deviation * std_deviation ) );
    float tmp2 = exp( ( -( ( x - mean ) * ( x - mean ) ) ) / ( 2.0f * std_deviation * std_deviation ) );
    return tmp * tmp2;
}

float SCurve (float x) 
{
		x = x * 2.0 - 1.0;
		return -x * abs(x) * 0.5 + x + 0.5;
}

float4 BlurH (TORQUE_SAMPLER2D(source), float2 size, float2 uv, float radius) 
{
	if (radius >= 1.0)
	{
		float4 A = float4(0.0,0.0,0.0,0.0); 
		float4 C = float4(0.0,0.0,0.0,0.0); 

		float width = 1.0 / size.x;

      float divisor = 0.0; 
      float weight = 0.0;

      float radiusMultiplier = 1.0 / radius;

      // Hardcoded for radius 20 (normally we input the radius
      // in there), needs to be literal here

      for (float x = -20.0; x <= 20.0; x++)
      {
         A = TORQUE_TEX2D(source, uv + float2(x * width, 0.0));

         weight = SCurve(1.0 - (abs(x) * radiusMultiplier)); 

         C += A * weight; 

         divisor += weight; 
      }

		return float4(C.r / divisor, C.g / divisor, C.b / divisor, 1.0);
	}

	return TORQUE_TEX2D(source, uv);
}

float4 main( PFXVertToPix IN ) : TORQUE_TARGET0
{
   float4 color = { 0.0f, 0.0f, 0.0f, 0.0f };
   float offset = 0;
   float weight = 0;
   float x = 0;
   float fI = 0;

   for( int i = 0; i < 9; i++ )
   {
      fI = (float)i;
      offset = (i - 4.0) * oneOverTargetSize.x;
      x = (i - 4.0) / 4.0;
      weight = gaussMultiplier * computeGaussianValue( x, gaussMean, gaussStdDev );
      color += (TORQUE_TEX2D( inputTex, IN.uv0 + float2( offset, 0.0f ) ) * weight );
   }

   //float2 targetSize = 1/oneOverTargetSize;
   //float4 color = BlurH(TORQUE_SAMPLER2D_MAKEARG(inputTex), targetSize, IN.uv0, 20.0);
   
   return float4( color.rgb, 1.0f );
}