﻿using UnityEngine;

namespace FairyGUI
{
    public class PlayState
    {
        public bool reachEnding { get; private set; } //是否已播放到结尾
        public bool frameStarting { get; private set; } //是否刚开始新的一帧
        public bool reversed { get; private set; } //是否已反向播放
        public int repeatedCount { get; private set; } //重复次数

        int _curFrame; //当前帧
        float _lastTime;
        float _curFrameDelay; //当前帧延迟
        uint _lastUpdateSeq;

        public PlayState()
        {
        }

        public void Update(MovieClip mc, UpdateContext context)
        {
            if (_lastUpdateSeq == context.workCount) //PlayState may be shared, only update once per frame
                return;

            _lastUpdateSeq = context.workCount;
            float time = Time.time;
            float elapsed = time - _lastTime;
            _lastTime = time;

            reachEnding = false;
            frameStarting = false;
            _curFrameDelay += elapsed;
            int realFrame = reversed ? mc.frameCount - _curFrame - 1 : _curFrame;
            float interval = mc.interval + mc.frames[realFrame].addDelay + ((realFrame == 0 && repeatedCount > 0) ? mc.repeatDelay : 0);
            if (_curFrameDelay < interval)
                return;

            _curFrameDelay = 0;
            _curFrame++;
            frameStarting = true;

            if (_curFrame > mc.frameCount - 1)
            {
                _curFrame = 0;
                repeatedCount++;
                reachEnding = true;
                if (mc.swing)
                {
                    reversed = !reversed;
                    _curFrame++;
                }
            }
        }

        public int currrentFrame
        {
            get { return _curFrame; }
            set { _curFrame = value; _curFrameDelay = 0; }
        }

        public void Rewind()
        {
            _curFrame = 0;
            _curFrameDelay = 0;
            reversed = false;
            reachEnding = false;
        }

        public void Reset()
        {
            _curFrame = 0;
            _curFrameDelay = 0;
            repeatedCount = 0;
            reachEnding = false;
            reversed = false;
        }

        public void Copy(PlayState src)
        {
            _curFrame = src._curFrame;
            _curFrameDelay = src._curFrameDelay;
            repeatedCount = src.repeatedCount;
            reachEnding = src.reachEnding;
            reversed = src.reversed;
        }
    }
}
