using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace Autocad_ConcerteList.Lib
{
	public static class Extensions
	{
		public static string GetEffectiveName(this BlockReference br)
		{
			using (var btrDyn = br.DynamicBlockTableRecord.Open(OpenMode.ForRead) as BlockTableRecord)
			{
				return btrDyn.Name;
			}
		}

		public static bool IsValidEx(this ObjectId id)
		{
			return id.IsValid && !id.IsNull && !id.IsErased;
		}

		public static void Zoom(this Editor ed, Extents3d ext)
		{
			if (ed == null)
				return;

			using (var view = ed.GetCurrentView())
			{
				ext.TransformBy(view.WorldToEye());
				view.Width = ext.MaxPoint.X - ext.MinPoint.X;
				view.Height = ext.MaxPoint.Y - ext.MinPoint.Y;
				view.CenterPoint = new Point2d(
					(ext.MaxPoint.X + ext.MinPoint.X) / 2.0,
					(ext.MaxPoint.Y + ext.MinPoint.Y) / 2.0);
				ed.SetCurrentView(view);
			}
		}

		public static Matrix3d WorldToEye(this ViewTableRecord view)
		{
			return view.EyeToWorld().Inverse();
		}

		public static Matrix3d EyeToWorld(this ViewTableRecord view)
		{
			if (view == null)
				throw new ArgumentNullException("view");

			return
				Matrix3d.Rotation(-view.ViewTwist, view.ViewDirection, view.Target) *
				Matrix3d.Displacement(view.Target - Point3d.Origin) *
				Matrix3d.PlaneToWorld(view.ViewDirection);
		}

		public static double Diagonal(this Extents3d ext)
		{
			return (ext.MaxPoint - ext.MinPoint).Length;
		}

		/// <summary>
		/// Функция производит "мигание" объектом при помощи Highlight/Unhighlight
		/// </summary>
		/// <param name="id">ObjectId для примитива</param>
		/// <param name="num">Количество "миганий"</param>
		/// <param name="delay1">Длительность "подсвеченного" состояния</param>
		/// <param name="delay2">Длительность "неподсвеченного" состояния</param>
		public static void FlickObjectHighlight(this ObjectId id, int num, int delay1, int delay2)
		{
			var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
			for (var i = 0; i < num; i++)
			{
				// Highlight entity
				using (var doclock = doc.LockDocument())
				using (var tr = doc.TransactionManager.StartTransaction())
				{
					var en = (Entity)tr.GetObject(id, OpenMode.ForWrite);
					var ids = new ObjectId[1]; ids[0] = id;
					var index = new SubentityId(SubentityType.Null, IntPtr.Zero);
					var path = new FullSubentityPath(ids, index);
					en.Highlight(path, true);
					tr.Commit();
				}
				doc.Editor.UpdateScreen();
				// Wait for delay1 msecs
				Thread.Sleep(delay1);
				// Unhighlight entity
				using (var doclock = doc.LockDocument())
				{
					using (var tr = doc.TransactionManager.StartTransaction())
					{
						var en = (Entity)tr.GetObject(id, OpenMode.ForWrite);
						var ids = new ObjectId[1]; ids[0] = id;
						var index = new SubentityId(SubentityType.Null, IntPtr.Zero);
						var path = new FullSubentityPath(ids, index);
						en.Unhighlight(path, true);
						tr.Commit();
					}
				}
				doc.Editor.UpdateScreen();
				// Wait for delay2 msecs
				Thread.Sleep(delay2);
			}
		}

		// Opens a DBObject in ForRead mode (kaefer @ TheSwamp)
		public static T GetObject<T>(this ObjectId id) where T : DBObject
		{
			return id.GetObject<T>(OpenMode.ForRead);
		}

		// Opens a DBObject in the given mode (kaefer @ TheSwamp)
		public static T GetObject<T>(this ObjectId id, OpenMode mode) where T : DBObject
		{
			if (!id.IsValidEx())
				return null;
			return id.GetObject(mode, false, true) as T;
		}

		// Opens a collection of DBObject in ForRead mode (kaefer @ TheSwamp)       
		public static IEnumerable<T> GetObjects<T>(this IEnumerable ids) where T : DBObject
		{
			return ids.GetObjects<T>(OpenMode.ForRead);
		}

		// Opens a collection of DBObject in the given mode (kaefer @ TheSwamp)
		public static IEnumerable<T> GetObjects<T>(this IEnumerable ids, OpenMode mode) where T : DBObject
		{
			return ids
				.Cast<ObjectId>()
				.Select(id => id.GetObject<T>(mode))
				.Where(res => res != null);
		}

		/// <summary>
		/// Копирование объекта в одной базе
		/// </summary>
		/// <param name="idEnt">Копируемый объект</param>
		/// <param name="idBtrOwner">Куда копировать (контейнер - BlockTableRecord)</param>                
		public static ObjectId CopyEnt(this ObjectId idEnt, ObjectId idBtrOwner)
		{
			var db = idEnt.Database;
			var map = new IdMapping();
			var ids = new ObjectIdCollection(new[] { idEnt });
			db.DeepCloneObjects(ids, idBtrOwner, map, false);
			return map[idEnt].Value;
		}
	}
}
