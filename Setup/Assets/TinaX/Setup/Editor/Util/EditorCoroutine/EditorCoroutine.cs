// Reference from https://github.com/garettbass/UnityExtensions.EditorCoroutine

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace TinaXEditor.Setup.Internal
{

#pragma warning disable CA1063 // Implement IDisposable Correctly
    public class EditorCoroutine : IDisposable
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {

        private readonly Stack<IEnumerator> mStack = new Stack<IEnumerator>();


        public EditorCoroutine(IEnumerator routine)
        {
            Push(routine);
        }

#pragma warning disable CA1063 // Implement IDisposable Correctly
        void IDisposable.Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
        {
            Stop();
        }


        public static EditorCoroutine Start(IEnumerator routine)
        {
            return new EditorCoroutine(routine).Start();
        }

        public EditorCoroutine Start()
        {
            if (mStack.Count > 0)
            {
                EditorApplication.update -= Update;
                EditorApplication.update += Update;
            }
            return this;
        }

        public void Stop()
        {
            EditorApplication.update -= Update;
        }


        private void Update()
        {
            try { MoveNext(); }
            catch (Exception ex) { Stop(); Debug.LogError(ex); }
        }


        private void MoveNext()
        {
            var routine = mStack.Peek();
            if (routine.MoveNext())
                Push(routine.Current);
            else
                Pop();
        }


        private void Push(object subroutine)
        {
            Push(subroutine as IEnumerator ?? WaitForSeconds(subroutine));
        }

        private void Push(IEnumerator subroutine)
        {
            if (subroutine != null) 
                mStack.Push(subroutine);
        }


        private void Pop()
        {
            mStack.Pop();
            if (mStack.Count == 0) Stop();
        }


        private static readonly FieldInfo
        WaitForSeconds_m_Seconds =
            typeof(UnityEngine.WaitForSeconds)
            .GetField(
                "m_Seconds",
                BindingFlags.Instance |
                BindingFlags.NonPublic
            );

        private static IEnumerator WaitForSeconds(object subroutine)
        {
            var waitForSeconds = subroutine as UnityEngine.WaitForSeconds;
            if (waitForSeconds == null)
                return null;

            var seconds =
                (float)
                WaitForSeconds_m_Seconds
                .GetValue(waitForSeconds);

            return WaitForSeconds(seconds);
        }

        private static IEnumerator WaitForSeconds(float seconds)
        {
            var end = DateTime.Now + TimeSpan.FromSeconds(seconds);
            do yield return null; while (DateTime.Now < end);
        }

    }

}