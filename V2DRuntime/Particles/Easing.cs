using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace V2DRuntime.Particles
{
	// easing equations from the ever brilliant Robert Penner
	public class Easing
	{
		public const float pi = (float)Math.PI;
		public const float twoPi = (float)Math.PI * 2f;
		public const float halfPi = (float)Math.PI / 2f;
		public delegate float EasingFormula(float t, float start, float length);

		// simple linear tweening - no easing
		public static float Linear(float t, float start, float length)
		{
			return length * t + start;
		}


		///////////// QUADRATIC EASING: t^2 ///////////////////

		// quadratic easing in - accelerating from zero velocity
		public static float EaseInQuad(float t, float start, float length)
		{
			return length * (t) * t + start;
		}

		// quadratic easing out - decelerating to zero velocity
		public static float EaseOutQuad(float t, float start, float length)
		{
			return -length * (t) * (t - 2) + start;
		}

		// quadratic easing in/out - acceleration until halfway, then deceleration
		public static float EaseInOutQuad(float t, float start, float length)
		{
			if (t < .5f)
			{
				return length / 2f * t * t + start;
			}
			else
			{
				float t2 = 1f - t;
				return length - length / 2f * t2 * t2 + start;
				//return -length / 2f * ((--t) * (t - 2) - 1) + start;
			}
		}

		public static float EaseInAndBackQuad(float t, float start, float length, params float[] center)
		{
			float c = center.Length > 0 ? center[0] : .5f;
			if (t < c)
			{
				float t2 = t * (1f / c);
				return length * t2 * t2 + start;
			}
			else
			{
				float t2 = (t - c) * (1f / (1f - c));
				return length - length * t2 * t2 + start;
				//return -length / 2f * ((--t) * (t - 2) - 1) + start;
			}
		}

		public static float EaseOutAndBackQuad(float t, float start, float length, params float[] center)
		{
			float c = center.Length > 0 ? center[0] : .5f;
			if (t < c)
			{
				float t2 = t * (1f / c);
				return -length * t2 * (t2 - 2f) + start;
			}
			else
			{
				float rc = 1f - c;
				float t2 = (t - c) * (1f / rc);
				return length + length * t2 * (t2 - 1f - rc) + start;
				//return -length / 2f * ((--t) * (t - 2) - 1) + start;
			}
		}

		///////////// CUBIC EASING: t^3 ///////////////////////

		// cubic easing in - accelerating from zero velocity
		public static float EaseInCubic(float t, float start, float length)
		{
			return length * (t) * t * t + start;
		}

		// cubic easing out - decelerating to zero velocity
		public static float EaseOutCubic(float t, float start, float length)
		{
			return length * ((t = t - 1) * t * t + 1) + start;
		}

		// cubic easing in/out - acceleration until halfway, then deceleration
		public static float EaseInOutCubic(float t, float start, float length)
		{
			if ((t / 2) < 1) return length / 2 * t * t * t + start;
			return length / 2 * ((t -= 2) * t * t + 2) + start;
		}


		///////////// QUARTIC EASING: t^4 /////////////////////

		// quartic easing in - accelerating from zero velocity
		public static float EaseInQuart(float t, float start, float length)
		{
			return length * (t) * t * t * t + start;
		}

		// quartic easing out - decelerating to zero velocity
		public static float EaseOutQuart(float t, float start, float length)
		{
			return -length * ((t = t - 1) * t * t * t - 1) + start;
		}

		// quartic easing in/out - acceleration until halfway, then deceleration
		public static float EaseInOutQuart(float t, float start, float length)
		{
			if ((t / 2) < 1) return length / 2 * t * t * t * t + start;
			return -length / 2 * ((t -= 2) * t * t * t - 2) + start;
		}


		///////////// QUINTIC EASING: t^5  ////////////////////

		// quintic easing in - accelerating from zero velocity
		public static float EaseInQuint(float t, float start, float length)
		{
			return length * (t) * t * t * t * t + start;
		}

		// quintic easing out - decelerating to zero velocity
		public static float EaseOutQuint(float t, float start, float length)
		{
			return length * ((t = t - 1) * t * t * t * t + 1) + start;
		}

		// quintic easing in/out - acceleration until halfway, then deceleration
		public static float EaseInOutQuint(float t, float start, float length)
		{
			if ((t / 2) < 1) return length / 2 * t * t * t * t * t + start;
			return length / 2 * ((t -= 2) * t * t * t * t + 2) + start;
		}


		public static float Sin(float t, float min, float max)
		{
			return (float)Math.Sin(t * twoPi) * (max - min) + min;
		}
		public static float Cos(float t, float min, float max)
		{
			return (float)Math.Cos(t * twoPi) * (max - min) + min;
		}
		public static float Sin(float t, float min, float max, float period)
		{
			return (float)Math.Sin(t * twoPi * period) * (max - min) + min;
		}
		public static float Cos(float t, float min, float max, float period)
		{
			return (float)Math.Cos(t * twoPi * period) * (max - min) + min;
		}

		///////////// SINUSOIDAL EASING: sin(t) ///////////////

		// sinusoidal easing in - accelerating from zero velocity
		public static float EaseInSine(float t, float start, float length)
		{
			return -length * (float)Math.Cos(pi / 2) + length + start;
		}

		// sinusoidal easing out - decelerating to zero velocity
		public static float EaseOutSine(float t, float start, float length)
		{
			return length * (float)Math.Sin(pi / 2) + start;
		}

		// sinusoidal easing in/out - accelerating until halfway, then decelerating
		public static float EaseInOutSine(float t, float start, float length)
		{
			return -length / 2 * ((float)Math.Cos(pi * t) - 1) + start;
		}


		///////////// EXPONENTIAL EASING: 2^t /////////////////

		// exponential easing in - accelerating from zero velocity
		public static float EaseInExpo(float t, float start, float length)
		{
			return (t == 0) ? start : length * (float)Math.Pow(2, 10 * (t - 1)) + start;
		}

		// exponential easing out - decelerating to zero velocity
		public static float EaseOutExpo(float t, float start, float length)
		{
			return (t == 1) ? start + length : length * (-(float)Math.Pow(2, -10 * t) + 1) + start;
		}

		// exponential easing in/out - accelerating until halfway, then decelerating
		public static float EaseInOutExpo(float t, float start, float length)
		{
			if (t == 0) return start;
			if (t == 1) return start + length;
			if ((t / 2) < 1) return length / 2 * (float)Math.Pow(2, 10 * (t - 1)) + start;
			return length / 2 * (-(float)Math.Pow(2, -10 * --t) + 2) + start;
		}


		/////////// CIRCULAR EASING: sqrt(1-t^2) //////////////

		// circular easing in - accelerating from zero velocity
		public static float EaseInCirc(float t, float start, float length)
		{
			return -length * ((float)Math.Sqrt(1 - (t) * t) - 1) + start;
		}

		// circular easing out - decelerating to zero velocity
		public static float EaseOutCirc(float t, float start, float length)
		{
			return length * (float)Math.Sqrt(1 - (t = t - 1) * t) + start;
		}

		// circular easing in/out - acceleration until halfway, then deceleration
		public static float EaseInOutCirc(float t, float start, float length)
		{
			if ((t / 2) < 1) return -length / 2 * ((float)Math.Sqrt(1 - t * t) - 1) + start;
			return length / 2 * ((float)Math.Sqrt(1 - (t -= 2) * t) + 1) + start;
		}

		 /////////// ELASTIC EASING: exponentially decaying sine wave  //////////////

		/// <summary>
		/// EaseInElastic
		/// </summary>
		/// <param name="t">current time</param>
		/// <param name="start">beginning value</param>
		/// <param name="length">change in value</param>
		/// <param name="duration">duration</param>
		/// <param name="a">amplitude (optional)</param>
		/// <param name="p">period (optional)</param>
		/// <returns>equation result</returns>
		public static float EaseInElastic(float t, float start, float length, params float[] ampl_period)
		{
			if (t == 0) return start;
			if ((t) == 1) return start + length;
			float p = ampl_period.Length > 1 ? ampl_period[1] : .3f;
			float a = ampl_period.Length > 0 ? ampl_period[0] : 0;
			float s;
			if (a < Math.Abs(length))
			{
				a = length;
				s = p / 4;
			}
			else
			{
				s = p / (2 * pi) * (float)Math.Asin(length / a);
			}
			return -(a * (float)Math.Pow(2, 10 * (t -= 1)) * (float)Math.Sin((t - s) * (2 * pi) / p)) + start;
		}

		/// <summary>
		/// EaseOutElastic
		/// </summary>
		/// <param name="t">current time</param>
		/// <param name="start">beginning value</param>
		/// <param name="length">change in value</param>
		/// <param name="duration">duration</param>
		/// <param name="a">amplitude (optional)</param>
		/// <param name="p">period (optional)</param>
		/// <returns>equation result</returns>
		public static float EaseOutElastic(float t, float start, float length, params float[] ampl_period)
		{
			if (t == 0) return start;
			if ((t) == 1) return start + length;
			float p = ampl_period.Length > 1 ? ampl_period[1] :  .3f;
			float a = ampl_period.Length > 0 ? ampl_period[0] : 0;
			float s;
			if (a < (float)Math.Abs(length))
			{
				a = length;
				s = p / 4;
			}
			else
			{
				s = p / (2 * pi) * (float)Math.Asin(length / a);
			}
			return a * (float)Math.Pow(2, -10 * t) * (float)Math.Sin((t - s) * (2 * pi) / p) + length + start;
		}

		/// <summary>
		/// EaseInOutElastic
		/// </summary>
		/// <param name="t">current time</param>
		/// <param name="start">beginning value</param>
		/// <param name="length">change in value</param>
		/// <param name="duration">duration</param>
		/// <param name="a">amplitude (optional)</param>
		/// <param name="p">period (optional)</param>
		/// <returns>equation result</returns>
		public static float EaseInOutElastic(float t, float start, float length, params float[] ampl_period)
		{
			if (t == 0) return start;
			if ((t / 2) == 2) return start + length;
			float p = ampl_period.Length > 1 ? ampl_period[1] : .3f * 1.5f;
			float a = ampl_period.Length > 0 ? ampl_period[0] : 0;
			float s;
			if (a < (float)Math.Abs(length))
			{
				a = length;
				s = p / 4;
			}
			else
			{
				s = p / (2 * pi) * (float)Math.Asin(length / a);
			}
			if (t < 1)
			{
				return -.5f * (float)(a * Math.Pow(2, 10 * (t -= 1)) * Math.Sin((t - s) * (2 * Math.PI) / p)) + start;
			}
			return a * (float)Math.Pow(2, -10 * (t -= 1)) * (float)Math.Sin((t - s) * (2 * pi) / p) * .5f + length + start;
		}


		 /////////// BACK EASING: overshooting cubic easing: (s+1)*t^3 - s*t^2  //////////////

		// back easing in - backtracking slightly, then reversing direction and moving to target
		public static float EaseInBack(float t, float start, float length, params float[] sval) 
		{
			float s = sval.Length == 0 ? 1.70158f : sval[0];
			return length*t*t*((s+1)*t - s) + start;
		}

		// back easing out - moving towards target, overshooting it slightly, then reversing and coming back to target
		public static float EaseOutBack(float t, float start, float length, params float[] sval)
		{
			float s = sval.Length == 0 ? 1.70158f : sval[0];
			return length*((t=t-1)*t*((s+1)*t + s) + 1) + start;
		}

		// back easing in/out - backtracking slightly, then reversing direction and moving to target,
		public static float EaseInOutBack(float t, float start, float length, params float[] sval)
		{
			float s = sval.Length == 0 ? 1.70158f : sval[0];
			if ((t / 2) < 1)
			{
				return length / 2 * (t * t * (((s *= (1.525f)) + 1) * t - s)) + start;
			}
			return length / 2 * ((t -= 2) * t * (((s *= (1.525f)) + 1) * t + s) + 2) + start;
		}
		

		 /////////// BOUNCE EASING: exponentially decaying parabolic bounce  //////////////

		// bounce easing in
		public static float EaseInBounce(float t, float start, float length)
		{
			return length - EaseOutBounce(1 - t, 0, length) + start;
		}

		// bounce easing out
		public static float EaseOutBounce(float t, float start, float length)
		{
			if ((t) < (1 / 2.75f))
			{
				return length * (7.5625f * t * t) + start;
			}
			else if (t < (2 / 2.75))
			{
				return length * (7.5625f * (t -= (1.5f / 2.75f)) * t + .75f) + start;
			}
			else if (t < (2.5 / 2.75))
			{
				return length * (7.5625f * (t -= (2.25f / 2.75f)) * t + .9375f) + start;
			}
			else
			{
				return length * (7.5625f * (t -= (2.625f / 2.75f)) * t + .984375f) + start;
			}
		}

		// bounce easing in/out
		public static float EaseInOutBounce(float t, float start, float length)
		{
			if (t < .5f)
			{
				return EaseInBounce(t * 2f, 0, length) * .5f + start;
			}
			return EaseOutBounce(t, 0, length) * .5f + length * .5f + start;
		}

	}
}
