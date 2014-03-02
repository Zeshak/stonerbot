﻿namespace bnet.protocol.channel
{
    using bnet.protocol;
    using Google.ProtocolBuffers;
    using System;
    using System.CodeDom.Compiler;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;

    [DebuggerNonUserCode, GeneratedCode("ProtoGen", "2.4.1.473"), CompilerGenerated]
    public sealed class UpdateChannelStateNotification : GeneratedMessageLite<UpdateChannelStateNotification, Builder>
    {
        private static readonly string[] _updateChannelStateNotificationFieldNames = new string[] { "agent_id", "state_change" };
        private static readonly uint[] _updateChannelStateNotificationFieldTags = new uint[] { 10, 0x12 };
        private EntityId agentId_;
        public const int AgentIdFieldNumber = 1;
        private static readonly UpdateChannelStateNotification defaultInstance = new UpdateChannelStateNotification().MakeReadOnly();
        private bool hasAgentId;
        private bool hasStateChange;
        private int memoizedSerializedSize = -1;
        private ChannelState stateChange_;
        public const int StateChangeFieldNumber = 2;

        static UpdateChannelStateNotification()
        {
            object.ReferenceEquals(ChannelService.Descriptor, null);
        }

        private UpdateChannelStateNotification()
        {
        }

        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        public static Builder CreateBuilder(UpdateChannelStateNotification prototype)
        {
            return new Builder(prototype);
        }

        public override Builder CreateBuilderForType()
        {
            return new Builder();
        }

        public override bool Equals(object obj)
        {
            UpdateChannelStateNotification notification = obj as UpdateChannelStateNotification;
            if (notification == null)
            {
                return false;
            }
            if ((this.hasAgentId != notification.hasAgentId) || (this.hasAgentId && !this.agentId_.Equals(notification.agentId_)))
            {
                return false;
            }
            return ((this.hasStateChange == notification.hasStateChange) && (!this.hasStateChange || this.stateChange_.Equals(notification.stateChange_)));
        }

        public override int GetHashCode()
        {
            int hashCode = base.GetType().GetHashCode();
            if (this.hasAgentId)
            {
                hashCode ^= this.agentId_.GetHashCode();
            }
            if (this.hasStateChange)
            {
                hashCode ^= this.stateChange_.GetHashCode();
            }
            return hashCode;
        }

        private UpdateChannelStateNotification MakeReadOnly()
        {
            return this;
        }

        public static UpdateChannelStateNotification ParseDelimitedFrom(Stream input)
        {
            return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
        }

        public static UpdateChannelStateNotification ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
        }

        public static UpdateChannelStateNotification ParseFrom(byte[] data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static UpdateChannelStateNotification ParseFrom(ByteString data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static UpdateChannelStateNotification ParseFrom(ICodedInputStream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static UpdateChannelStateNotification ParseFrom(Stream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static UpdateChannelStateNotification ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static UpdateChannelStateNotification ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static UpdateChannelStateNotification ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static UpdateChannelStateNotification ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public override void PrintTo(TextWriter writer)
        {
            GeneratedMessageLite<UpdateChannelStateNotification, Builder>.PrintField("agent_id", this.hasAgentId, this.agentId_, writer);
            GeneratedMessageLite<UpdateChannelStateNotification, Builder>.PrintField("state_change", this.hasStateChange, this.stateChange_, writer);
        }

        public override Builder ToBuilder()
        {
            return CreateBuilder(this);
        }

        public override void WriteTo(ICodedOutputStream output)
        {
            int serializedSize = this.SerializedSize;
            string[] strArray = _updateChannelStateNotificationFieldNames;
            if (this.hasAgentId)
            {
                output.WriteMessage(1, strArray[0], this.AgentId);
            }
            if (this.hasStateChange)
            {
                output.WriteMessage(2, strArray[1], this.StateChange);
            }
        }

        public EntityId AgentId
        {
            get
            {
                // This item is obfuscated and can not be translated.
                if (this.agentId_ != null)
                {
                    goto Label_0012;
                }
                return EntityId.DefaultInstance;
            }
        }

        public static UpdateChannelStateNotification DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
        }

        public override UpdateChannelStateNotification DefaultInstanceForType
        {
            get
            {
                return DefaultInstance;
            }
        }

        public bool HasAgentId
        {
            get
            {
                return this.hasAgentId;
            }
        }

        public bool HasStateChange
        {
            get
            {
                return this.hasStateChange;
            }
        }

        public override bool IsInitialized
        {
            get
            {
                if (!this.hasStateChange)
                {
                    return false;
                }
                if (this.HasAgentId && !this.AgentId.IsInitialized)
                {
                    return false;
                }
                if (!this.StateChange.IsInitialized)
                {
                    return false;
                }
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
                    if (this.hasAgentId)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeMessageSize(1, this.AgentId);
                    }
                    if (this.hasStateChange)
                    {
                        memoizedSerializedSize += CodedOutputStream.ComputeMessageSize(2, this.StateChange);
                    }
                    this.memoizedSerializedSize = memoizedSerializedSize;
                }
                return memoizedSerializedSize;
            }
        }

        public ChannelState StateChange
        {
            get
            {
                // This item is obfuscated and can not be translated.
                if (this.stateChange_ != null)
                {
                    goto Label_0012;
                }
                return ChannelState.DefaultInstance;
            }
        }

        protected override UpdateChannelStateNotification ThisMessage
        {
            get
            {
                return this;
            }
        }

        [GeneratedCode("ProtoGen", "2.4.1.473"), DebuggerNonUserCode, CompilerGenerated]
        public sealed class Builder : GeneratedBuilderLite<UpdateChannelStateNotification, UpdateChannelStateNotification.Builder>
        {
            private UpdateChannelStateNotification result;
            private bool resultIsReadOnly;

            public Builder()
            {
                this.result = UpdateChannelStateNotification.DefaultInstance;
                this.resultIsReadOnly = true;
            }

            internal Builder(UpdateChannelStateNotification cloneFrom)
            {
                this.result = cloneFrom;
                this.resultIsReadOnly = true;
            }

            public override UpdateChannelStateNotification BuildPartial()
            {
                if (this.resultIsReadOnly)
                {
                    return this.result;
                }
                this.resultIsReadOnly = true;
                return this.result.MakeReadOnly();
            }

            public override UpdateChannelStateNotification.Builder Clear()
            {
                this.result = UpdateChannelStateNotification.DefaultInstance;
                this.resultIsReadOnly = true;
                return this;
            }

            public UpdateChannelStateNotification.Builder ClearAgentId()
            {
                this.PrepareBuilder();
                this.result.hasAgentId = false;
                this.result.agentId_ = null;
                return this;
            }

            public UpdateChannelStateNotification.Builder ClearStateChange()
            {
                this.PrepareBuilder();
                this.result.hasStateChange = false;
                this.result.stateChange_ = null;
                return this;
            }

            public override UpdateChannelStateNotification.Builder Clone()
            {
                if (this.resultIsReadOnly)
                {
                    return new UpdateChannelStateNotification.Builder(this.result);
                }
                return new UpdateChannelStateNotification.Builder().MergeFrom(this.result);
            }

            public UpdateChannelStateNotification.Builder MergeAgentId(EntityId value)
            {
                Google.ProtocolBuffers.ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                if (this.result.hasAgentId && (this.result.agentId_ != EntityId.DefaultInstance))
                {
                    this.result.agentId_ = EntityId.CreateBuilder(this.result.agentId_).MergeFrom(value).BuildPartial();
                }
                else
                {
                    this.result.agentId_ = value;
                }
                this.result.hasAgentId = true;
                return this;
            }

            public override UpdateChannelStateNotification.Builder MergeFrom(UpdateChannelStateNotification other)
            {
                if (other != UpdateChannelStateNotification.DefaultInstance)
                {
                    this.PrepareBuilder();
                    if (other.HasAgentId)
                    {
                        this.MergeAgentId(other.AgentId);
                    }
                    if (other.HasStateChange)
                    {
                        this.MergeStateChange(other.StateChange);
                    }
                }
                return this;
            }

            public override UpdateChannelStateNotification.Builder MergeFrom(ICodedInputStream input)
            {
                return this.MergeFrom(input, ExtensionRegistry.Empty);
            }

            public override UpdateChannelStateNotification.Builder MergeFrom(IMessageLite other)
            {
                if (other is UpdateChannelStateNotification)
                {
                    return this.MergeFrom((UpdateChannelStateNotification) other);
                }
                base.MergeFrom(other);
                return this;
            }

            public override UpdateChannelStateNotification.Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
            {
                uint num;
                string str;
                this.PrepareBuilder();
                while (input.ReadTag(out num, out str))
                {
                    if ((num == 0) && (str != null))
                    {
                        int index = Array.BinarySearch<string>(UpdateChannelStateNotification._updateChannelStateNotificationFieldNames, str, StringComparer.Ordinal);
                        if (index >= 0)
                        {
                            num = UpdateChannelStateNotification._updateChannelStateNotificationFieldTags[index];
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

                        case 10:
                        {
                            EntityId.Builder builder = EntityId.CreateBuilder();
                            if (this.result.hasAgentId)
                            {
                                builder.MergeFrom(this.AgentId);
                            }
                            input.ReadMessage(builder, extensionRegistry);
                            this.AgentId = builder.BuildPartial();
                            continue;
                        }
                        case 0x12:
                        {
                            ChannelState.Builder builder2 = ChannelState.CreateBuilder();
                            if (this.result.hasStateChange)
                            {
                                builder2.MergeFrom(this.StateChange);
                            }
                            input.ReadMessage(builder2, extensionRegistry);
                            this.StateChange = builder2.BuildPartial();
                            continue;
                        }
                    }
                    if (WireFormat.IsEndGroupTag(num))
                    {
                        return this;
                    }
                    this.ParseUnknownField(input, extensionRegistry, num, str);
                }
                return this;
            }

            public UpdateChannelStateNotification.Builder MergeStateChange(ChannelState value)
            {
                Google.ProtocolBuffers.ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                if (this.result.hasStateChange && (this.result.stateChange_ != ChannelState.DefaultInstance))
                {
                    this.result.stateChange_ = ChannelState.CreateBuilder(this.result.stateChange_).MergeFrom(value).BuildPartial();
                }
                else
                {
                    this.result.stateChange_ = value;
                }
                this.result.hasStateChange = true;
                return this;
            }

            private UpdateChannelStateNotification PrepareBuilder()
            {
                if (this.resultIsReadOnly)
                {
                    UpdateChannelStateNotification result = this.result;
                    this.result = new UpdateChannelStateNotification();
                    this.resultIsReadOnly = false;
                    this.MergeFrom(result);
                }
                return this.result;
            }

            public UpdateChannelStateNotification.Builder SetAgentId(EntityId value)
            {
                Google.ProtocolBuffers.ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasAgentId = true;
                this.result.agentId_ = value;
                return this;
            }

            public UpdateChannelStateNotification.Builder SetAgentId(EntityId.Builder builderForValue)
            {
                Google.ProtocolBuffers.ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
                this.PrepareBuilder();
                this.result.hasAgentId = true;
                this.result.agentId_ = builderForValue.Build();
                return this;
            }

            public UpdateChannelStateNotification.Builder SetStateChange(ChannelState value)
            {
                Google.ProtocolBuffers.ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasStateChange = true;
                this.result.stateChange_ = value;
                return this;
            }

            public UpdateChannelStateNotification.Builder SetStateChange(ChannelState.Builder builderForValue)
            {
                Google.ProtocolBuffers.ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
                this.PrepareBuilder();
                this.result.hasStateChange = true;
                this.result.stateChange_ = builderForValue.Build();
                return this;
            }

            public EntityId AgentId
            {
                get
                {
                    return this.result.AgentId;
                }
                set
                {
                    this.SetAgentId(value);
                }
            }

            public override UpdateChannelStateNotification DefaultInstanceForType
            {
                get
                {
                    return UpdateChannelStateNotification.DefaultInstance;
                }
            }

            public bool HasAgentId
            {
                get
                {
                    return this.result.hasAgentId;
                }
            }

            public bool HasStateChange
            {
                get
                {
                    return this.result.hasStateChange;
                }
            }

            public override bool IsInitialized
            {
                get
                {
                    return this.result.IsInitialized;
                }
            }

            protected override UpdateChannelStateNotification MessageBeingBuilt
            {
                get
                {
                    return this.PrepareBuilder();
                }
            }

            public ChannelState StateChange
            {
                get
                {
                    return this.result.StateChange;
                }
                set
                {
                    this.SetStateChange(value);
                }
            }

            protected override UpdateChannelStateNotification.Builder ThisBuilder
            {
                get
                {
                    return this;
                }
            }
        }
    }
}

