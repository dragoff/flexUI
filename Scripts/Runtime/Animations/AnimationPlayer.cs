﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace FlexUI.Animations
{
    public class AnimationPlayer : MonoBehaviour
    {
        private static Dictionary<RectTransform, AnimatedItem> rectToItem = new Dictionary<RectTransform, AnimatedItem>();
        private static AnimationPlayer instance;

        private static AnimationPlayer Instance
        {
            get
            {
                if (instance == null)
                {
                    var holder = new GameObject(nameof(AnimationPlayer), typeof(AnimationPlayer));
                    DontDestroyOnLoad(holder);
                    instance = holder.GetComponent<AnimationPlayer>();
                }
                return instance;
            }
        }

        #region Play

        public static AnimatedItem Play(Component comp, EaseAnimation anim, Action onDone = null, bool forcedStopPreviousAnimation = true, float timeScale = 1, int repeatCount = 1)
        {
            return Play(comp.transform as RectTransform, anim, onDone, forcedStopPreviousAnimation, timeScale, repeatCount);
        }

        public static AnimatedItem Play(GameObject obj, EaseAnimation anim, Action onDone = null, bool forcedStopPreviousAnimation = true, float timeScale = 1, int repeatCount = 1)
        {
            return Play(obj.transform as RectTransform, anim, onDone, forcedStopPreviousAnimation, timeScale, repeatCount);
        }

        public static IEnumerator PlayCoroutine(Component comp, EaseAnimation anim, bool forcedStopPreviousAnimation = true, float timeScale = 1, int repeatCount = 1)
        {
            return PlayCoroutine(comp.transform as RectTransform, anim, forcedStopPreviousAnimation, timeScale, repeatCount);
        }

        public static IEnumerator PlayCoroutine(GameObject obj, EaseAnimation anim, bool forcedStopPreviousAnimation = true, float timeScale = 1, int repeatCount = 1)
        {
            return PlayCoroutine(obj.transform as RectTransform, anim, forcedStopPreviousAnimation, timeScale, repeatCount);
        }

        public static async Task PlayAsync(Component comp, EaseAnimation anim, bool forcedStopPreviousAnimation = true, float timeScale = 1, int repeatCount = 1)
        {
            await PlayAsync(comp.transform as RectTransform, anim, forcedStopPreviousAnimation, timeScale, repeatCount);
        }

        public static async Task PlayAsync(GameObject obj, EaseAnimation anim, bool forcedStopPreviousAnimation = true, float timeScale = 1, int repeatCount = 1)
        {
            await PlayAsync(obj.transform as RectTransform, anim, forcedStopPreviousAnimation, timeScale, repeatCount);
        }

        public static AnimatedItem Play(RectTransform rt, EaseAnimation anim, Action onDone = null, bool forcedStopPreviousAnimation = true, float timeScale = 1, int repeatCount = 1)
        {
	        if (anim == null)
	        {
		        onDone?.Invoke();
				return default;
	        }

            //get or create AnimatedItem
            var item = GetOrCreateAnimatedItem(rt);

            //start coroutine
            Instance.StartCoroutine(PlayCoroutine(item, anim, onDone, forcedStopPreviousAnimation, timeScale, repeatCount));

            return item;
        }

        public static IEnumerator PlayCoroutine(RectTransform rt, EaseAnimation anim, bool forcedStopPreviousAnimation = true, float timeScale = 1, int repeatCount = 1)
        {
            //get or create AnimatedItem
            var item = GetOrCreateAnimatedItem(rt);

            //start coroutine
            var finished = false;
            Instance.StartCoroutine(PlayCoroutine(item, anim, ()=> { finished = true; }, forcedStopPreviousAnimation, timeScale, repeatCount));

            while (!finished)
                yield return null;
        }

        public static async Task PlayAsync(RectTransform rt, EaseAnimation anim, bool forcedStopPreviousAnimation = true, float timeScale = 1, int repeatCount = 1)
        {
            //get or create AnimatedItem
            var item = GetOrCreateAnimatedItem(rt);

            //start coroutine
            var tcs = new TaskCompletionSource<bool>();
            Instance.StartCoroutine(PlayCoroutine(item, anim, () => tcs.TrySetResult(true), forcedStopPreviousAnimation, timeScale, repeatCount));

            await tcs.Task;
        }

        #endregion

        #region Private

        private static AnimatedItem GetOrCreateAnimatedItem(RectTransform rt)
        {
            AnimatedItem item;
            if (!rectToItem.TryGetValue(rt, out item))
                item = rectToItem[rt] = new AnimatedItem(rt);
            return item;
        }

        private static IEnumerator PlayCoroutine(AnimatedItem item, EaseAnimation anim, Action onDone = null, bool forcedStopPreviousAnimation = true, float timeScale = 1, int repeatCount = 1)
        {
            //wait finsh of prev animation
            while (item.CurrentEaseAnimation != null)
            {
                if (forcedStopPreviousAnimation)
                    item.CurrentEaseAnimation.StopAndResetTransform();//forced stop prev animation
                yield return null;
            }

            //activate object
            item.Transform.gameObject.SetActive(true);

            if (!item.Transform.gameObject.activeInHierarchy)
            {
                //can not play inactive...
                item.CurrentEaseAnimation = null;
                onDone?.Invoke();
                yield break;
            }

            var animInstance = anim?.CreateInstance();

            item.CurrentEaseAnimation = animInstance;
            try
            {
                if (animInstance != null)
                    yield return animInstance.Play(item.Transform, timeScale, repeatCount);
            }
            finally
            {
                item.CurrentEaseAnimation = null;
                onDone?.Invoke();
            }
        }

        #endregion

        public class AnimatedItem
        {
            public RectTransform Transform;
            public IEaseAnimation CurrentEaseAnimation;

            public AnimatedItem(RectTransform transform)
            {
                this.Transform = transform;
            }
        }
    }
}