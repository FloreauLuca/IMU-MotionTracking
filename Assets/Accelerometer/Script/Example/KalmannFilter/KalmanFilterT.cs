//using System.Collections.Generic;

///// <summary>A Kalman filter implementation for any type of value which can added and multiplied.</summary>
///// <remarks>
///// Determining whether the type can be added and multiplied occurs at runtime via the <c>dynamic</c> keyword.
///// Note that if you use this with the incorrect data type (such as a <c>Quaternion</c>, which cannot be added),
///// the error will occur at runtime.
/////
///// <c>dynamic</c> also incurs a runtime cost, so if performance is crucial, it is suggested a concrete Kalman
///// filter implementation be used such as <c>KalmanFilterFloat</c> or <c>KalmanFilterVector3</c>.
///// </remarks>
//public class KalmanFilter<T> {

//	//-----------------------------------------------------------------------------------------
//	// Constants:
//	//-----------------------------------------------------------------------------------------

//	public const float DEFAULT_Q = 0.000001f;
//	public const float DEFAULT_R = 0.01f;

//	public const float DEFAULT_P = 1;

//	//-----------------------------------------------------------------------------------------
//	// Private Fields:
//	//-----------------------------------------------------------------------------------------

//	private float q;
//	private float r;
//	private float p = DEFAULT_P;
//	private T x;
//	private float k;

//	//-----------------------------------------------------------------------------------------
//	// Constructors:
//	//-----------------------------------------------------------------------------------------

//	// N.B. passing in DEFAULT_Q is necessary, even though we have the same value (as an optional parameter), because this
//	// defines a parameterless constructor, allowing us to be new()'d in generics contexts.
//	public KalmanFilter() : this(DEFAULT_Q) { }

//	public KalmanFilter(float aQ = DEFAULT_Q, float aR = DEFAULT_R) {
//		q = aQ;
//		r = aR;
//	}

//	//-----------------------------------------------------------------------------------------
//	// Public Methods:
//	//-----------------------------------------------------------------------------------------

//	public T Update(T measurement, float? newQ = null, float? newR = null) {

//		// update values if supplied.
//		if (newQ != null && q != newQ) {
//			q = (float)newQ;
//		}
//		if (newR != null && r != newR) {
//			r = (float)newR;
//		}

//		// update measurement.
//		{
//			k = (p + q) / (p + q + r);
//			p = r * (p + q) / (r + p + q);
//		}

//		// filter result back into calculation.
//		T dynamicMeasurement = measurement;
//        T result = x + (dynamicMeasurement - x) * k;
//		x = result;
//		return result;
//	}

//	public T Update(List<T> measurements, bool areMeasurementsNewestFirst = false, float? newQ = null, float? newR = null) {

//		T result = default(T);
//		int i = (areMeasurementsNewestFirst) ? measurements.Count - 1 : 0;

//		while (i < measurements.Count && i >= 0) {

//			// decrement or increment the counter.
//			if (areMeasurementsNewestFirst) {
//				--i;
//			}
//			else {
//				++i;
//			}

//			result = Update(measurements[i], newQ, newR);
//		}

//		return result;
//	}

//	public void Reset() {
//		p = 1;
//		x = default(T);
//		k = 0;
//	}
//}