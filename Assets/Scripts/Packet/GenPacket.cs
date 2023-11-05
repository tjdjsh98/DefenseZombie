using ServerCore;
using System.Net;
using System.Text;
using System;
using System.Collections.Generic;

public enum PacketID
{
    C_RequestEnterGame = 1,
	S_AnswerEnterGame = 2,
	C_SuccessToEnterServer = 3,
	S_BroadcastNewClient = 4,
	C_LeaveGame = 5,
	S_BroadcastLeaveGame = 6,
	S_EnterSyncInfos = 7,
	C_CharacterInfo = 8,
	S_BroadcastCharacterInfo = 9,
	C_ItemInfo = 10,
	S_BroadcastItemInfo = 11,
	C_BuildingInfo = 12,
	S_BroadcastBuildingInfo = 13,
	C_Damage = 14,
	S_BroadcastDamage = 15,
	C_RequestGenerateCharacter = 16,
	S_BroadcastGenerateCharacter = 17,
	C_RequestRemoveCharacter = 18,
	S_BroadcastRemoveCharacter = 19,
	C_RequestGenerateItem = 20,
	S_BroadcastGenerateItem = 21,
	C_RequestRemoveItem = 22,
	S_BroadcastRemoveItem = 23,
	C_RequestGenerateBuilding = 24,
	S_BroadcastGenerateBuilding = 25,
	C_RequestRemoveBuilding = 26,
	S_BroadcastRemoveBuilding = 27,
	
}

public interface IPacket
{
	ushort Protocol { get;}
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}
    

public class C_RequestEnterGame : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.C_RequestEnterGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RequestEnterGame), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_AnswerEnterGame : IPacket
{
    public bool permission;
	public int playerId;

    public ushort Protocol { get { return (ushort)PacketID.S_AnswerEnterGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.permission = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_AnswerEnterGame), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(permission), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(playerId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_SuccessToEnterServer : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.C_SuccessToEnterServer; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_SuccessToEnterServer), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastNewClient : IPacket
{
    public int clientId;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastNewClient; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.clientId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastNewClient), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(clientId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_LeaveGame : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.C_LeaveGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_LeaveGame), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastLeaveGame : IPacket
{
    public int playerId;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastLeaveGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastLeaveGame), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(playerId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_EnterSyncInfos : IPacket
{
    public class CharacterInfo
	{
	    public int characterId;
		public int characterName;
		public int hp;
		public float posX;
		public float posY;
		public string data;
	    public void Read(ArraySegment<byte> segment, ref ushort count)
	    {
	        this.characterId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			this.characterName = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			this.hp = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
			this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
			ushort dataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.data = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, dataLen);
			count += dataLen;
			
	    }
	
	    public bool Write(ArraySegment<byte> segment, ref ushort count)
	    {
	        bool success = true;
	        Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(characterName), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(hp), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
			Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
			ushort dataLen = (ushort)Encoding.Unicode.GetBytes(this.data,0,data.Length,segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(dataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += dataLen;
			
	        return success;
	    }
	}
	
	public List<CharacterInfo> characterInfos = new List<CharacterInfo>();
	
	public class ItemInfo
	{
	    public int itemId;
		public int itemName;
		public float posX;
		public float posY;
		public string data;
	    public void Read(ArraySegment<byte> segment, ref ushort count)
	    {
	        this.itemId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			this.itemName = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
			this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
			ushort dataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.data = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, dataLen);
			count += dataLen;
			
	    }
	
	    public bool Write(ArraySegment<byte> segment, ref ushort count)
	    {
	        bool success = true;
	        Array.Copy(BitConverter.GetBytes(itemId), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(itemName), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
			Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
			ushort dataLen = (ushort)Encoding.Unicode.GetBytes(this.data,0,data.Length,segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(dataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += dataLen;
			
	        return success;
	    }
	}
	
	public List<ItemInfo> itemInfos = new List<ItemInfo>();
	
	public class BuildingInfo
	{
	    public int buildingId;
		public int buildingName;
		public int hp;
		public float posX;
		public float posY;
		public int cellPosX;
		public int cellPosY;
		public string data;
	    public void Read(ArraySegment<byte> segment, ref ushort count)
	    {
	        this.buildingId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			this.buildingName = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			this.hp = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
			this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
			this.cellPosX = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			this.cellPosY = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			ushort dataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.data = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, dataLen);
			count += dataLen;
			
	    }
	
	    public bool Write(ArraySegment<byte> segment, ref ushort count)
	    {
	        bool success = true;
	        Array.Copy(BitConverter.GetBytes(buildingId), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(buildingName), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(hp), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
			Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
			Array.Copy(BitConverter.GetBytes(cellPosX), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(cellPosY), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			ushort dataLen = (ushort)Encoding.Unicode.GetBytes(this.data,0,data.Length,segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(dataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += dataLen;
			
	        return success;
	    }
	}
	
	public List<BuildingInfo> buildingInfos = new List<BuildingInfo>();
	

    public ushort Protocol { get { return (ushort)PacketID.S_EnterSyncInfos; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.characterInfos.Clear();
		ushort characterInfoLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for(int i =0; i < characterInfoLen; i++)
		{
		    CharacterInfo characterInfo = new CharacterInfo();
		    characterInfo.Read(segment, ref count);
		    characterInfos.Add(characterInfo);
		}
		
		this.itemInfos.Clear();
		ushort itemInfoLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for(int i =0; i < itemInfoLen; i++)
		{
		    ItemInfo itemInfo = new ItemInfo();
		    itemInfo.Read(segment, ref count);
		    itemInfos.Add(itemInfo);
		}
		
		this.buildingInfos.Clear();
		ushort buildingInfoLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for(int i =0; i < buildingInfoLen; i++)
		{
		    BuildingInfo buildingInfo = new BuildingInfo();
		    buildingInfo.Read(segment, ref count);
		    buildingInfos.Add(buildingInfo);
		}
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_EnterSyncInfos), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)this.characterInfos.Count), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (CharacterInfo characterInfo in characterInfos)
		    characterInfo.Write(segment,ref count);
		
		Array.Copy(BitConverter.GetBytes((ushort)this.itemInfos.Count), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (ItemInfo itemInfo in itemInfos)
		    itemInfo.Write(segment,ref count);
		
		Array.Copy(BitConverter.GetBytes((ushort)this.buildingInfos.Count), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (BuildingInfo buildingInfo in buildingInfos)
		    buildingInfo.Write(segment,ref count);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_CharacterInfo : IPacket
{
    public int characterId;
	public int hp;
	public float posX;
	public float posY;
	public string data;

    public ushort Protocol { get { return (ushort)PacketID.C_CharacterInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.characterId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.hp = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		ushort dataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.data = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, dataLen);
		count += dataLen;
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_CharacterInfo), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(hp), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		ushort dataLen = (ushort)Encoding.Unicode.GetBytes(this.data,0,data.Length,segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(dataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += dataLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastCharacterInfo : IPacket
{
    public int characterId;
	public int hp;
	public float posX;
	public float posY;
	public string data;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastCharacterInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.characterId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.hp = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		ushort dataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.data = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, dataLen);
		count += dataLen;
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastCharacterInfo), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(hp), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		ushort dataLen = (ushort)Encoding.Unicode.GetBytes(this.data,0,data.Length,segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(dataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += dataLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_ItemInfo : IPacket
{
    public int itemId;
	public float posX;
	public float posY;
	public string data;

    public ushort Protocol { get { return (ushort)PacketID.C_ItemInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.itemId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		ushort dataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.data = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, dataLen);
		count += dataLen;
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_ItemInfo), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(itemId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		ushort dataLen = (ushort)Encoding.Unicode.GetBytes(this.data,0,data.Length,segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(dataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += dataLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastItemInfo : IPacket
{
    public int itemId;
	public float posX;
	public float posY;
	public string data;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastItemInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.itemId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		ushort dataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.data = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, dataLen);
		count += dataLen;
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastItemInfo), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(itemId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		ushort dataLen = (ushort)Encoding.Unicode.GetBytes(this.data,0,data.Length,segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(dataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += dataLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_BuildingInfo : IPacket
{
    public int buildingId;
	public int hp;
	public float posX;
	public float posY;
	public int cellPosX;
	public int cellPosY;
	public string data;

    public ushort Protocol { get { return (ushort)PacketID.C_BuildingInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.buildingId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.hp = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.cellPosX = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.cellPosY = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		ushort dataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.data = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, dataLen);
		count += dataLen;
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_BuildingInfo), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(buildingId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(hp), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(cellPosX), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(cellPosY), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		ushort dataLen = (ushort)Encoding.Unicode.GetBytes(this.data,0,data.Length,segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(dataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += dataLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastBuildingInfo : IPacket
{
    public int buildingId;
	public int hp;
	public float posX;
	public float posY;
	public int cellPosX;
	public int cellPosY;
	public string data;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastBuildingInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.buildingId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.hp = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.cellPosX = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.cellPosY = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		ushort dataLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.data = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, dataLen);
		count += dataLen;
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastBuildingInfo), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(buildingId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(hp), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(cellPosX), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(cellPosY), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		ushort dataLen = (ushort)Encoding.Unicode.GetBytes(this.data,0,data.Length,segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(dataLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += dataLen;
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_Damage : IPacket
{
    public int characterId;
	public float directionX;
	public float directionY;
	public float power;
	public float stagger;

    public ushort Protocol { get { return (ushort)PacketID.C_Damage; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.characterId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.directionX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.directionY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.power = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.stagger = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_Damage), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(directionX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(directionY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(power), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(stagger), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastDamage : IPacket
{
    public int characterId;
	public float directionX;
	public float directionY;
	public float power;
	public float stagger;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastDamage; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.characterId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.directionX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.directionY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.power = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.stagger = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastDamage), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(directionX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(directionY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(power), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(stagger), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_RequestGenerateCharacter : IPacket
{
    public int requestNumber;
	public bool isPlayerableChracter;
	public int characterName;
	public float posX;
	public float posY;

    public ushort Protocol { get { return (ushort)PacketID.C_RequestGenerateCharacter; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.isPlayerableChracter = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.characterName = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RequestGenerateCharacter), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(isPlayerableChracter), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(characterName), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastGenerateCharacter : IPacket
{
    public int requestNumber;
	public bool isSuccess;
	public int characterId;
	public int characterName;
	public float posX;
	public float posY;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastGenerateCharacter; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.isSuccess = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.characterId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.characterName = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastGenerateCharacter), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(isSuccess), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(characterName), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_RequestRemoveCharacter : IPacket
{
    public int requestNumber;
	public int characterId;

    public ushort Protocol { get { return (ushort)PacketID.C_RequestRemoveCharacter; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.characterId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RequestRemoveCharacter), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastRemoveCharacter : IPacket
{
    public int requestNumber;
	public int characterId;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastRemoveCharacter; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.characterId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastRemoveCharacter), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_RequestGenerateItem : IPacket
{
    public int requestNumber;
	public int itemName;
	public float posX;
	public float posY;

    public ushort Protocol { get { return (ushort)PacketID.C_RequestGenerateItem; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.itemName = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RequestGenerateItem), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(itemName), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastGenerateItem : IPacket
{
    public int requestNumber;
	public bool isSuccess;
	public int itemId;
	public int itemName;
	public float posX;
	public float posY;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastGenerateItem; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.isSuccess = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.itemId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.itemName = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastGenerateItem), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(isSuccess), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(itemId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(itemName), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_RequestRemoveItem : IPacket
{
    public int itemId;
	public int requestNumber;
	public float posX;
	public float posY;

    public ushort Protocol { get { return (ushort)PacketID.C_RequestRemoveItem; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.itemId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RequestRemoveItem), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(itemId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastRemoveItem : IPacket
{
    public int itemId;
	public bool isSuccess;
	public int requestNumber;
	public float posX;
	public float posY;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastRemoveItem; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.itemId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.isSuccess = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastRemoveItem), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(itemId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(isSuccess), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_RequestGenerateBuilding : IPacket
{
    public int requestNumber;
	public int buildingName;
	public int posX;
	public int posY;

    public ushort Protocol { get { return (ushort)PacketID.C_RequestGenerateBuilding; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.buildingName = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posY = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RequestGenerateBuilding), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(buildingName), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastGenerateBuilding : IPacket
{
    public int requestNumber;
	public int buildingId;
	public int buildingName;
	public bool isSuccess;
	public int posX;
	public int posY;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastGenerateBuilding; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.buildingId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.buildingName = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.isSuccess = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.posX = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posY = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastGenerateBuilding), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(buildingId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(buildingName), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(isSuccess), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_RequestRemoveBuilding : IPacket
{
    public int requestNumber;
	public int buildingId;

    public ushort Protocol { get { return (ushort)PacketID.C_RequestRemoveBuilding; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.buildingId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RequestRemoveBuilding), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(buildingId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastRemoveBuilding : IPacket
{
    public int requestNumber;
	public int buildingId;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastRemoveBuilding; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.requestNumber = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.buildingId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastRemoveBuilding), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(requestNumber), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(buildingId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

