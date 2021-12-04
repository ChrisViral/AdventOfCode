using System;

namespace AdventOfCode.Grids.Vectors;

/// <summary>
/// An angle object
/// </summary>
public readonly struct Angle : IComparable, IComparable<Angle>, IEquatable<Angle>, IFormattable
{
    #region Constants
    /// <summary>
    /// Radians to degrees conversion factor
    /// </summary>
    public const double RAD_TO_DEG = 180d / Math.PI;
    /// <summary>
    /// Degrees to radians conversion factor
    /// </summary>
    public const double DEG_TO_RAD = Math.PI / 180d;
    /// <summary>
    /// Radians to gradians conversion factor
    /// </summary>
    public const double RAD_TO_GRAD = 200d / Math.PI;
    /// <summary>
    /// Gradians to radians conversion factor
    /// </summary>
    public const double GRAD_TO_RAD = Math.PI / 200d;
    /// <summary>
    /// Total angles in radians in a circle
    /// </summary>
    public const double FULL_CIRCLE = 2d * Math.PI;
    /// <summary>
    /// Equality angle tolerance
    /// </summary>
    private const double TOLERANCE = 1E-5;
    /// <summary>
    /// Zero angle
    /// </summary>
    public static readonly Angle Zero = new(0d);
    /// <summary>
    /// Half turn angle (180 degrees)
    /// </summary>
    public static readonly Angle HalfTurn = new(Math.PI);
    /// <summary>
    /// Full turn angle (360 degrees)
    /// </summary>
    public static readonly Angle FullCircle = new(FULL_CIRCLE);
    #endregion

    #region Properties
    /// <summary>
    /// The angle as radians
    /// </summary>
    public double Radians { get; }

    /// <summary>
    /// The angles as degrees
    /// </summary>
    public double Degrees => this.Radians * RAD_TO_DEG;

    /// <summary>
    /// The angle as gradians
    /// </summary>
    public double Gradians => this.Radians * RAD_TO_GRAD;

    /// <summary>
    /// The angle as DMS (degrees, minutes, seconds)
    /// </summary>
    public (int degrees, int minutes, double seconds) DMS
    {
        get
        {
            double angle = this.Degrees;
            int d = (int)angle;
            angle = (angle - d) * 60d;
            int m = (int)angle;
            double s = (angle - m) * 60d;
            return (d, m, s);
        }
    }

    /// <summary>
    /// Returns the angle as an absolute angle between 0 and 180 degrees
    /// </summary>
    public Angle Absolute
    {
        get
        {
            double rads = this.Radians;
            if (rads > Math.PI)
            {
                rads -= Math.PI;
            }

            return new Angle(Math.Abs(rads));
        }
    }

    /// <summary>
    /// Returns the angle as a signed angle between -180 and 180 degrees
    /// </summary>
    public Angle Signed
    {
        get
        {
            double rads = this.Radians;
            if (rads > Math.PI)
            {
                rads -= FULL_CIRCLE;
            }

            return new Angle(rads);
        }
    }

    /// <summary>
    /// Returns the angle as a positive circular angle between 0 and 360 degrees
    /// </summary>
    public Angle Circular
    {
        get
        {
            double rads = this.Radians;
            if (rads < 0d)
            {
                rads += FULL_CIRCLE;
            }

            return new Angle(rads);
        }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new angle from radians
    /// </summary>
    /// <param name="radians">Radians of the angle</param>
    private Angle(double radians) => this.Radians = radians;
    #endregion

    #region Methods
    /// <inheritdoc cref="IComparable.CompareTo"/>
    public int CompareTo(object? obj) => obj is Angle angle ? CompareTo(angle) : 0;

    /// <inheritdoc cref="IComparable{T}.CompareTo"/>
    public int CompareTo(Angle other) => this.Radians.CompareTo(other.Radians);

    /// <inheritdoc cref="object.Equals(object)"/>
    public override bool Equals(object? obj) => obj is Angle angle && Equals(angle);

    /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
    public bool Equals(Angle other) => Math.Abs(this.Radians - other.Radians) < TOLERANCE;

    /// <inheritdoc cref="object.GetHashCode"/>
    public override int GetHashCode() => this.Radians.GetHashCode();

    /// <summary>
    /// String representation of this angle as degrees
    /// </summary>
    /// <returns>Degrees representation of this angle</returns>
    public override string ToString() => $"{this.Degrees}°";

    /// <summary>
    /// String version of this angle, formatted.<br/>
    /// If a normal number format is passed, the angle is returned in degrees with the given format.<br/>
    /// Otherwise, the format can be preceded by "R-", "D-", "G-", "A-" for radians, degrees, gradians, or arc (DMS)
    /// </summary>
    /// <param name="format">Format to return the string in</param>
    /// <param name="formatProvider">Format provider</param>
    /// <returns>A formatted string representation of the angle</returns>
    public string ToString(string? format, IFormatProvider? formatProvider = null)
    {
        string[] splits = format?.Split('-', StringSplitOptions.TrimEntries) ?? Array.Empty<string>();
        if (splits.Length is 2)
        {
            switch (splits[0].ToUpper())
            {
                case "R":
                    return $"{this.Radians.ToString(splits[1], formatProvider)}rad";
                case "D":
                    return $"{this.Degrees.ToString(splits[1], formatProvider)}°";
                case "G":
                    return $"{this.Gradians.ToString(splits[1], formatProvider)}gon";
                case "A":
                    (int d, int m, double s) = this.DMS;
                    return $"{d}°{m}'{s.ToString(splits[1], formatProvider)}\"";
                default:
                    throw new FormatException($"Invalid angle format detected, only 'R', 'D', 'G', or 'A' supported, got {splits[0]} instead");

            }
        }

        switch (format)
        {
            case "R":
            case "r":
                return $"{this.Radians}rad";
            case "D":
            case "d":
                return $"{this.Degrees}°";
            case "G":
            case "g":
                return $"{this.Gradians}gon";
            case "A":
            case "a":
                (int d, int m, double s) = this.DMS;
                return $"{d}°{m}'{s}\"";
            default:
                return $"{this.Degrees.ToString(format, formatProvider)}°";
        }
    }
    #endregion

    #region Static methods
    /// <summary>
    /// Creates a new angle from radians
    /// </summary>
    /// <param name="radians">Radians of the angle</param>
    /// <returns>The angle object</returns>
    public static Angle FromRadians(double radians)
    {
        //Ensure the angle is in a valid range
        switch (radians)
        {
            case > FULL_CIRCLE:
                radians %= FULL_CIRCLE;
                break;
            case < -Math.PI:
                //Mathematical modulo
                radians = ((radians % Math.PI) + Math.PI) % Math.PI;
                break;
        }

        return new Angle(radians);
    }

    /// <summary>
    /// Creates a new angle from degrees
    /// </summary>
    /// <param name="degrees">Degrees of the angle</param>
    /// <returns>The angle object</returns>
    public static Angle FromDegrees(double degrees) => FromRadians(degrees * DEG_TO_RAD);

    /// <summary>
    /// Creates a new angle from gradians
    /// </summary>
    /// <param name="gradians">Gradians of the angle</param>
    /// <returns>The angle object</returns>
    public static Angle FromGradians(double gradians) => FromRadians(gradians * GRAD_TO_RAD);

    /// <summary>
    /// Creates a new angle from DMS (degrees, minutes, seconds)
    /// </summary>
    /// <param name="dms">The DMS of the angle</param>
    /// <returns>The angle object</returns>
    public static Angle FromDMS((int d, int m, double s) dms) => FromDMS(dms.d, dms.m, dms.s);

    /// <summary>
    /// Creates a new angle from DMS (degrees, minutes, seconds)
    /// </summary>
    /// <param name="d">Degrees of the angle</param>
    /// <param name="m">Minutes of the angle</param>
    /// <param name="s">Seconds of the angle</param>
    /// <returns>The angle object</returns>
    public static Angle FromDMS(int d, int m, double s) => FromRadians((d + (m / 60d) + (s / 3600d)) * DEG_TO_RAD);
    #endregion

    #region Operators
    /// <summary>
    /// Equality on two angles
    /// </summary>
    /// <param name="a">First angle</param>
    /// <param name="b">Second angle</param>
    /// <returns>True if both angles are equal, false otherwise</returns>
    public static bool operator ==(in Angle a, in Angle b) => a.Equals(b);

    /// <summary>
    /// Inequality on two angles
    /// </summary>
    /// <param name="a">First angle</param>
    /// <param name="b">Second angle</param>
    /// <returns>True if both angles are unequal, false otherwise</returns>
    public static bool operator !=(in Angle a, in Angle b) => !a.Equals(b);

    /// <summary>
    /// Less than on two vectors
    /// </summary>
    /// <param name="a">First angle</param>
    /// <param name="b">Second angle</param>
    /// <returns>True if both the first angle is less than the second, false otherwise</returns>
    public static bool operator <(in Angle a, in Angle b) => a.Radians < b.Radians;

    /// <summary>
    /// Greater than on two vectors
    /// </summary>
    /// <param name="a">First angle</param>
    /// <param name="b">Second angle</param>
    /// <returns>True if both the first angle is greater than the second, false otherwise</returns>
    public static bool operator >(in Angle a, in Angle b) => a.Radians > b.Radians;

    /// <summary>
    /// Less than or equals on two vectors
    /// </summary>
    /// <param name="a">First angle</param>
    /// <param name="b">Second angle</param>
    /// <returns>True if both the first angle is less than or equals the second, false otherwise</returns>
    public static bool operator <=(in Angle a, in Angle b) => a.Radians <= b.Radians;

    /// <summary>
    /// Greater than or equals on two vectors
    /// </summary>
    /// <param name="a">First angle</param>
    /// <param name="b">Second angle</param>
    /// <returns>True if both the first angle is greater than or equals the second, false otherwise</returns>
    public static bool operator >=(in Angle a, in Angle b) => a.Radians >= b.Radians;

    /// <summary>
    /// Addition on two angles. The result is between 0 and 360 degrees
    /// </summary>
    /// <param name="a">First angle</param>
    /// <param name="b">Second angle</param>
    /// <returns>The sum angle of a and b, between 0 and 360 degrees</returns>
    public static Angle operator +(in Angle a, in Angle b)
    {
        double angle = a.Radians + b.Radians;
        if (angle > FULL_CIRCLE)
        {
            angle -= FULL_CIRCLE;
        }

        return new Angle(angle);
    }

    /// <summary>
    /// Subtraction on two angles. The result is between 0 and 360 degrees
    /// </summary>
    /// <param name="a">First angle</param>
    /// <param name="b">Second angle</param>
    /// <returns>The subtracted angle of a and b, between 0 and 360 degrees</returns>
    public static Angle operator -(in Angle a, in Angle b)
    {
        double angle = a.Radians - b.Radians;
        if (angle < 0d)
        {
            angle += FULL_CIRCLE;
        }

        return new Angle(angle);
    }

    /// <summary>
    /// Negation of an angle. The result is between -180 and 180 degrees
    /// </summary>
    /// <param name="a">Angle to negate</param>
    /// <returns>The negated angle, between -180 and 180 degrees</returns>
    public static Angle operator -(in Angle a)
    {
        double angle = a.Radians;
        if (angle > Math.PI)
        {
            angle -= Math.PI;
        }

        return new Angle(-angle);
    }
    #endregion
}