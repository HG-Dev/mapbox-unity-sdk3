//-----------------------------------------------------------------------
// <copyright file="CanonicalTileId.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mapbox.BaseModule.Data.Tiles
{
	/// <summary>
	/// Data type to store  <see href="https://en.wikipedia.org/wiki/Web_Mercator"> Web Mercator</see> tile scheme.
	/// <see href="http://www.maptiler.org/google-maps-coordinates-tile-bounds-projection/"> See tile IDs in action. </see>
	/// </summary>
	public readonly struct CanonicalTileId : IEquatable<CanonicalTileId>
	{
		/// <summary>
		/// Serializable child type.
		/// This may not be necessary if CanonicalTileIds have no need to be serialized.
		/// </summary>
		[Serializable]
		public struct SerializedValues
		{
			/// <summary> The zoom level. </summary>
			public int z;

			/// <summary> The X coordinate in the tile grid. </summary>
			public int x;

			/// <summary> The Y coordinate in the tile grid. </summary>
			public int y;
		
			public CanonicalTileId ToReadOnly() => new CanonicalTileId(z,x,y);
		}
		
		/// <summary> The zoom level. </summary>
		public readonly int Z;

		/// <summary> The X coordinate in the tile grid. </summary>
		public readonly int X;

		/// <summary> The Y coordinate in the tile grid. </summary>
		public readonly int Y;

		/// <summary>
		///     Initializes a new instance of the <see cref="CanonicalTileId"/> struct,
		///     representing a tile coordinate in a <see href="https://wiki.openstreetmap.org/wiki/Slippy_map">slippy map</see>.
		/// </summary>
		/// <param name="z"> The z coordinate or the zoom level. </param>
		/// <param name="x"> The x coordinate. </param>
		/// <param name="y"> The y coordinate. </param>
		public CanonicalTileId(int z, int x, int y)
		{
			this.Z = z;
			this.X = x;
			this.Y = y;
		}

		internal CanonicalTileId(UnwrappedTileId unwrapped)
		{
			var z = unwrapped.Z;
			var x = unwrapped.X;
			var y = unwrapped.Y;

			var wrap = (x < 0 ? x - (1 << z) + 1 : x) / (1 << z);

			this.Z = z;
			this.X = x - wrap * (1 << z);
			this.Y = y < 0 ? 0 : Math.Min(y, (1 << z) - 1);
		}

		public static CanonicalTileId FromUnwrappedValues(int z, int x, int y)
		{
			var wrap = (x < 0 ? x - (1 << z) + 1 : x) / (1 << z);
			return new CanonicalTileId(z, x - wrap * (1 << z), y < 0 ? 0 : Math.Min(y, (1 << z) - 1));
		}

		/// <summary>
		///     Get the cordinate at the top left of corner of the tile.
		/// </summary>
		/// <returns> The coordinate. </returns>
		public Vector2d.Vector2d ToVector2d()
		{
			double n = Math.PI - ((2.0 * Math.PI * this.Y) / Math.Pow(2.0, this.Z));

			double lat = 180.0 / Math.PI * Math.Atan(Math.Sinh(n));
			double lng = (this.X / Math.Pow(2.0, this.Z) * 360.0) - 180.0;

			// FIXME: Super hack because of rounding issues.
			return new Vector2d.Vector2d(lat - 0.0001, lng + 0.0001);
		}

		/// <summary>
		///     Returns a <see cref="T:System.String"/> that represents the current
		///     <see cref="T:Mapbox.BaseModule.Data.Tiles.CanonicalTileId"/>.
		/// </summary>
		/// <returns>
		///     A <see cref="T:System.String"/> that represents the current
		///     <see cref="T:Mapbox.BaseModule.Data.Tiles.CanonicalTileId"/>.
		/// </returns>
		public override string ToString()
		{
			return $"{Z}/{X}/{Y}";
		}

		public CanonicalTileId GetParentTileId() => new(Z - 1, X >> 1, Y >> 1);

		#region Equality 
		public bool Equals(CanonicalTileId other)
		{
			return this.X == other.X && this.Y == other.Y && this.Z == other.Z;
		}
		
		public override int GetHashCode()
		{
			//old hashcode
			//return X ^ Y ^ Z;

			int hash = X.GetHashCode();
			hash = (hash * 397) ^ Y.GetHashCode();
			hash = (hash * 397) ^ Z.GetHashCode();

			return hash;
		}

		public static bool operator ==(CanonicalTileId a, CanonicalTileId b)
		{
			return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
		}

		public static bool operator !=(CanonicalTileId a, CanonicalTileId b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			if (obj is CanonicalTileId)
			{
				return this.Equals((CanonicalTileId)obj);
			}
			else
			{
				return false;
			}
		}

		#endregion

		/// <summary>
		/// If maxZoom is less than this CanonicalTileId's zoom level, get the ancestor with maxZoom.
		/// Otherwise, this CanonicalTileId is returned. 
		/// </summary>
		/// <param name="maxZoom">Maximum allowed zoom level</param>
		/// <returns>CanonicalTileId at or less than maxZoom</returns>
		public CanonicalTileId ClampZoomToAncestorOrSelf(int maxZoom)
		{
			if (Z < maxZoom)
			{
				return this;
			}

			var delta = Z - maxZoom; //zoom level diff
			return new CanonicalTileId(maxZoom, X >> delta, Y >> delta);
		}

		/// <summary>
		/// Enumerate through the ancestors backwards from this CanonicalTileId.
		/// </summary>
		/// <param name="minZoomInclusive">Final zoom level allowed</param>
		/// <returns>A sequence of CanonicalTileId structs</returns>
		public IEnumerable<CanonicalTileId> EnumerateAncestors(int minZoomInclusive = 1)
		{
			for (var ancestor = GetParentTileId();
			     ancestor.Z >= minZoomInclusive;
			     ancestor = ancestor.GetParentTileId())
			{
				yield return ancestor;
			}
		}
		
		public bool IsAncestorOf(CanonicalTileId canonicalTileId) => this == canonicalTileId.ClampZoomToAncestorOrSelf(Z);
	}

	public static class TileIdExtensions
	{
		public static Vector4 CalculateScaleOffsetAtZoom(this CanonicalTileId current, int zoomDiff)
		{
			var tileZoom = current.Z;

			var scale = 1f;
			var offsetX = 0f;
			var offsetY = 0f;

			var currentParent = current.GetParentTileId();

			for (int i = tileZoom - 1; i >= zoomDiff; i--)
			{
				scale /= 2;

				var bottomLeftChildX = currentParent.X * 2;
				var bottomLeftChildY = currentParent.Y * 2;

				//top left
				if (current.X == bottomLeftChildX && current.Y == bottomLeftChildY)
				{
					offsetX = offsetX / 2;
					offsetY = 0.5f + (offsetY / 2);
				}
				//top right
				else if (current.X == bottomLeftChildX + 1 && current.Y == bottomLeftChildY)
				{
					offsetX = 0.5f + (offsetX / 2);
					offsetY = 0.5f + (offsetY / 2);
				}
				//bottom left
				else if (current.X == bottomLeftChildX && current.Y == bottomLeftChildY + 1)
				{
					offsetX = offsetX / 2;
					offsetY = offsetY / 2;
				}
				//bottom right
				else if (current.X == bottomLeftChildX + 1 && current.Y == bottomLeftChildY + 1)
				{
					offsetX = 0.5f + (offsetX / 2);
					offsetY = offsetY / 2;
				}

				current = currentParent;
				currentParent = currentParent.GetParentTileId();
			}

			return new Vector4(scale, scale, offsetX, offsetY);
		}
		
		public static Vector4 CalculateTopRightScaleOffsetAtZoom(this CanonicalTileId current, int zoomDiff)
		{
			var tileZoom = current.Z;

			var scale = 1f;
			var offsetX = 0f;
			var offsetY = 0f;

			var currentParent = current.GetParentTileId();

			for (int i = tileZoom - 1; i >= zoomDiff; i--)
			{
				scale /= 2;

				var bottomLeftChildX = currentParent.X * 2;
				var bottomLeftChildY = currentParent.Y * 2;

				//top left
				if (current.X == bottomLeftChildX && current.Y == bottomLeftChildY)
				{
					offsetX = offsetX / 2;
					offsetY = offsetY / 2;
					
					
				}
				//top right
				else if (current.X == bottomLeftChildX + 1 && current.Y == bottomLeftChildY)
				{
					offsetX = 0.5f + (offsetX / 2);
					offsetY = offsetY / 2;
				}
				//bottom left
				else if (current.X == bottomLeftChildX && current.Y == bottomLeftChildY + 1)
				{
					offsetX = offsetX / 2;
					offsetY = 0.5f + (offsetY / 2);
				}
				//bottom right
				else if (current.X == bottomLeftChildX + 1 && current.Y == bottomLeftChildY + 1)
				{
					offsetX = 0.5f + (offsetX / 2);
					offsetY = 0.5f + (offsetY / 2);
				}

				current = currentParent;
				currentParent = currentParent.GetParentTileId();
			}

			return new Vector4(scale, scale, offsetX, offsetY);
		}

		public static CanonicalTileId Quadrant(this CanonicalTileId id, int i)
		{
			var childX  = (id.X << 1) + (i % 2);
			var childY  = (id.Y << 1) + (i >> 1);
			return new CanonicalTileId(id.Z + 1, childX, childY);
		}
		
		public static UnwrappedTileId Quadrant(this UnwrappedTileId id, int i)
		{
			var childX  = (id.X << 1) + (i % 2);
			var childY  = (id.Y << 1) + (i >> 1);
			return new UnwrappedTileId(id.Z + 1, childX, childY);
		}
	}
}
