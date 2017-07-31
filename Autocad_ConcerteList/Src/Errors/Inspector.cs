﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Autocad_ConcerteList.Errors.UI;
using Autocad_ConcerteList.Log;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Autocad_ConcerteList.Errors
{
    public static class Inspector
    {
        private static Database _db;
        private static Document _doc;
        private static Editor _ed;
        public static List<IError> Errors { get; private set; }

        public static bool HasErrors => Errors.Count > 0;

	    static Inspector()
        {
            Clear();
        }

        public static void Clear()
        {
            Errors = new List<IError>();
            _doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            if (_doc != null)
            {
                _db = _doc.Database;
                _ed = _doc.Editor;
            }
        }

        public static void AddError(IError err)
        {            
            Errors.Add(err);
        }
        public static void AddError(Error err)
        {
            Errors.Add(err);
        }

        public static void AddError(string msg, Icon icon = null)
        {
            var err = new Error(msg, icon);
            Errors.Add(err);
        }

        public static void AddError(string msg)
        {
            var err = new Error(msg, SystemIcons.Error);
            Errors.Add(err);
        }

        public static void AddError(string msg, params object[] args)
        {
            var err = new Error(string.Format(msg, args));
            Errors.Add(err);
        }


        public static void AddError(string msg, Entity ent, Icon icon = null)
        {
            var err = new Error(msg, ent, icon);
            Errors.Add(err);
        }
        public static void AddError(string msg, Entity ent)
        {
            var err = new Error(msg, ent);
            Errors.Add(err);
        }
        public static void AddError(string msg, Entity ent, Matrix3d trans, Icon icon = null)
        {
            var err = new Error(msg, ent, trans, icon);
            Errors.Add(err);
        }

        public static void AddError(string msg, Entity ent, Extents3d ext, Icon icon = null)
        {
            var err = new Error(msg, ext, ent, icon);
            Errors.Add(err);
        }
        public static void AddError(string msg, Entity ent, Extents3d ext)
        {
            var err = new Error(msg, ext, ent);
            Errors.Add(err);
        }
        public static void AddError(string msg, Extents3d ext, ObjectId idEnt, Icon icon = null)
        {
            var err = new Error(msg, ext, idEnt, icon);
            Errors.Add(err);
        }

        public static void AddError(string msg, Extents3d ext, Matrix3d trans, Icon icon = null)
        {
            var err = new Error(msg, ext, trans, icon);
            Errors.Add(err);
        }
        public static void AddError(string msg, Extents3d ext, ObjectId idEnt)
        {
            var err = new Error(msg, ext, idEnt);
            Errors.Add(err);
        }
        public static void AddError(string msg, ObjectId idEnt, Icon icon = null)
        {
            var err = new Error(msg, idEnt, icon);
            Errors.Add(err);
        }
        public static void AddError(string msg, ObjectId idEnt)
        {
            var err = new Error(msg, idEnt);
            Errors.Add(err);
        }
        public static void AddError(string msg, ObjectId idEnt, Matrix3d trans, Icon icon = null)
        {
            var err = new Error(msg, idEnt, trans, icon);
            Errors.Add(err);
        }

        public static void Show()
        {
            if (HasErrors)
            {
                Logger.Log.Error(string.Join("\n", Errors.Select(e => e.Message)));
                Errors = SortErrors(Errors);

                //// WinForms
                //Application.ShowModelessDialog(new FormError(false));

                // WPF
                Show(Errors);
            }
        }
        public static void Show (List<IError> errors)
        {
            var errVM = new ErrorsViewModel(errors);
            errVM.IsDialog = false;
            var errView = new UI.ErrorsView(errVM);
            Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowModelessWindow(errView);
        }

        private static List<IError> SortErrors(List<IError> errors)
        {
            var comparer = NetLib.Comparers.AlphanumComparator.New;
            return errors.OrderBy(o=>o.Message, comparer).ToList();
        }

        /// <summary>
        /// При прерывании вызывает исключение "Отменено пользователем.".
        /// Т.е. можно не обрабатывает DialogResult.
        /// </summary>      
        public static System.Windows.Forms.DialogResult ShowDialog()
        {
	        if (HasErrors)
	        {
		        Logger.Log.Error(string.Join("\n", Errors.Select(e => e.Message)));
		        Errors = SortErrors(Errors);

		        // WPF
		        if (ShowDialog(Errors) == true)
		        {
			        return System.Windows.Forms.DialogResult.OK;
		        }
		        throw new OperationCanceledException();
	        }
	        return System.Windows.Forms.DialogResult.OK;
        }

        public static bool? ShowDialog(List<IError> errors)
        {
	        var errVM = new ErrorsViewModel(errors) {IsDialog = true};
	        var errView = new UI.ErrorsView(errVM);
            return Application.ShowModalWindow(errView);            
        }

        public static void LogErrors()
        {
            Logger.Log.Error(string.Join("\n", Errors.Select(e => e.Message)));
            Errors.Sort();
        }
    }
}