﻿namespace bnet.protocol.channel
{
    using bnet.protocol;
    using Google.ProtocolBuffers;
    using Google.ProtocolBuffers.Collections;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;

    [DebuggerNonUserCode, GeneratedCode("ProtoGen", "2.4.1.473"), CompilerGenerated]
    public sealed class UpdateMemberStateNotification : GeneratedMessageLite<UpdateMemberStateNotification, Builder>
    {
        private static readonly string[] _updateMemberStateNotificationFieldNames = new string[] { "agent_id", "state_change" };
        private static readonly uint[] _updateMemberStateNotificationFieldTags = new uint[] { 10, 0x12 };
        private EntityId agentId_;
        public const int AgentIdFieldNumber = 1;
        private static readonly UpdateMemberStateNotification defaultInstance = new UpdateMemberStateNotification().MakeReadOnly();
        private bool hasAgentId;
        private int memoizedSerializedSize = -1;
        private PopsicleList<Member> stateChange_ = new PopsicleList<Member>();
        public const int StateChangeFieldNumber = 2;

        static UpdateMemberStateNotification()
        {
            object.ReferenceEquals(ChannelService.Descriptor, null);
        }

        private UpdateMemberStateNotification()
        {
        }

        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        public static Builder CreateBuilder(UpdateMemberStateNotification prototype)
        {
            return new Builder(prototype);
        }

        public override Builder CreateBuilderForType()
        {
            return new Builder();
        }

        public override bool Equals(object obj)
        {
            UpdateMemberStateNotification notification = obj as UpdateMemberStateNotification;
            if (notification == null)
            {
                return false;
            }
            if ((this.hasAgentId != notification.hasAgentId) || (this.hasAgentId && !this.agentId_.Equals(notification.agentId_)))
            {
                return false;
            }
            if (this.stateChange_.Count != notification.stateChange_.Count)
            {
                return false;
            }
            for (int i = 0; i < this.stateChange_.Count; i++)
            {
                if (!this.stateChange_[i].Equals(notification.stateChange_[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            int hashCode = base.GetType().GetHashCode();
            if (this.hasAgentId)
            {
                hashCode ^= this.agentId_.GetHashCode();
            }
            IEnumerator<Member> enumerator = this.stateChange_.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Member current = enumerator.Current;
                    hashCode ^= current.GetHashCode();
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            return hashCode;
        }

        public Member GetStateChange(int index)
        {
            return this.stateChange_[index];
        }

        private UpdateMemberStateNotification MakeReadOnly()
        {
            this.stateChange_.MakeReadOnly();
            return this;
        }

        public static UpdateMemberStateNotification ParseDelimitedFrom(Stream input)
        {
            return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
        }

        public static UpdateMemberStateNotification ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
        }

        public static UpdateMemberStateNotification ParseFrom(ByteString data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static UpdateMemberStateNotification ParseFrom(ICodedInputStream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static UpdateMemberStateNotification ParseFrom(byte[] data)
        {
            return CreateBuilder().MergeFrom(data).BuildParsed();
        }

        public static UpdateMemberStateNotification ParseFrom(Stream input)
        {
            return CreateBuilder().MergeFrom(input).BuildParsed();
        }

        public static UpdateMemberStateNotification ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public static UpdateMemberStateNotification ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static UpdateMemberStateNotification ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
        }

        public static UpdateMemberStateNotification ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
        {
            return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
        }

        public override void PrintTo(TextWriter writer)
        {
            GeneratedMessageLite<UpdateMemberStateNotification, Builder>.PrintField("agent_id", this.hasAgentId, this.agentId_, writer);
            GeneratedMessageLite<UpdateMemberStateNotification, Builder>.PrintField<Member>("state_change", this.stateChange_, writer);
        }

        public override Builder ToBuilder()
        {
            return CreateBuilder(this);
        }

        public override void WriteTo(ICodedOutputStream output)
        {
            int serializedSize = this.SerializedSize;
            string[] strArray = _updateMemberStateNotificationFieldNames;
            if (this.hasAgentId)
            {
                output.WriteMessage(1, strArray[0], this.AgentId);
            }
            if (this.stateChange_.Count > 0)
            {
                output.WriteMessageArray<Member>(2, strArray[1], this.stateChange_);
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

        public static UpdateMemberStateNotification DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
        }

        public override UpdateMemberStateNotification DefaultInstanceForType
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

        public override bool IsInitialized
        {
            get
            {
                if (this.HasAgentId && !this.AgentId.IsInitialized)
                {
                    return false;
                }
                IEnumerator<Member> enumerator = this.StateChangeList.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Member current = enumerator.Current;
                        if (!current.IsInitialized)
                        {
                            return false;
                        }
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
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
                    IEnumerator<Member> enumerator = this.StateChangeList.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            Member current = enumerator.Current;
                            memoizedSerializedSize += CodedOutputStream.ComputeMessageSize(2, current);
                        }
                    }
                    finally
                    {
                        if (enumerator == null)
                        {
                        }
                        enumerator.Dispose();
                    }
                    this.memoizedSerializedSize = memoizedSerializedSize;
                }
                return memoizedSerializedSize;
            }
        }

        public int StateChangeCount
        {
            get
            {
                return this.stateChange_.Count;
            }
        }

        public IList<Member> StateChangeList
        {
            get
            {
                return this.stateChange_;
            }
        }

        protected override UpdateMemberStateNotification ThisMessage
        {
            get
            {
                return this;
            }
        }

        [DebuggerNonUserCode, CompilerGenerated, GeneratedCode("ProtoGen", "2.4.1.473")]
        public sealed class Builder : GeneratedBuilderLite<UpdateMemberStateNotification, UpdateMemberStateNotification.Builder>
        {
            private UpdateMemberStateNotification result;
            private bool resultIsReadOnly;

            public Builder()
            {
                this.result = UpdateMemberStateNotification.DefaultInstance;
                this.resultIsReadOnly = true;
            }

            internal Builder(UpdateMemberStateNotification cloneFrom)
            {
                this.result = cloneFrom;
                this.resultIsReadOnly = true;
            }

            public UpdateMemberStateNotification.Builder AddRangeStateChange(IEnumerable<Member> values)
            {
                this.PrepareBuilder();
                this.result.stateChange_.Add(values);
                return this;
            }

            public UpdateMemberStateNotification.Builder AddStateChange(Member value)
            {
                Google.ProtocolBuffers.ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.stateChange_.Add(value);
                return this;
            }

            public UpdateMemberStateNotification.Builder AddStateChange(Member.Builder builderForValue)
            {
                Google.ProtocolBuffers.ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
                this.PrepareBuilder();
                this.result.stateChange_.Add(builderForValue.Build());
                return this;
            }

            public override UpdateMemberStateNotification BuildPartial()
            {
                if (this.resultIsReadOnly)
                {
                    return this.result;
                }
                this.resultIsReadOnly = true;
                return this.result.MakeReadOnly();
            }

            public override UpdateMemberStateNotification.Builder Clear()
            {
                this.result = UpdateMemberStateNotification.DefaultInstance;
                this.resultIsReadOnly = true;
                return this;
            }

            public UpdateMemberStateNotification.Builder ClearAgentId()
            {
                this.PrepareBuilder();
                this.result.hasAgentId = false;
                this.result.agentId_ = null;
                return this;
            }

            public UpdateMemberStateNotification.Builder ClearStateChange()
            {
                this.PrepareBuilder();
                this.result.stateChange_.Clear();
                return this;
            }

            public override UpdateMemberStateNotification.Builder Clone()
            {
                if (this.resultIsReadOnly)
                {
                    return new UpdateMemberStateNotification.Builder(this.result);
                }
                return new UpdateMemberStateNotification.Builder().MergeFrom(this.result);
            }

            public Member GetStateChange(int index)
            {
                return this.result.GetStateChange(index);
            }

            public UpdateMemberStateNotification.Builder MergeAgentId(EntityId value)
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

            public override UpdateMemberStateNotification.Builder MergeFrom(UpdateMemberStateNotification other)
            {
                if (other != UpdateMemberStateNotification.DefaultInstance)
                {
                    this.PrepareBuilder();
                    if (other.HasAgentId)
                    {
                        this.MergeAgentId(other.AgentId);
                    }
                    if (other.stateChange_.Count != 0)
                    {
                        this.result.stateChange_.Add(other.stateChange_);
                    }
                }
                return this;
            }

            public override UpdateMemberStateNotification.Builder MergeFrom(ICodedInputStream input)
            {
                return this.MergeFrom(input, ExtensionRegistry.Empty);
            }

            public override UpdateMemberStateNotification.Builder MergeFrom(IMessageLite other)
            {
                if (other is UpdateMemberStateNotification)
                {
                    return this.MergeFrom((UpdateMemberStateNotification) other);
                }
                base.MergeFrom(other);
                return this;
            }

            public override UpdateMemberStateNotification.Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
            {
                uint num;
                string str;
                this.PrepareBuilder();
                while (input.ReadTag(out num, out str))
                {
                    if ((num == 0) && (str != null))
                    {
                        int index = Array.BinarySearch<string>(UpdateMemberStateNotification._updateMemberStateNotificationFieldNames, str, StringComparer.Ordinal);
                        if (index >= 0)
                        {
                            num = UpdateMemberStateNotification._updateMemberStateNotificationFieldTags[index];
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
                            break;

                        default:
                        {
                            if (WireFormat.IsEndGroupTag(num))
                            {
                                return this;
                            }
                            this.ParseUnknownField(input, extensionRegistry, num, str);
                            continue;
                        }
                    }
                    input.ReadMessageArray<Member>(num, str, this.result.stateChange_, Member.DefaultInstance, extensionRegistry);
                }
                return this;
            }

            private UpdateMemberStateNotification PrepareBuilder()
            {
                if (this.resultIsReadOnly)
                {
                    UpdateMemberStateNotification result = this.result;
                    this.result = new UpdateMemberStateNotification();
                    this.resultIsReadOnly = false;
                    this.MergeFrom(result);
                }
                return this.result;
            }

            public UpdateMemberStateNotification.Builder SetAgentId(EntityId value)
            {
                Google.ProtocolBuffers.ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.hasAgentId = true;
                this.result.agentId_ = value;
                return this;
            }

            public UpdateMemberStateNotification.Builder SetAgentId(EntityId.Builder builderForValue)
            {
                Google.ProtocolBuffers.ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
                this.PrepareBuilder();
                this.result.hasAgentId = true;
                this.result.agentId_ = builderForValue.Build();
                return this;
            }

            public UpdateMemberStateNotification.Builder SetStateChange(int index, Member value)
            {
                Google.ProtocolBuffers.ThrowHelper.ThrowIfNull(value, "value");
                this.PrepareBuilder();
                this.result.stateChange_[index] = value;
                return this;
            }

            public UpdateMemberStateNotification.Builder SetStateChange(int index, Member.Builder builderForValue)
            {
                Google.ProtocolBuffers.ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
                this.PrepareBuilder();
                this.result.stateChange_[index] = builderForValue.Build();
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

            public override UpdateMemberStateNotification DefaultInstanceForType
            {
                get
                {
                    return UpdateMemberStateNotification.DefaultInstance;
                }
            }

            public bool HasAgentId
            {
                get
                {
                    return this.result.hasAgentId;
                }
            }

            public override bool IsInitialized
            {
                get
                {
                    return this.result.IsInitialized;
                }
            }

            protected override UpdateMemberStateNotification MessageBeingBuilt
            {
                get
                {
                    return this.PrepareBuilder();
                }
            }

            public int StateChangeCount
            {
                get
                {
                    return this.result.StateChangeCount;
                }
            }

            public IPopsicleList<Member> StateChangeList
            {
                get
                {
                    return this.PrepareBuilder().stateChange_;
                }
            }

            protected override UpdateMemberStateNotification.Builder ThisBuilder
            {
                get
                {
                    return this;
                }
            }
        }
    }
}

