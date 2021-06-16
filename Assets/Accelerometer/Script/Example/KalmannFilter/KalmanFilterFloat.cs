using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
/// <summary>A Kalman filter implementation for <c>float</c> values.</summary>
public class KalmanFilterFloat {

	//-----------------------------------------------------------------------------------------
	// Constants:
	//-----------------------------------------------------------------------------------------

	public const float DEFAULT_Q = 0.000001f;
	public const float DEFAULT_R = 0.01f;

	public const float DEFAULT_P = 1;

	//-----------------------------------------------------------------------------------------
	// Private Fields:
	//-----------------------------------------------------------------------------------------

	private float q;
	private float r;
	private float p = DEFAULT_P;
	private float x;
	private float k;

    public float K => k;
    public float P => p;

	//-----------------------------------------------------------------------------------------
	// Constructors:
	//-----------------------------------------------------------------------------------------

	// N.B. passing in DEFAULT_Q is necessary, even though we have the same value (as an optional parameter), because this
	// defines a parameterless constructor, allowing us to be new()'d in generics contexts.
	public KalmanFilterFloat() : this(DEFAULT_Q) { }

	public KalmanFilterFloat(float aQ = DEFAULT_Q, float aR = DEFAULT_R) {
		q = aQ;
		r = aR;
	}

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public float Update(float measurement, float? newQ = null, float? newR = null) {

		// update values if supplied.
		if (newQ != null && q != newQ) {
			q = (float)newQ;
		}
		if (newR != null && r != newR) {
			r = (float)newR;
		}

		// update measurement.
		{
			k = (p + q) / (p + q + r);
			Debug.Log(k);
			p = r * (p + q) / (r + p + q);
            Debug.Log(p);
		}

		// filter result back into calculation.
		float result = x + (measurement - x) * k;
		x = result;
		return result;
	}

	public float Update(List<float> measurements, bool areMeasurementsNewestFirst = false, float? newQ = null, float? newR = null) {

		float result = 0;
		int i = (areMeasurementsNewestFirst) ? measurements.Count - 1 : 0;

		while (i < measurements.Count && i >= 0) {

			// decrement or increment the counter.
			if (areMeasurementsNewestFirst) {
				--i;
			}
			else {
				++i;
			}

			result = Update(measurements[i], newQ, newR);
		}

		return result;
	}

	public void Reset() {
		p = 1;
		x = 0;
		k = 0;
	}
}