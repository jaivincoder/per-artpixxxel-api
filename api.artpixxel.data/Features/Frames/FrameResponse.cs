using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Models;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.Frames
{
    public class FrameResponse : BaseResponse
    {
        public List<Frame> Data { get; set; }

    }

    public class FrameGetResponse : BaseResponse
    {
        public List<FrameDto> Data { get; set; }

    }

    public class FrameCRUDResponse : BaseResponse
    {
        public Frame Data { get; set; }
    }
}