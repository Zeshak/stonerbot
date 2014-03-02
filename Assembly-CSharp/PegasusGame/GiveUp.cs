﻿namespace PegasusGame
{
    using Google.ProtocolBuffers;
    using System;
    using System.CodeDom.Compiler;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;

    [DebuggerNonUserCode, GeneratedCode("ProtoGen", "2.4.1.473"), CompilerGenerated]
    public sealed class GiveUp : GeneratedMessageLite<GiveUp, Builder>
    {
        private static readonly string[] _giveUpFieldNames = new string[0];
        private static readonly uint[] _giveUpFieldTags = new uint[0];
        private static readonly GiveUp defaultInstance = new GiveUp().MakeReadOnly();
        private int memoizedSerializedSize = -1;

        static GiveUp()
        {
            object.ReferenceEquals(PegasusGamelite.Descriptor, null);
        }

        private GiveUp()
        {
        }

        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        public static Builder CreateBuilder(GiveUp prototype)
        {
            return new Builder(prototype);
        }

        public override Builder CreateBuilderForType()
        {
            return new Builder();
        }

        public override bool Equals(object obj)
        {
            return (obj is GiveUp);
        }

        public override int GetHashCode()
        {
            return base.GetType().GetHashCode();
        }

        private GiveUp MakeReadOnly()
        {
            return this;
        }

        public static GiveUp ParseDelimitedFrom(Stream input)
        {
            return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
        }

        public static GiveUp ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
        }

        public static GiveUp ParseFrom(byte[] data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static GiveUp ParseFrom(ByteString data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static GiveUp ParseFrom(ICodedInputStream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static GiveUp ParseFrom(Stream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static GiveUp ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static GiveUp ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static GiveUp ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static GiveUp ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public override void PrintTo(TextWriter writer)
        {
        }

        public override Builder ToBuilder()
        {
            return CreateBuilder(this);
        }

        public override void WriteTo(ICodedOutputStream output)
        {
            int serializedSize = this.SerializedSize;
            string[] strArray = _giveUpFieldNames;
        }

        public static GiveUp DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
        }

        public override GiveUp DefaultInstanceForType
        {
            get
            {
                return DefaultInstance;
            }
        }

        public override bool IsInitialized
        {
            get
            {
                return true;
            }
        }

        public override int SerializedSize
        {
            get
            {
                int memoizedSerializedSize = this.memoizedSerializedSize;
                if (memoizedSerializedSize == -1)
                {
                    memoizedSerializedSize = 0;
                    this.memoizedSerializedSize = memoizedSerializedSize;
                }
                return memoizedSerializedSize;
            }
        }

        protected override GiveUp ThisMessage
        {
            get
            {
                return this;
            }
        }

        [DebuggerNonUserCode, CompilerGenerated, GeneratedCode("ProtoGen", "2.4.1.473")]
        public sealed class Builder : GeneratedBuilderLite<GiveUp, GiveUp.Builder>
        {
            private GiveUp result;
            private bool resultIsReadOnly;

            public Builder()
            {
                this.result = GiveUp.DefaultInstance;
                this.resultIsReadOnly = true;
            }

            internal Builder(GiveUp cloneFrom)
            {
                this.result = cloneFrom;
                this.resultIsReadOnly = true;
            }

            public override GiveUp BuildPartial()
            {
                if (this.resultIsReadOnly)
                {
                    return this.result;
                }
                this.resultIsReadOnly = true;
                return this.result.MakeReadOnly();
            }

            public override GiveUp.Builder Clear()
            {
                this.result = GiveUp.DefaultInstance;
                this.resultIsReadOnly = true;
                return this;
            }

            public override GiveUp.Builder Clone()
            {
                if (this.resultIsReadOnly)
                {
                    return new GiveUp.Builder(this.result);
                }
                return new GiveUp.Builder().MergeFrom(this.result);
            }

            public override GiveUp.Builder MergeFrom(ICodedInputStream input)
            {
                return this.MergeFrom(input, ExtensionRegistry.Empty);
            }

            public override GiveUp.Builder MergeFrom(IMessageLite other)
            {
                if (other is GiveUp)
                {
                    return this.MergeFrom((GiveUp) other);
                }
                base.MergeFrom(other);
                return this;
            }

            public override GiveUp.Builder MergeFrom(GiveUp other)
            {
                if (other != GiveUp.DefaultInstance)
                {
                    this.PrepareBuilder();
                }
                return this;
            }

            public override GiveUp.Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
            {
                uint num;
                string str;
                this.PrepareBuilder();
                while (input.ReadTag(out num, out str))
                {
                    if ((num == 0) && (str != null))
                    {
                        int index = Array.BinarySearch<string>(GiveUp._giveUpFieldNames, str, StringComparer.Ordinal);
                        if (index >= 0)
                        {
                            num = GiveUp._giveUpFieldTags[index];
                        }
                        else
                        {
                            this.ParseUnknownField(input, extensionRegistry, num, str);
                            continue;
                        }
                    }
                    switch (num)
                    {
                        case 0:
                            throw InvalidProtocolBufferException.InvalidTag();
                    }
                    if (WireFormat.IsEndGroupTag(num))
                    {
                        return this;
                    }
                    this.ParseUnknownField(input, extensionRegistry, num, str);
                }
                return this;
            }

            private GiveUp PrepareBuilder()
            {
                if (this.resultIsReadOnly)
                {
                    GiveUp result = this.result;
                    this.result = new GiveUp();
                    this.resultIsReadOnly = false;
                    this.MergeFrom(result);
                }
                return this.result;
            }

            public override GiveUp DefaultInstanceForType
            {
                get
                {
                    return GiveUp.DefaultInstance;
                }
            }

            public override bool IsInitialized
            {
                get
                {
                    return this.result.IsInitialized;
                }
            }

            protected override GiveUp MessageBeingBuilt
            {
                get
                {
                    return this.PrepareBuilder();
                }
            }

            protected override GiveUp.Builder ThisBuilder
            {
                get
                {
                    return this;
                }
            }
        }

        [CompilerGenerated, GeneratedCode("ProtoGen", "2.4.1.473"), DebuggerNonUserCode]
        public static class Types
        {
            [GeneratedCode("ProtoGen", "2.4.1.473"), CompilerGenerated]
            public enum PacketID
            {
                ID = 11
            }
        }
    }
}

