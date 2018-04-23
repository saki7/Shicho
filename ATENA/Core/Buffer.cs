extern alias CitiesL;

using System;
using System.Collections;
using System.Collections.Generic;


namespace ATENA.Core
{
    using NetSegmentBuffer = IndexedBuffer<CitiesL.NetSegment>;
    using NetNodeBuffer = IndexedBuffer<CitiesL.NetNode>;

    internal class IndexedBuffer<T>
        : IEnumerable<T>
        where T: struct
    {
        public IndexedBuffer(T[] arr, UInt64 actualCount)
        {
            arr_ = arr;
            actualCount_ = actualCount;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0u; i < actualCount_; ++i) {
                yield return arr_[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        private T[] arr_;
        private UInt64 actualCount_;
    }

    static class Buffer
    {
        public static NetSegmentBuffer Segments(CitiesL.NetManager mgr)
        {
            return new NetSegmentBuffer(mgr.m_segments.m_buffer, (UInt64)mgr.m_segmentCount);
        }

        public static NetNodeBuffer Nodes(CitiesL.NetManager mgr)
        {
            return new NetNodeBuffer(mgr.m_nodes.m_buffer, (UInt64) mgr.m_nodeCount);
        }
    }
}
