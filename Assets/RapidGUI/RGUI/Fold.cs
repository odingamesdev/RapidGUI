﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace RapidGUI
{
    public class Fold
    {
        public bool isOpen;
        public string name;
        Action titleAction;

        public class FuncData
        {
            public Func<bool> _checkEnable;
            public Func<bool> _draw;
        }
        List<FuncData> _funcDatas = new List<FuncData>();

        public Fold(string name, Func<bool> drawFunc, bool enableFirst = false) : this(name, null, drawFunc, enableFirst) { }

        public Fold(string name, Func<bool> checkEnableFunc, Func<bool> drawFunc, bool enableFirst = false)
        {
            this.name = name;
            isOpen = enableFirst;
            Add(checkEnableFunc, drawFunc);
        }

        public Fold(string name, Action drawFunc, bool enableFirst = false) : this(name, null, drawFunc, enableFirst) { }

        public Fold(string name, Func<bool> checkEnableFunc, Action drawFunc, bool enableFirst = false) : this(name, checkEnableFunc, () => { drawFunc(); return false; }, enableFirst) { }


        public void SetTitleAction(Action titleAction) => this.titleAction = titleAction;


        public void Add(Func<bool> drawFunc) => Add(null, drawFunc);

        public void Add(Func<bool> checkEnableFunc, Func<bool> drawFunc)
        {
            _funcDatas.Add(new FuncData()
            {
                _checkEnable = checkEnableFunc,
                _draw = drawFunc
            });
        }

        public bool DoGUI()
        {
            var ret = false;
            var drawFuncs = _funcDatas.Where(fd => fd._checkEnable == null || fd._checkEnable()).Select(fd => fd._draw).ToList();

            if (drawFuncs.Any())
            {
                var foldStr = isOpen ? "▼" : "▶";

                using (new GUILayout.HorizontalScope())
                {
                    isOpen ^= GUILayout.Button(foldStr + name, Style.Fold);
                    titleAction?.Invoke();
                }

                using (new RGUI.IndentScope())
                {
                    if (isOpen)
                    {
                        ret |= drawFuncs.Aggregate(false, (changed, drawFunc) => changed || drawFunc());
                    }
                }
            }

            return ret;
        }

        public static class Style
        {
            public static readonly GUIStyle Fold;

            static Style()
            {
                var style = new GUIStyle(GUI.skin.label);
                var toggle = GUI.skin.toggle;
                style.normal.textColor = toggle.normal.textColor;
                style.hover.textColor = toggle.hover.textColor;

                var tex = new Texture2D(1, 1);
                tex.SetPixels(new[] { new Color(0.5f, 0.5f, 0.5f, 0.5f) });
                tex.Apply();
                style.hover.background = tex;

                Fold = style;
            }
        }
    }
}