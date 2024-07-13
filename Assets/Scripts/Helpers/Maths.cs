using UnityEngine;

public static class Maths
    {
        /// <summary>
        /// Internal method used to compute the spring velocity
        /// </summary>
        /// <param name="currentValue"></param>
        /// <param name="targetValue"></param>
        /// <param name="velocity"></param>
        /// <param name="damping"></param>
        /// <param name="frequency"></param>
        /// <param name="speed"></param>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        private static float SpringVelocity(float currentValue, float targetValue, float velocity, float damping, float frequency, float speed, float deltaTime)
        {
            float maxDeltaTime = Mathf.Min(1.0f / (frequency * 10.0f), deltaTime); 
            frequency = frequency * 2f * Mathf.PI;
            deltaTime = Mathf.Min(deltaTime, maxDeltaTime);
            return velocity + (deltaTime * frequency * frequency * (targetValue - currentValue)) + (-2.0f * deltaTime * frequency * damping * velocity);
        }
        
        /// <summary>
        /// Springs a Vector3 towards a target value 
        /// </summary>
        /// <param name="currentValue">the current value to spring, passed as a ref</param>
        /// <param name="targetValue">the target value we're aiming for</param>
        /// <param name="velocity">a velocity value, passed as ref, used to compute the current speed of the springed value</param>
        /// <param name="damping">the damping, between 0.01f and 1f, the higher the daming, the less springy it'll be</param>
        /// <param name="frequency">the frequency, in Hz, so the amount of periods the spring should go over in 1 second</param>
        /// <param name="speed">the speed (between 0 and 1) at which the spring should operate</param>
        /// <param name="deltaTime">the delta time (usually Time.deltaTime or Time.unscaledDeltaTime)</param>
        public static void Spring(ref Vector3 currentValue, Vector3 targetValue, ref Vector3 velocity, float damping, float frequency, float speed, float deltaTime)
        {
            Vector3 initialVelocity = velocity;
            velocity.x = SpringVelocity(currentValue.x, targetValue.x, velocity.x, damping, frequency, speed, deltaTime);
            velocity.y = SpringVelocity(currentValue.y, targetValue.y, velocity.y, damping, frequency, speed, deltaTime);
            velocity.z = SpringVelocity(currentValue.z, targetValue.z, velocity.z, damping, frequency, speed, deltaTime);
            velocity.x = Lerp(initialVelocity.x, velocity.x, speed, Time.deltaTime);
            velocity.y = Lerp(initialVelocity.y, velocity.y, speed, Time.deltaTime);
            velocity.z = Lerp(initialVelocity.z, velocity.z, speed, Time.deltaTime);
            currentValue += deltaTime * velocity;
        }
        
        /// <summary>
        /// Remaps a value x in interval [A,B], to the proportional value in interval [C,D]
        /// </summary>
        /// <param name="x">The value to remap.</param>
        /// <param name="A">the minimum bound of interval [A,B] that contains the x value</param>
        /// <param name="B">the maximum bound of interval [A,B] that contains the x value</param>
        /// <param name="C">the minimum bound of target interval [C,D]</param>
        /// <param name="D">the maximum bound of target interval [C,D]</param>
        public static float Remap(float x, float A, float B, float C, float D)
        {
            float remappedValue = C + (x-A)/(B-A) * (D - C);
            return remappedValue;
        }
        
        /// <summary>
        /// Returns a vector3 based on the angle in parameters
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector3 DirectionFromAngle(float angle, float additionalAngle)
        {
            angle += additionalAngle;

            Vector3 direction = Vector3.zero;
            direction.x = Mathf.Sin(angle * Mathf.Deg2Rad);
            direction.y = 0f;
            direction.z = Mathf.Cos(angle * Mathf.Deg2Rad);
            return direction;
        }
        
        /// <summary>
        /// Lerps a float towards a target at the specified rate
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static float Lerp(float value, float target, float rate, float deltaTime)
        {
            if (deltaTime == 0f) { return value; }
            return Mathf.Lerp(target, value, LerpRate(rate, deltaTime));
        }
        
        /// <summary>
        /// Lerps a Quaternion towards a target at the specified rate
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static Quaternion Lerp(Quaternion value, Quaternion target, float rate, float deltaTime)
        {
            if (deltaTime == 0f) { return value; }
            return Quaternion.Lerp(target, value, LerpRate(rate, deltaTime));
        }
        
        /// <summary>
        /// Lerps a Vector3 towards a target at the specified rate
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static Vector3 Lerp(Vector3 value, Vector3 target, float rate, float deltaTime)
        {
            if (deltaTime == 0f) { return value; }
            return Vector3.Lerp(target, value, LerpRate(rate, deltaTime));
        }
        
        /// <summary>
        /// internal method used to determine the lerp rate
        /// </summary>
        /// <param name="rate"></param>
        /// <returns></returns>
        private static float LerpRate(float rate, float deltaTime)
        {
            rate = Mathf.Clamp01(rate);
            float invRate = - Mathf.Log(1.0f - rate, 2.0f) * 60f;
            return Mathf.Pow(2.0f, -invRate * deltaTime);
        }
        
        /// <summary>
        /// Returns the result of rolling a dice of the specified number of sides
        /// </summary>
        /// <returns>The result of the dice roll.</returns>
        /// <param name="numberOfSides">Number of sides of the dice.</param>
        public static int RollADice(int numberOfSides)
        {
            return (UnityEngine.Random.Range(1,numberOfSides+1));
        }
        
        /// <summary>
        /// Moves from "from" to "to" by the specified amount and returns the corresponding value
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="amount">Amount.</param>
        public static float Approach(float from, float to, float amount)
        {
            if (from < to)
            {
                from += amount;
                if (from > to)
                {
                    return to;
                }
            }
            else
            {
                from -= amount;
                if (from < to)
                {
                    return to;
                }
            }
            return from;
        } 
        
        public static float RoundToDecimal(float value, int numberOfDecimals)
        {
            if (numberOfDecimals <= 0)
            {
                return Mathf.Round(value);   
            }
            else
            {
                return Mathf.Round(value * 10f * numberOfDecimals) / (10f * numberOfDecimals);    
            }
        }

        /// <summary>
        /// Rounds the value passed in parameters to the closest value in the parameter array
        /// </summary>
        /// <param name="value"></param>
        /// <param name="possibleValues"></param>
        /// <returns></returns>
        public static float RoundToClosest(float value, float[] possibleValues, bool pickSmallestDistance = false)
        {
            if (possibleValues.Length == 0) 
            {
                return 0f;
            }

            float closestValue = possibleValues[0];

            foreach (float possibleValue in possibleValues)
            {
                float closestDistance = Mathf.Abs(closestValue - value);
                float possibleDistance = Mathf.Abs(possibleValue - value);

                if (closestDistance > possibleDistance)
                {
                    closestValue = possibleValue;
                }
                else if (closestDistance == possibleDistance)
                {                    
                    if ((pickSmallestDistance && closestValue > possibleValue) || (!pickSmallestDistance && closestValue < possibleValue))
                    {
                        closestValue = (value < 0) ? closestValue : possibleValue;
                    }
                }
            }
            return closestValue;

        }
    }